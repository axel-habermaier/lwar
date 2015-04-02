namespace Pegasus.Platform.Network
{
	using System;
	using System.Collections.Generic;
	using Memory;
	using Utilities;

	/// <summary>
	///     Listens for incoming UDP connections, creating a channel for each incoming connection.
	/// </summary>
	public sealed class UdpListener : DisposableObject
	{
		/// <summary>
		///     The allocator that is used to allocate packets and channels.
		/// </summary>
		private readonly PoolAllocator _allocator;

		/// <summary>
		///     The list of active UDP channels.
		/// </summary>
		private readonly List<UdpChannel> _channels = new List<UdpChannel>();

		/// <summary>
		///     The maximum supported packet size.
		/// </summary>
		private readonly int _maxPacketSize;

		/// <summary>
		///     The port the underlying socket is bound to.
		/// </summary>
		private readonly ushort _port;

		/// <summary>
		///     The UDP socket that is used for communication over the network.
		/// </summary>
		private readonly UdpSocket _socket;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate packets and channels.</param>
		/// <param name="port">The port the underlying socket should be bound to.</param>
		/// <param name="maxPacketSize">The maximum supported packet size.</param>
		public UdpListener(PoolAllocator allocator, ushort port, int maxPacketSize)
		{
			Assert.ArgumentNotNull(allocator);

			try
			{
				_allocator = allocator;
				_socket = new UdpSocket();
				_port = port;
				_maxPacketSize = maxPacketSize;
			}
			catch (NetworkException)
			{
				this.SafeDispose();
				throw;
			}
		}

		/// <summary>
		///     Starts listening for incoming connections.
		/// </summary>
		public void Start()
		{
			_socket.Bind(_port);
		}

		/// <summary>
		///     Raised when a new UDP channel has been created.
		/// </summary>
		public event Action<UdpChannel> ChannelCreated;

		/// <summary>
		///     Checks for new incoming UDP packets and creates new UDP channels, if necessary.
		/// </summary>
		public void Update()
		{
			IncomingUdpPacket packet = null;

			try
			{
				while (true)
				{
					packet = IncomingUdpPacket.Allocate(_allocator, _maxPacketSize);
					IPEndPoint sender;
					int size;

					if (!_socket.TryReceive(packet.Buffer, out sender, out size))
						return;

					packet.Size = size;
					HandlePacket(packet, sender);
					packet = null; // We cannot dispose the packet in the finally block, as it is still in use
				}
			}
			finally
			{
				packet.SafeDispose();
			}
		}

		/// <summary>
		///     Handles the given packet.
		/// </summary>
		/// <param name="packet">The packet that should be handled.</param>
		/// <param name="sender">The sender of the packet.</param>
		private void HandlePacket(IncomingUdpPacket packet, IPEndPoint sender)
		{
			// Get the channel that was created for the sender and let it handle the incoming packet
			foreach (var channel in _channels)
			{
				if (channel.RemoteEndPoint != sender)
					continue;

				channel.HandlePacket(packet);
				return;
			}

			// We don't have a channel yet for this sender, so create one
			var newChannel = UdpChannel.Create(_allocator, sender, _socket, this);
			newChannel.HandlePacket(packet);

			Assert.NotNull(ChannelCreated, "No event handler has been registered on the ChannelCreated event.");

			_channels.Add(newChannel);
			ChannelCreated(newChannel);
		}

		/// <summary>
		///     Removes the given channel.
		/// </summary>
		/// <param name="channel">The channel that should be removed.</param>
		internal void Remove(UdpChannel channel)
		{
			Assert.ArgumentNotNull(channel);
			Assert.ArgumentSatisfies(_channels.Contains(channel), "Unknown channel.");

			_channels.Remove(channel);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			// Strange but necessary, as a channel's Dispose method remove the channel from the list
			while (_channels.Count > 0)
				_channels[_channels.Count - 1].SafeDispose();

			_socket.SafeDispose();
		}
	}
}