using System;

namespace Lwar.Client.Network
{
	using System.Collections.Generic;
	using System.Net;
	using Messages;
	using Pegasus.Framework;
	using Pegasus.Framework.Network;
	using Pegasus.Framework.Platform;

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
		/// <param name="packetFactory">The packet factory that should be used to create incoming packets.</param>
		public ServerConnection(IPEndPoint serverEndPoint, IPacketFactory packetFactory)
		{
			Assert.ArgumentNotNull(serverEndPoint, () => serverEndPoint);
			Assert.ArgumentNotNull(packetFactory, () => packetFactory);

			_socket = new UdpSocket(packetFactory);
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
					_socket.Send(messageQueue.CreatePacket(), ServerEndPoint);

				// Once we're connected, only send a packet if we actually have some payload to send
				if (State == ConnectionState.Connected && messageQueue.HasPendingData)
					_socket.Send(messageQueue.CreatePacket(), ServerEndPoint);
			}
			catch (SocketOperationException e)
			{
				NetworkLog.ClientError("The connection to the server has been terminated due to an error: {0}", e.Message);
				State = ConnectionState.Faulted;
			}
		}

		/// <summary>
		///   Handles incoming packets from the server.
		/// </summary>
		/// <param name="messageQueue">The message queue the received messages should be added to.</param>
		/// <param name="deliveryManager">
		///   The delivery manager that is used to determine whether a message should be added to the queue.
		/// </param>
		public void Receive(Queue<IMessage> messageQueue, DeliveryManager deliveryManager)
		{
			Assert.ArgumentNotNull(messageQueue, () => messageQueue);
			Assert.ArgumentNotNull(deliveryManager, () => deliveryManager);

			if (State != ConnectionState.Connecting && State != ConnectionState.Connected && State != ConnectionState.Syncing)
				return;

			try
			{
				var sender = new IPEndPoint(IPAddress.Any, 0);
				IncomingPacket packet;

				while (_socket.TryReceive(ref sender, out packet))
				{
					using (packet)
					{
						if (sender.SameEndPoint(ServerEndPoint))
							HandleMessages(packet, messageQueue, deliveryManager);
						else
						{
							NetworkLog.ClientWarn("Received a packet from {0}, but expecting packets from {1} only. Packet was ignored.",
												  sender, ServerEndPoint);
						}
					}
				}
			}
			catch (SocketOperationException e)
			{
				NetworkLog.ClientError("The connection to the server has been terminated due to an error: {0}", e.Message);
				State = ConnectionState.Faulted;
			}
		}

		/// <summary>
		///   Handles all messages in the incoming packet.
		/// </summary>
		/// <param name="packet">The packet from which the messages should be deserialized.</param>
		/// <param name="messageQueue">The message queue the received messages should be added to.</param>
		/// <param name="deliveryManager">
		///   The delivery manager that is used to determine whether a message should be added to the queue.
		/// </param>
		private void HandleMessages(IncomingPacket packet, Queue<IMessage> messageQueue, DeliveryManager deliveryManager)
		{
			Assert.ArgumentNotNull(packet, () => packet);

			var buffer = packet.Reader;
			var header = Header.Create(buffer);

			if (header == null)
				return;

			_lastPacketTimestamp = _clock.Milliseconds;
			deliveryManager.UpdateLastAckedSequenceNumber(header.Value.Acknowledgement);
			var allowUnreliableDelivery = deliveryManager.AllowUnreliableDelivery(header.Value.Timestamp);

			while (!buffer.EndOfBuffer)
			{
				var type = (MessageType)buffer.ReadByte();
				IMessage message = null;

				if (type.IsReliable())
					message = HandleReliableMessage(buffer, type, deliveryManager);
				else if (type.IsUnreliable())
					message = HandleUnreliableMessage(buffer, type, allowUnreliableDelivery);
				else
					Assert.That(false, "Unclassified message type.");

				// Only enqueue the message if we actually have a valid message to handle
				if (message != null)
					messageQueue.Enqueue(message);
			}
		}

		/// <summary>
		///   Deserializes a reliable message of the given type from the buffer, returning null if the message should
		///   be ignored.
		/// </summary>
		/// <param name="buffer">The buffer the message should be deserialized from.</param>
		/// <param name="type">The type of the reliable message.</param>
		/// <param name="deliveryManager">
		///   The delivery manager that is used to determine whether a message should be added to the queue.
		/// </param>
		private IReliableMessage HandleReliableMessage(BufferReader buffer, MessageType type, DeliveryManager deliveryManager)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			Assert.ArgumentInRange(type, () => type);
			Assert.ArgumentSatisfies(type.IsReliable(), () => type, "Not a reliable message type.");

			if (!buffer.CanRead(sizeof(uint)))
				return null;

			IReliableMessage message;

			var sequenceNumber = buffer.ReadUInt32();
			var allowDelivery = deliveryManager.AllowReliableDelivery(sequenceNumber);

			switch (type)
			{
				case MessageType.Join:
					message = JoinMessage.Create(buffer);

					// If this is the first packet that we received from the server, it means we're syncing the game state
					if (allowDelivery && sequenceNumber == 1)
					{
						((JoinMessage)message).IsLocalPlayer = true;
						State = ConnectionState.Syncing;
					}
					break;
				case MessageType.Leave:
					message = LeaveMessage.Create(buffer);
					break;
				case MessageType.Chat:
					message = ChatMessage.Create(buffer);
					break;
				case MessageType.Add:
					message = AddMessage.Create(buffer);
					break;
				case MessageType.Remove:
					message = RemoveMessage.Create(buffer);
					break;
				case MessageType.Selection:
					message = SelectionMessage.Create(buffer);
					break;
				case MessageType.Name:
					message = NameMessage.Create(buffer);
					break;
				case MessageType.Synced:
					message = SyncedMessage.Create(buffer);

					// We should only receive a sync packet if we're actually syncing
					if (allowDelivery && State != ConnectionState.Syncing)
						NetworkLog.ClientWarn("Ignored an unexpected synced message.");
					else if (allowDelivery)
						State = ConnectionState.Connected;

					allowDelivery = false;
					break;
				case MessageType.Connect:
				case MessageType.Disconnect:
					NetworkLog.ClientWarn("Received an unexpected reliable message of type {0}. The rest of the packet is ignored.", type);
					return null;
				default:
					NetworkLog.ClientWarn("Received a message of unknown reliable type. The rest of the packet is ignored.");
					return null;
			}

			if (!allowDelivery)
			{
				message.SafeDispose();
				return null;
			}

			message.SequenceNumber = sequenceNumber;
			return message;
		}

		/// <summary>
		///   Deserializes a reliable message of the given type from the buffer
		/// </summary>
		/// <param name="buffer">The buffer the message should be deserialized from.</param>
		/// <param name="type">The type of the unreliable message.</param>
		/// <param name="allowDelivery">Indicates whether the message is allowed to be delivered.</param>
		private IUnreliableMessage HandleUnreliableMessage(BufferReader buffer, MessageType type, bool allowDelivery)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			Assert.ArgumentInRange(type, () => type);
			Assert.ArgumentSatisfies(type.IsUnreliable(), () => type, "Not an unreliable message type.");

			IUnreliableMessage message;
			switch (type)
			{
				case MessageType.Stats:
					message = StatsMessage.Create(buffer);
					break;
				case MessageType.Update:
					message = UpdateMessage.Create(buffer, UpdateRecordType.Full);
					break;
				case MessageType.UpdatePosition:
					message = UpdateMessage.Create(buffer, UpdateRecordType.Position);
					break;
				case MessageType.UpdateRay:
					message = UpdateMessage.Create(buffer, UpdateRecordType.Ray);
					break;
				case MessageType.UpdateCircle:
					message = UpdateMessage.Create(buffer, UpdateRecordType.Circle);
					break;
				case MessageType.Full:
					message = FullMessage.Create(buffer);

					// Only the first message can be a server full message
					if (allowDelivery && State != ConnectionState.Connecting)
						NetworkLog.ClientWarn("Ignored an unexpected server full message.");
					else if (allowDelivery)
						State = ConnectionState.Full;

					allowDelivery = false;
					break;
				case MessageType.Collision:
					message = CollisionMessage.Create(buffer);
					break;
				case MessageType.Input:
					NetworkLog.ClientWarn("Received an unexpected unreliable message of type {0}. The rest of the packet is ignored.", type);
					return null;
				default:
					NetworkLog.ClientWarn("Received an unreliable message of unknown type. The rest of the packet is ignored.");
					return null;
			}

			if (!allowDelivery)
			{
				message.SafeDispose();
				return null;
			}

			return message;
		}

		/// <summary>
		///   Updates the connection state, dropping the connection if no more packets are received for a specific amount of time.
		/// </summary>
		public void Update()
		{
			if (State != ConnectionState.Syncing && State != ConnectionState.Connected)
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

			if (State == ConnectionState.Connected || State == ConnectionState.Syncing)
				NetworkLog.ClientInfo("Disconnected from {0}.", ServerEndPoint);
		}
	}
}