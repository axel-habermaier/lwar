using System;

namespace Lwar.Network
{
	using System.Collections.Generic;
	using System.Net;
	using Messages;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Logging;
	using Pegasus.Framework.Platform.Memory;
	using Pegasus.Framework.Platform.Network;

	/// <summary>
	///   Represents a proxy of an lwar server that a client can use to communicate with the server.
	/// </summary>
	public class ServerConnection : DisposableObject
	{
		/// <summary>
		///   The duration in milliseconds that the proxy waits for a new packet from the server before the connection is
		///   considered to be dropped.
		/// </summary>
		private const int DroppedTimeout = 15000;

		/// <summary>
		///   The duration in milliseconds that the proxy waits for a new packet from the server before the connection is
		///   considered to be lagging.
		/// </summary>
		private const int LaggingTimeout = 500;

		/// <summary>
		///   Cached delegate of the message deserialization function.
		/// </summary>
		private static readonly Func<BufferReader, List<Message>> MessageDeserializer = MessageSerialization.Deserialize;

		/// <summary>
		///   The buffer that is used to send and receive data.
		/// </summary>
		private readonly byte[] _buffer = new byte[Specification.MaxPacketSize];

		/// <summary>
		///   Provides the time that is used to check whether a connection is lagging or dropped.
		/// </summary>
		private readonly Clock _clock = Clock.Create();

		/// <summary>
		///   The Udp socket that is used for the communication with the server.
		/// </summary>
		private readonly UdpSocket _socket;

		/// <summary>
		///   The time when the last packet has been received from the server.
		/// </summary>
		private double _lastPacketTimestamp;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="serverEndPoint">The endpoint of the server.</param>
		public ServerConnection(IPEndPoint serverEndPoint)
		{
			Assert.ArgumentNotNull(serverEndPoint);

			_socket = new UdpSocket();
			ServerEndPoint = serverEndPoint;
			State = ConnectionState.Connecting;
		}

		/// <summary>
		///   Gets the current state of the virtual connection to the server.
		/// </summary>
		public ConnectionState State { get; private set; }

		/// <summary>
		///   Gets the endpoint of the server.
		/// </summary>
		public IPEndPoint ServerEndPoint { get; private set; }

		/// <summary>
		///   Gets the remaining time in milliseconds before the connection will be dropped.
		/// </summary>
		public double TimeToDrop
		{
			get { return DroppedTimeout - (_clock.Milliseconds - _lastPacketTimestamp); }
		}

		/// <summary>
		///   Gets a value indicating whether the connection to the server is lagging.
		/// </summary>
		public bool IsLagging
		{
			get { return _clock.Milliseconds - _lastPacketTimestamp > LaggingTimeout; }
		}

		/// <summary>
		///   Sends the queued messages to the server.
		/// </summary>
		/// <param name="messageQueue">The queued messages that should be sent.</param>
		public void Send(MessageQueue messageQueue)
		{
			try
			{
				// While we're connecting or syncing, send as many packets as possible (even if the payload is empty)
				if (State == ConnectionState.Connecting || State == ConnectionState.Syncing)
					SendMessages(messageQueue);

				// Once we're connected, only send a packet if we actually have some payload to send
				if (State == ConnectionState.Connected && messageQueue.HasPendingData)
					SendMessages(messageQueue);
			}
			catch (SocketOperationException e)
			{
				Log.Error("The connection to the server has been terminated due to an error: {0}", e.Message);
				State = ConnectionState.Faulted;
			}
		}

		/// <summary>
		///   Sends the messages that are currently enqueued.
		/// </summary>
		/// <param name="messageQueue">The message queue that stores the messages that should be sent.</param>
		private void SendMessages(MessageQueue messageQueue)
		{
			var size = messageQueue.WritePacket(_buffer);
			_socket.Send(_buffer, size, ServerEndPoint);
		}

		/// <summary>
		///   Handles incoming packets from the server.
		/// </summary>
		/// <param name="messageQueue">The message queue the received messages should be added to.</param>
		/// <param name="deliveryManager">
		///   The delivery manager that is used to determine whether a message should be added to the queue.
		/// </param>
		public void Receive(Queue<Message> messageQueue, DeliveryManager deliveryManager)
		{
			Assert.ArgumentNotNull(messageQueue);
			Assert.ArgumentNotNull(deliveryManager);

			if (State != ConnectionState.Connecting && State != ConnectionState.Connected && State != ConnectionState.Syncing)
				return;

			try
			{
				var sender = new IPEndPoint(IPAddress.IPv6Any, 0);
				int receivedBytes;

				while (_socket.TryReceive(_buffer, ref sender, out receivedBytes))
				{
					using (var reader = BufferReader.Create(_buffer, 0, receivedBytes, Endianess.Big))
					{
						if (sender.SameEndPoint(ServerEndPoint))
							HandlePacket(reader, messageQueue, deliveryManager);
						else
						{
							Log.Warn("Received a packet from {0}, but expecting packets from {1} only. Packet was ignored.",
									 sender, ServerEndPoint);
						}
					}

					if (State != ConnectionState.Connecting && State != ConnectionState.Connected && State != ConnectionState.Syncing)
						return;
				}
			}
			catch (SocketOperationException e)
			{
				Log.Error("The connection to the server has been terminated due to an error: {0}", e.Message);
				State = ConnectionState.Faulted;
			}
		}

