namespace Pegasus.Platform.Network
{
	using System;
	using Memory;
	using Utilities;

	/// <summary>
	///     Represents a Udp-based socket that can be used to unreliably send and receive packets over the network.
	/// </summary>
	public sealed unsafe class UdpSocket : DisposableObject
	{
		/// <summary>
		///     The native Udp interface that is used to send and receive packets.
		/// </summary>
		private readonly UdpInterface* _udpInterface;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public UdpSocket()
		{
			_udpInterface = NativeMethods.CreateUdpInterface();

			if (!_udpInterface->Initialize())
				throw new NetworkException(_udpInterface);
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
			Assert.ArgumentSatisfies(size >= 0, "Invalid size.");
			Assert.ArgumentSatisfies(size <= buffer.Length, "Invalid size.");
			Assert.NotDisposed(this);

			if (size == 0)
				return;

			fixed (void* sendBuffer = buffer)
			{
				if (!_udpInterface->Send(sendBuffer, size, &remoteEndPoint))
					throw new NetworkException(_udpInterface);
			}
		}

		/// <summary>
		///     Tries to receive a packet sent over the connection. Returns true if a packet has been received, false otherwise.
		/// </summary>
		/// <param name="buffer">The buffer the received data should be written to.</param>
		/// <param name="remoteEndPoint">After the method completes, contains the endpoint of the peer that sent the packet.</param>
		/// <param name="size">Returns the number of bytes that have been received.</param>
		public bool TryReceive(byte[] buffer, out IPEndPoint remoteEndPoint, out int size)
		{
			Assert.ArgumentNotNull(buffer);
			Assert.NotDisposed(this);

			fixed (void* receiveBuffer = buffer)
			{
				IPEndPoint endPoint;
				int receivedBytes;

				switch ((ReceiveStatus)_udpInterface->TryReceive(receiveBuffer, buffer.Length, &endPoint, &receivedBytes))
				{
					case ReceiveStatus.Error:
						throw new NetworkException(_udpInterface);
					case ReceiveStatus.PacketReceived:
						remoteEndPoint = endPoint;
						size = receivedBytes;
						return true;
					case ReceiveStatus.NoPacketAvailable:
						remoteEndPoint = new IPEndPoint();
						size = 0;
						return false;
					default:
						throw new InvalidOperationException("Unexpected receive status.");
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

			if (!_udpInterface->Bind(port))
				throw new NetworkException(_udpInterface);
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

			if (!_udpInterface->BindMulticast(&multicastGroup, timeToLive))
				throw new NetworkException(_udpInterface);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			NativeMethods.FreeUdpInterface(_udpInterface);
		}
	}
}