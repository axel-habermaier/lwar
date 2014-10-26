namespace Pegasus.Platform.Network
{
	using System;
	using System.Collections.Generic;
	using Logging;
	using Memory;
	using Utilities;

	/// <summary>
	///     Represents a UDP channel to a remote peer.
	/// </summary>
	public sealed class UdpChannel : UniquePooledObject
	{
		/// <summary>
		///     The application-wide object pool that is used to allocate UDP channels.
		/// </summary>
		private static readonly ObjectPool<UdpChannel> Pool =
			new ObjectPool<UdpChannel>(() => new UdpChannel(), hasGlobalLifetime: true);

		/// <summary>
		///     Indicates whether the underlying socket is bound and can be used to receive messages.
		/// </summary>
		private bool _isBound;

		/// <summary>
		/// Indicates whether packets should be received from the socket or retrieved from the packets queue.
		/// </summary>
		private bool _receiveFromSocket;

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
		private Queue<UdpPacket> _packets;

		/// <summary>
		///     The UDP socket that is used for communication over the network.
		/// </summary>
		private UdpSocket _socket;

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
		/// <param name="remoteEndPoint">The remote endpoint of the channel.</param>
		/// <param name="maxPacketSize">The maximum supported packet size.</param>
		public static UdpChannel Create(IPEndPoint remoteEndPoint, int maxPacketSize)
		{
			var channel = Pool.Allocate();
			channel.RemoteEndPoint = remoteEndPoint;
			channel._socket = new UdpSocket();
			channel._maxPacketSize = maxPacketSize;
			channel._receiveFromSocket = true;
			return channel;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="remoteEndPoint">The remote endpoint of the channel.</param>
		/// <param name="socket">The shared socket that is used for communcation over the network.</param>
		/// <param name="listener">The UDP listener that created the channel.</param>
		internal static UdpChannel Create(IPEndPoint remoteEndPoint, UdpSocket socket, UdpListener listener)
		{
			Assert.ArgumentNotNull(socket);
			Assert.ArgumentNotNull(listener);

			var channel = Pool.Allocate();
			channel.RemoteEndPoint = remoteEndPoint;
			channel._socket = socket;
			channel._listener = listener;
			channel._isBound = true;
			channel._receiveFromSocket = false;
			channel._packets = channel._packets ?? new Queue<UdpPacket>();
			return channel;
		}

		/// <summary>
		///     Sends the given packet over the connection.
		/// </summary>
		/// <param name="packet">The packet containing the data that should be sent.</param>
		public void Send(UdpPacket packet)
		{
			Assert.ArgumentNotNull(packet);
			Assert.NotPooled(this);
			Assert.That(!IsFaulted, "The channel is faulted and can no longer be used.");

			try
			{
				_socket.Send(packet.Buffer, packet.Size, RemoteEndPoint);
				_isBound = true;
			}
			catch (NetworkException)
			{
				IsFaulted = true;
				throw;
			}
		}

		/// <summary>
		///     Tries to receive a packet sent over the connection. Returns true if a packet has been received, false otherwise.
		/// </summary>
		/// <param name="packet">The packet that contains the received data.</param>
		public bool TryReceive(out UdpPacket packet)
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

			UdpPacket allocatedPacket = null;
			try
			{
				allocatedPacket = UdpPacket.Allocate(_maxPacketSize);
				while (true)
				{
					IPEndPoint sender;
					int size;

					if (!_socket.TryReceive(allocatedPacket.Buffer, out sender, out size))
						return false;

					if (sender != RemoteEndPoint)
						Log.Warn("Received a packet from {0}, but expecting packets from {1} only. Packet was ignored.", sender, RemoteEndPoint);
					else
					{
						packet = allocatedPacket;
						packet.Size = size;
						allocatedPacket = null; // We cannot dispose the packet in the finally block, as it is still in use
						return true;
					}
				}
			}
			catch (NetworkException)
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
		internal void HandlePacket(UdpPacket packet)
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