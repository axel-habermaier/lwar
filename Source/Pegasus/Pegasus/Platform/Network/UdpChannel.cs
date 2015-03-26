namespace Pegasus.Platform.Network
{
	using System;
	using System.Collections.Generic;
	using System.Net.Sockets;
	using Logging;
	using Memory;
	using Utilities;

	/// <summary>
	///     Represents a UDP channel to a remote peer.
	/// </summary>
	public sealed class UdpChannel : UniquePooledObject
	{
		/// <summary>
		///     The allocator that is used to allocate packets.
		/// </summary>
		private PoolAllocator _allocator;

		/// <summary>
		///     Indicates whether the underlying socket is bound and can be used to receive messages.
		/// </summary>
		private bool _isBound;

		/// <summary>
		///     The UDP listener that created the channel.
		/// </summary>
		private UdpListener _listener;

		/// <summary>
		///     The maximum supported packet size.
		/// </summary>
		private int _maxPacketSize;

		/// <summary>
		///     Stores the packets received by a UDP listener.
		/// </summary>
		private Queue<Shared<IncomingUdpPacket>> _packets;

		/// <summary>
		///     Indicates whether packets should be received from the socket or retrieved from the packets queue.
		/// </summary>
		private bool _receiveFromSocket;

		/// <summary>
		///     The UDP socket that is used for communication over the network.
		/// </summary>
		private UdpSocket _socket;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static UdpChannel()
		{
			ConstructorCache.Register(() => new UdpChannel());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private UdpChannel()
		{
		}

		/// <summary>
		///     Gets a value indicating whether the channel to the remote peer is faulted and can no longer be used.
		/// </summary>
		public bool IsFaulted { get; private set; }

		/// <summary>
		///     Gets the remote endpoint of the channel.
		/// </summary>
		public IPEndPoint RemoteEndPoint { get; private set; }

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate objects.</param>
		/// <param name="remoteEndPoint">The remote endpoint of the channel.</param>
		/// <param name="maxPacketSize">The maximum supported packet size.</param>
		public static UdpChannel Create(PoolAllocator allocator, IPEndPoint remoteEndPoint, int maxPacketSize)
		{
			Assert.ArgumentNotNull(allocator);

			var channel = allocator.Allocate<UdpChannel>();
			channel.RemoteEndPoint = remoteEndPoint;
			channel._socket = new UdpSocket();
			channel._maxPacketSize = maxPacketSize;
			channel._receiveFromSocket = true;
			channel._allocator = allocator;
			return channel;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate objects.</param>
		/// <param name="remoteEndPoint">The remote endpoint of the channel.</param>
		/// <param name="socket">The shared socket that is used for communcation over the network.</param>
		/// <param name="listener">The UDP listener that created the channel.</param>
		internal static UdpChannel Create(PoolAllocator allocator, IPEndPoint remoteEndPoint, UdpSocket socket, UdpListener listener)
		{
			Assert.ArgumentNotNull(allocator);
			Assert.ArgumentNotNull(socket);
			Assert.ArgumentNotNull(listener);

			var channel = allocator.Allocate<UdpChannel>();
			channel.RemoteEndPoint = remoteEndPoint;
			channel._socket = socket;
			channel._listener = listener;
			channel._isBound = true;
			channel._receiveFromSocket = false;
			channel._packets = channel._packets ?? new Queue<IncomingUdpPacket>();
			channel._allocator = allocator;
			return channel;
		}

		/// <summary>
		///     Sends the given packet over the connection.
		/// </summary>
		/// <param name="packet">The packet containing the data that should be sent.</param>
		/// <param name="size">The size of the packet in bytes.</param>
		public void Send(OutgoingUdpPacket packet, int size)
		{
			Assert.ArgumentNotNull(packet);
			Assert.NotPooled(this);
			Assert.That(!IsFaulted, "The channel is faulted and can no longer be used.");

			try
			{
				_socket.Send(packet.Buffer, size, RemoteEndPoint);
				_isBound = true;
			}
			catch (SocketException)
			{
				IsFaulted = true;
				throw;
			}
		}

		/// <summary>
		///     Tries to receive a packet sent over the connection. Returns true if a packet has been received, false otherwise.
		/// </summary>
		/// <param name="packet">The packet that contains the received data.</param>
		public bool TryReceive(out Shared<IncomingUdpPacket> packet)
		{
			Assert.NotPooled(this);
			Assert.That(!IsFaulted, "The channel is faulted and can no longer be used.");
			Assert.That(_isBound, "The underlying socket has not yet been bound and cannot be used to receive any packets.");

			packet = null;
			if (!_receiveFromSocket)
			{
				if (_packets.Count == 0)
					return false;

				packet = _packets.Dequeue();
				return true;
			}

			var allocatedPacket = _allocator.AllocateIncomingUdpPacket(_maxPacketSize);;
			try
			{
				while (true)
				{
					IPEndPoint sender;
					int size;

					if (!_socket.TryReceive(allocatedPacket.Object.Buffer, out sender, out size))
						return false;

					if (sender != RemoteEndPoint)
						Log.Warn("Received a packet from {0}, but expecting packets from {1} only. Packet was ignored.", sender, RemoteEndPoint);
					else
					{
						packet = allocatedPacket;
						packet.Object.Size = size;
						allocatedPacket = null; // We cannot dispose the packet in the finally block, as it is still in use
						return true;
					}
				}
			}
			catch (SocketException)
			{
				IsFaulted = true;
				throw;
			}
			finally
			{
				allocatedPacket.SafeDispose();
			}
		}

		/// <summary>
		///     Handles a packet received by a UDP listener.
		/// </summary>
		/// <param name="packet">The packet that contains the received data.</param>
		internal void HandlePacket(IncomingUdpPacket packet)
		{
			Assert.That(!_receiveFromSocket, "The channel is configured to receive messages directly from the underlying socket.");
			_packets.Enqueue(packet);
		}

		/// <summary>
		///     Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override void OnReturning()
		{
			if (_listener == null)
				_socket.SafeDispose();
			else
				_listener.Remove(this);

			_packets.SafeDisposeAll();
			_socket = null;
			_listener = null;
			_isBound = false;
			IsFaulted = false;
		}
	}
}