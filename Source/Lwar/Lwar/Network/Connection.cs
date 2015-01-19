namespace Lwar.Network
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using Messages;
	using Pegasus.Platform;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;
	using Pegasus.Platform.Network;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a connection to a remote peer, using the Lwar network protocol specification.
	/// </summary>
	internal class Connection : UniquePooledObject
	{
		/// <summary>
		///     The delivery manager responsible for the delivery guarantees of all incoming and outgoing messages.
		/// </summary>
		private readonly DeliveryManager _deliveryManager;

		/// <summary>
		///     The message queue responsible for packing all queued outgoing messages into a packet. Reliable messages will be
		///     resent until their reception has been acknowledged by the remote peer.
		/// </summary>
		private readonly MessageQueue _outgoingMessages;

		/// <summary>
		///     A cached queue of received messages.
		/// </summary>
		private readonly Queue<SequencedMessage> _receivedMessages = new Queue<SequencedMessage>();

		/// <summary>
		///     The allocator that is used to allocate message objects.
		/// </summary>
		private PoolAllocator _allocator;

		/// <summary>
		///     The Udp channel that is used for the communication with the remote peer.
		/// </summary>
		private UdpChannel _channel;

		/// <summary>
		///     Provides the time that is used to check whether a connection is lagging or dropped.
		/// </summary>
		private Clock _clock = new Clock();

		/// <summary>
		///     The deserializer that is used to deserialize incoming messages.
		/// </summary>
		private MessageDeserializer _deserializer;

		/// <summary>
		///     The time in milliseconds since the last packet has been received.
		/// </summary>
		private double _timeSinceLastPacket;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Connection()
		{
			ConstructorCache.Register(() => new Connection());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private Connection()
		{
			_deliveryManager = new DeliveryManager();
			_outgoingMessages = new MessageQueue(_deliveryManager);
		}

		/// <summary>
		///     Gets a value indicating whether the connection to the remote peer has been dropped.
		/// </summary>
		public bool IsDropped { get; private set; }

		/// <summary>
		///     Gets the endpoint of the remote peer.
		/// </summary>
		public IPEndPoint RemoteEndPoint
		{
			get { return _channel.RemoteEndPoint; }
		}

		/// <summary>
		///     Gets the remaining time in milliseconds before the connection will be dropped.
		/// </summary>
		public double TimeToDrop
		{
			get { return NetworkProtocol.DroppedTimeout - _timeSinceLastPacket; }
		}

		/// <summary>
		///     Gets a value indicating whether the connection to the remote peer is lagging.
		/// </summary>
		public bool IsLagging
		{
			get { return _timeSinceLastPacket > NetworkProtocol.LaggingTimeout; }
		}

		/// <summary>
		///     Gets a value indicating whether the channel to the remote peer can no longer be used.
		/// </summary>
		public bool IsFaulted
		{
			get
			{
				Assert.NotPooled(this);
				return IsDropped || _channel.IsFaulted;
			}
		}

		/// <summary>
		///     Dispatches all received messages using the given message dispatcher.
		/// </summary>
		/// <param name="handler">The dispatcher that should be used to dispatch the messages.</param>
		public void DispatchReceivedMessages(IMessageHandler handler)
		{
			Assert.ArgumentNotNull(handler);
			CheckAccess();

			var elapsedTime = _clock.Milliseconds;
			_clock.Reset();

			// Cap the time so that we don't disconnect when the debugger is suspending the process
			_timeSinceLastPacket += Math.Min(elapsedTime, 500);

			IncomingUdpPacket packet;
			while (_channel.TryReceive(out packet))
			{
				using (packet)
				using (var reader = packet.Reader)
					HandlePacket(reader);
			}

			foreach (var sequencedMessage in _receivedMessages)
				sequencedMessage.Message.Dispatch(handler, sequencedMessage.SequenceNumber);

			ClearReceivedMessages();
		}

		/// <summary>
		///     Sends the given message to the remote peer.
		/// </summary>
		/// <param name="message">The message that should be sent.</param>
		public void Send(Message message)
		{
			CheckAccess();
			_outgoingMessages.Enqueue(message);
		}

		/// <summary>
		///     Sends all queued messages, checking whether the connection has been dropped.
		/// </summary>
		public void SendQueuedMessages()
		{
			CheckAccess();

			// Send all queued messages to the remote peer
			_outgoingMessages.SendMessages(new PacketAssembler(_allocator, _channel, _deliveryManager.LastReceivedReliableSequenceNumber));

			// Check if the connection has been dropped
			if (TimeToDrop > 0)
				return;

			IsDropped = true;
			throw new ConnectionDroppedException();
		}

		/// <summary>
		///     Closes the connection to the remote peer.
		/// </summary>
		public void Disconnect()
		{
			if (IsFaulted)
				return;

			try
			{
				Send(DisconnectMessage.Create(_allocator));
				SendQueuedMessages();
			}
			catch (NetworkException e)
			{
				Log.Debug("Failed to send disconnect message to sever: {0}", e.Message);
			}

			IsDropped = true;
		}

		/// <summary>
		///     Dispatches the messages contained in the given packet.
		/// </summary>
		/// <param name="buffer">The buffer the packet data should be read from.</param>
		private void HandlePacket(BufferReader buffer)
		{
			uint acknowledgement;
			if (!PacketHeader.TryRead(ref buffer, out acknowledgement))
				return;

			_deliveryManager.UpdateLastAckedSequenceNumber(acknowledgement);

			var readBytes = -1;
			while (!buffer.EndOfBuffer && readBytes != buffer.Count)
			{
				readBytes = buffer.Count;

				SequencedMessage message;
				while (_deserializer.TryDeserialize(ref buffer, out message))
					_receivedMessages.Enqueue(message);
			}

			Assert.That(buffer.EndOfBuffer, "Received an invalid packet from the remote peer.");

			if (!buffer.EndOfBuffer)
				return;

			_timeSinceLastPacket = 0;
		}

		/// <summary>
		///     Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override void OnReturning()
		{
			ClearReceivedMessages();

			_channel.SafeDispose();
			_outgoingMessages.Clear();
			_deserializer.SafeDispose();
			_timeSinceLastPacket = 0;
			IsDropped = false;
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_outgoingMessages.SafeDispose();
		}

		/// <summary>
		///     Clears the queue of received messages.
		/// </summary>
		private void ClearReceivedMessages()
		{
			foreach (var sequencedMessage in _receivedMessages)
				sequencedMessage.Message.SafeDispose();

			_receivedMessages.Clear();
		}

		/// <summary>
		///     In debug builds, checks whether the connection is faulted or has already been disposed.
		/// </summary>
		[Conditional("DEBUG"), DebuggerHidden]
		private void CheckAccess()
		{
			Assert.NotPooled(this);
			Assert.That(!IsFaulted, "The connection is faulted and can no longer be used.");
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate message objects.</param>
		/// <param name="channel">The channel that is used to receive and send data.</param>
		public static Connection Create(PoolAllocator allocator, UdpChannel channel)
		{
			Assert.ArgumentNotNull(channel);
			Assert.ArgumentNotNull(allocator);

			var connection = allocator.Allocate<Connection>();
			connection._allocator = allocator;
			connection._deserializer = MessageDeserializer.Create(allocator, connection._deliveryManager);
			connection._channel = channel;
			connection._clock.Reset();
			connection._deliveryManager.Reset();
			return connection;
		}
	}
}