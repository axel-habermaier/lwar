namespace Pegasus.Platform.Network
{
	using System;
	using System.Diagnostics;
	using System.Net.Sockets;
	using System.Runtime.InteropServices;
	using Memory;
	using Utilities;

	/// <summary>
	///     Represents a Udp-based socket that can be used to unreliably send and receive packets over the network.
	/// </summary>
	/// <remarks>
	///     We're wrapping System.Net.Sockets.Socket in order to reduce the pressure on the garbage collector as each
	///     invocation of System.Net.Sockets.Socket.ReceiveFrom and System.Net.Sockets.Socket.SendTo allocates memory.
	///     Additionally, we always throw NetworkExceptions instead of SocketExceptions to simplify exception handling
	///     logic at higher layers.
	/// </remarks>
	public sealed class UdpSocket : DisposableObject
	{
		/// <summary>
		///     The underlying socket that is used to send and receive packets.
		/// </summary>
		private readonly Socket _socket;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public unsafe UdpSocket()
		{
			try
			{
				_socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp) { Blocking = false };

				// We only enable dual mode on Windows and don't care about other platforms; on Linux, it seems to be the default anyway
				if (PlatformInfo.Platform != PlatformType.Windows)
					return;

				// We can't use the DualMode property, as it is not available on Mono
				const SocketOptionName ipv6Only = (SocketOptionName)27;
				var falseValue = 0;

				if (NativeMethodsWindows.setsockopt(_socket.Handle.ToInt32(), SocketOptionLevel.IPv6, ipv6Only, &falseValue, sizeof(int)) != 0)
					ThrowSocketError("Unable to switch UDP socket to dual-stack mode.");
			}
			catch (SocketException e)
			{
				ThrowAsNetworkException(e);
			}
		}

		/// <summary>
		///     Sends the given packet over the connection.
		/// </summary>
		/// <param name="buffer">The buffer that contains the data that should be sent.</param>
		/// <param name="size">The number of bytes that should be sent.</param>
		/// <param name="remoteEndPoint">The endpoint of the peer the packet should be sent to.</param>
		public unsafe void Send(byte[] buffer, int size, IPEndPoint remoteEndPoint)
		{
			Assert.ArgumentNotNull(buffer);
			Assert.ArgumentSatisfies(size >= 0, "Invalid size.");
			Assert.ArgumentSatisfies(size <= buffer.Length, "Invalid size.");
			Assert.NotDisposed(this);

			if (size == 0)
				return;

			var address = new SocketAddress
			{
				AddressFamily = AddressFamily.InterNetworkV6,
				Port = remoteEndPoint.Port
			};

			remoteEndPoint.Address.CopyTo(address.IPv6);

			fixed (byte* data = buffer)
			{
				int sent;
				if (PlatformInfo.Platform == PlatformType.Windows)
					sent = NativeMethodsWindows.sendto(_socket.Handle, data, size, SocketFlags.None, &address, sizeof(SocketAddress));
				else
					sent = NativeMethodsLinux.sendto(_socket.Handle.ToInt32(), data, size, SocketFlags.None, &address, sizeof(SocketAddress));

				if (sent < 0)
					ThrowSocketError();

				if (sent != size)
					throw new NetworkException("UDP packet was sent only partially.");
			}
		}

		/// <summary>
		///     Tries to receive a packet sent over the connection. Returns true if a packet has been received, false otherwise.
		/// </summary>
		/// <param name="buffer">The buffer the received data should be written to.</param>
		/// <param name="remoteEndPoint">After the method completes, contains the endpoint of the peer that sent the packet.</param>
		/// <param name="size">Returns the number of bytes that have been received.</param>
		public unsafe bool TryReceive(byte[] buffer, out IPEndPoint remoteEndPoint, out int size)
		{
			Assert.ArgumentNotNull(buffer);
			Assert.NotDisposed(this);

			if (_socket.Available <= 0)
			{
				remoteEndPoint = new IPEndPoint();
				size = 0;
				return false;
			}

			fixed (byte* data = buffer)
			{
				var address = new SocketAddress();
				var length = sizeof(SocketAddress);

				if (PlatformInfo.Platform == PlatformType.Windows)
					size = NativeMethodsWindows.recvfrom(_socket.Handle, data, buffer.Length, SocketFlags.None, &address, &length);
				else
					size = NativeMethodsLinux.recvfrom(_socket.Handle.ToInt32(), data, buffer.Length, SocketFlags.None, &address, &length);

				if (size < 0)
					ThrowSocketError();

				remoteEndPoint = new IPEndPoint(new IPAddress(ref address), address.Port);
				return true;
			}
		}

		/// <summary>
		///     Binds the socket to the given port.
		/// </summary>
		/// <param name="port">The port the socket should be bound to.</param>
		public void Bind(ushort port)
		{
			Assert.NotDisposed(this);

			try
			{
				_socket.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.IPv6Any, port));
			}
			catch (SocketException e)
			{
				ThrowAsNetworkException(e);
			}
		}

		/// <summary>
		///     Binds the socket to the given port.
		/// </summary>
		/// <param name="multicastGroup">The multicast group that the socket should listen to or send to.</param>
		/// <param name="timeToLive">The time to live for the multicast messages.</param>
		public void BindMulticast(IPEndPoint multicastGroup, int timeToLive = 1)
		{
			Assert.NotDisposed(this);
			Assert.ArgumentInRange(timeToLive, 1, Int32.MaxValue);

			var address = multicastGroup.Address.ToSystemAddress();

			try
			{
				_socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.MulticastTimeToLive, timeToLive);
				_socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.MulticastLoopback, true);
				_socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership, new IPv6MulticastOption(address));
				_socket.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.IPv6Any, multicastGroup.Port));
			}
			catch (SocketException e)
			{
				ThrowAsNetworkException(e);
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_socket.SafeDispose();
		}

		/// <summary>
		///     Throws a network exception after a socket error.
		/// </summary>
		/// <param name="message">An optional additional message to display.</param>
		[DebuggerHidden]
		private static void ThrowSocketError(string message = null)
		{
			ThrowAsNetworkException(new SocketException(Marshal.GetLastWin32Error()), message);
		}

		/// <summary>
		///     Wraps a socket exception in a network exception.
		/// </summary>
		/// <param name="e">The socket exception that should be wrapped.</param>
		/// <param name="message">An optional additional message to display.</param>
		[DebuggerHidden]
		private static void ThrowAsNetworkException(SocketException e, string message = null)
		{
			if (message != null)
				message = String.Format("{0} {1}", message, e.Message.Trim());
			else
				message = e.Message.Trim();

			if (!message.EndsWith("."))
				message += ".";

			throw new NetworkException(message);
		}

		/// <summary>
		///     Provides access to the native SendTo and ReceiveFrom functions.
		/// </summary>
		private static class NativeMethodsWindows
		{
			private const string DllName = "Ws2_32.dll";

			[DllImport(DllName, SetLastError = true)]
			public static extern unsafe int setsockopt(int socket, SocketOptionLevel level, SocketOptionName optname, void* optval, int optlen);

			[DllImport(DllName, SetLastError = true)]
			public static extern unsafe int recvfrom(IntPtr socket, byte* buffer, int len, SocketFlags flags, void* addr, int* addrLen);

			[DllImport(DllName, SetLastError = true)]
			public static extern unsafe int sendto(IntPtr socket, byte* buffer, int len, SocketFlags flags, void* addr, int addrLen);
		}

		/// <summary>
		///     Provides access to the native SendTo and ReceiveFrom functions.
		/// </summary>
		private static class NativeMethodsLinux
		{
			private const string DllName = "libc";

			[DllImport(DllName, SetLastError = true)]
			public static extern unsafe int recvfrom(int socket, byte* buffer, int len, SocketFlags flags, void* addr, int* addrLen);

			[DllImport(DllName, SetLastError = true)]
			public static extern unsafe int sendto(int socket, byte* buffer, int len, SocketFlags flags, void* addr, int addrLen);
		}
	}
}