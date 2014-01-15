namespace Pegasus.Platform.Network
{
	using System;
	using System.Net;
	using System.Net.Sockets;
	using Memory;

	/// <summary>
	///     Represents a Udp-based socket that can be used to unreliably send and receive packets over the network.
	/// </summary>
	public class UdpSocket : DisposableObject
	{
		/// <summary>
		///     The underlying socket that is used to send and receive packets.
		/// </summary>
		private readonly Socket _socket;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public UdpSocket()
		{
			_socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp) { Blocking = false };
			_socket.EnableDualMode();
		}

		/// <summary>
		///     Gets the underlying socket instance.
		/// </summary>
		internal Socket Socket
		{
			get { return _socket; }
		}

		/// <summary>
		///     Sends the given packet over the connection.
		/// </summary>
		/// <param name="buffer">The buffer that contains the data that should be sent.</param>
		/// <param name="size">The number of bytes that should be sent.</param>
		/// <param name="remoteEndPoint">The endpoint of the peer the packet should be sent to.</param>
		public void Send(byte[] buffer, int size, IPEndPoint remoteEndPoint)
		{
			Assert.ArgumentNotNull(buffer);
			Assert.ArgumentNotNull(remoteEndPoint);

			try
			{
				_socket.SendTo(buffer, 0, size, SocketFlags.None, remoteEndPoint);
			}
			catch (SocketException e)
			{
				throw new SocketOperationException("Unable to send Udp packet to {0}: {1}.", remoteEndPoint, e.Message);
			}
		}

		/// <summary>
		///     Tries to receive a packet sent over the connection. Returns true if a packet has been received, false otherwise.
		/// </summary>
		/// <param name="buffer">The buffer the received data should be written to.</param>
		/// <param name="remoteEndPoint">After the method completes, contains the endpoint of the peer that sent the packet.</param>
		/// <param name="size">Returns the number of bytes that have been received.</param>
		public bool TryReceive(byte[] buffer, ref IPEndPoint remoteEndPoint, out int size)
		{
			Assert.ArgumentNotNull(buffer);
			Assert.ArgumentNotNull(remoteEndPoint);

			size = 0;

			if (_socket.Available == 0)
				return false;

			try
			{
				var endPoint = (EndPoint)remoteEndPoint;
				size = _socket.ReceiveFrom(buffer, ref endPoint);
				remoteEndPoint = (IPEndPoint)endPoint;

				return true;
			}
			catch (SocketException e)
			{
				throw new SocketOperationException("Error while trying to receive Udp packet: {0}.", e.Message);
			}
		}

		/// <summary>
		///     Binds the socket to the given local endpoint.
		/// </summary>
		/// <param name="localEndPoint">The local endpoint the socket should be bound to.</param>
		public void Bind(IPEndPoint localEndPoint)
		{
			Assert.ArgumentNotNull(localEndPoint);

			try
			{
				_socket.Bind(localEndPoint);
			}
			catch (SocketException e)
			{
				throw new SocketOperationException("Failed to bind Udp socket to {0}: {1}.", localEndPoint, e);
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_socket.Close();
			_socket.SafeDispose();
		}
	}
}