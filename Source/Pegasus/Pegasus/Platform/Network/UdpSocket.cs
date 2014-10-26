namespace Pegasus.Platform.Network
{
	using System;
	using System.Runtime.InteropServices;
	using System.Security;
	using Memory;
	using Utilities;

	/// <summary>
	///     Represents a Udp-based socket that can be used to unreliably send and receive packets over the network.
	/// </summary>
	public sealed class UdpSocket : DisposableObject
	{
		/// <summary>
		///     The underlying socket that is used to send and receive packets.
		/// </summary>
		private readonly IntPtr _socket;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public UdpSocket()
		{
			_socket = NativeMethods.CreateSocket();
			if (_socket == IntPtr.Zero)
				throw new NetworkException();
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

			var address = remoteEndPoint.Address;
			fixed (byte* data = buffer)
			{
				var packet = new NativeMethods.Packet
				{
					Capacity = (uint)buffer.Length,
					Data = data,
					Address = &address,
					Port = remoteEndPoint.Port,
					Size = (uint)size
				};

				if (!NativeMethods.Send(_socket, &packet))
					throw new NetworkException();
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

			size = 0;
			remoteEndPoint = new IPEndPoint();
			var address = new IPAddress();

			fixed (byte* data = buffer)
			{
				var packet = new NativeMethods.Packet
				{
					Capacity = (uint)buffer.Length,
					Data = data,
					Address = &address
				};

				switch (NativeMethods.TryReceive(_socket, &packet))
				{
					case NativeMethods.ReceiveStatus.Error:
						throw new NetworkException();
					case NativeMethods.ReceiveStatus.NoPacketAvailable:
						return false;
					case NativeMethods.ReceiveStatus.PacketReceived:
						size = (int)packet.Size;
						remoteEndPoint = new IPEndPoint(address, packet.Port);
						return true;
					default:
						throw new InvalidOperationException("Unknown receive status.");
				}
			}
		}

		/// <summary>
		///     Binds the socket to the given port.
		/// </summary>
		/// <param name="port">The port the socket should be bound to.</param>
		public void Bind(ushort port)
		{
			Assert.NotDisposed(this);
			if (!NativeMethods.BindSocket(_socket, port))
				throw new NetworkException();
		}

		/// <summary>
		///     Binds the socket to the given port.
		/// </summary>
		/// <param name="multicastGroup">The multicast group that the socket should listen to or send to.</param>
		/// <param name="timeToLive">The time to live for the multicast messages.</param>
		public unsafe void BindMulticast(IPEndPoint multicastGroup, int timeToLive = 1)
		{
			Assert.NotDisposed(this);
			Assert.ArgumentInRange(timeToLive, 1, Int32.MaxValue);

			var address = multicastGroup.Address;
			if (!NativeMethods.BindMulticast(_socket, timeToLive, &address, multicastGroup.Port))
				throw new NetworkException();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			NativeMethods.DestroySocket(_socket);
		}

		/// <summary>
		///     Provides access to the native socket functions.
		/// </summary>
		[SuppressUnmanagedCodeSecurity]
		private static class NativeMethods
		{
			public enum ReceiveStatus
			{
				Error = 0,
				PacketReceived = 1,
				NoPacketAvailable = 2
			}

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCreateUdpSocket")]
			public static extern IntPtr CreateSocket();

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDestroyUdpSocket")]
			public static extern bool DestroySocket(IntPtr socket);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgTryReceiveUdpPacket")]
			public static extern unsafe ReceiveStatus TryReceive(IntPtr socket, Packet* packet);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSendUdpPacket")]
			public static extern unsafe bool Send(IntPtr socket, Packet* packet);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgBindUdpSocket")]
			public static extern bool BindSocket(IntPtr socket, ushort port);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgBindUdpSocketMulticast")]
			public static extern unsafe bool BindMulticast(IntPtr socket, int timeToLive, IPAddress* address, ushort port);

			[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
			[StructLayout(LayoutKind.Sequential)]
			public unsafe struct Packet
			{
				public byte* Data;
				public uint Size;
				public uint Capacity;
				public IPAddress* Address;
				public ushort Port;
			}
		}
	}
}