		/// <summary>
		///   Handles the incoming packet.
		/// </summary>
		/// <param name="buffer">The buffer the packet data should be read from.</param>
		/// <param name="messageQueue">The message queue the received messages should be added to.</param>
		/// <param name="deliveryManager">
		///   The delivery manager that is used to determine whether a message should be added to the queue.
		/// </param>
		private void HandlePacket(BufferReader buffer, Queue<Message> messageQueue, DeliveryManager deliveryManager)
		{
			Assert.ArgumentNotNull(buffer);

			var header = PacketHeader.Create(buffer);
			if (header == null)
				return;

			deliveryManager.UpdateLastAckedSequenceNumber(header.Value.Acknowledgement);

			var readBytes = -1;
			while (!buffer.EndOfBuffer && readBytes != buffer.Count)
			{
				readBytes = buffer.Count;

				List<Message> messages;
				if (!buffer.TryRead(out messages, MessageDeserializer))
					continue;

				// There are two cases here:
				// - messages.Count == 1: This happens when a single protocol message results in a single Message instance
				// - messages.Count  > 1: This happens when a single protocol message is split into several Message instances
				// In either case, we can check if delivery is allowed for all generated Message instances, as they all
				// have the same sequence number and type.
				var message = messages[0];
				if (message.Type.IsReliable() && !deliveryManager.AllowReliableDelivery(message.SequenceNumber))
					continue;

				if (message.Type.IsUnreliable() && !deliveryManager.AllowUnreliableDelivery(message.SequenceNumber))
					continue;

				for (var i = 0; i < messages.Count; ++i)
				{
					message = messages[i];

					if (message.Type.IsUnreliable())
						message.Timestamp = header.Value.Timestamp;

					HandleMessage(ref message, messageQueue);
				}
			}

			Assert.That(buffer.EndOfBuffer, "Received an invalid packet from the server.");

			if (buffer.EndOfBuffer)
				_lastPacketTimestamp = _clock.Milliseconds;
		}

		/// <summary>
		///   Handles internal messages and adds external messages to the queue.
		/// </summary>
		/// <param name="message">The message that should be handled.</param>
		/// <param name="messageQueue">The message queue an external message should be added to.</param>
		private void HandleMessage(ref Message message, Queue<Message> messageQueue)
		{
			switch (message.Type)
			{
				case MessageType.Join:
					// If the first message we get from the server is a join message, we know that the server has
					// accepted the connection and starts syncing the game state.
					// The first join sent by the server is the join for the local player.
					if (message.SequenceNumber == 1)
					{
						message.Join.IsLocalPlayer = true;
						State = ConnectionState.Syncing;
					}
					messageQueue.Enqueue(message);
					break;
				case MessageType.Synced:
					// We should only receive a sync packet if we're actually syncing
					if (State != ConnectionState.Syncing)
						Log.Warn("Ignored an unexpected Synced message.");
					else
						State = ConnectionState.Connected;
					break;
				case MessageType.Reject:
					// Only the first message can be a reject message
					if (State != ConnectionState.Connecting)
						Log.Warn("Ignored an unexpected reject message.");
					else
					{
						switch (message.Reject)
						{
							case RejectReason.Full:
								State = ConnectionState.Full;
								break;
							case RejectReason.VersionMismatch:
								State = ConnectionState.VersionMismatch;
								break;
							default:
								throw new InvalidOperationException("Unknown reject reason.");
						}
					}
					break;
				default:
					messageQueue.Enqueue(message);
					break;
			}
		}

		/// <summary>
		///   Updates the connection state, dropping the connection if no more packets are received for a specific amount of time.
		/// </summary>
		public void Update()
		{
			if (State != ConnectionState.Connecting && State != ConnectionState.Syncing && State != ConnectionState.Connected)
				return;

			var delta = _clock.Milliseconds - _lastPacketTimestamp;
			if (delta > DroppedTimeout)
				State = ConnectionState.Dropped;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_clock.SafeDispose();
			_socket.SafeDispose();
		}
	}
}