using System;

namespace Lwar.Client.Network
{
	using System.Net;
	using System.Threading.Tasks;
	using Messages;
	using Pegasus.Framework;
	using Pegasus.Framework.Network;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Processes;

	/// <summary>
	///   Represents a proxy of an lwar server that a client can use to communicate with the server.
	/// </summary>
	public class ServerProxy : DisposableObject
	{
		/// <summary>
		///   The maximum number of connection attempts before giving up.
		/// </summary>
		private const int MaxConnectionAttempts = 10;

		/// <summary>
		///   The delay between two connection attempts in milliseconds.
		/// </summary>
		private const int RetryDelay = 100;

		/// <summary>
		///   The default server port.
		/// </summary>
		public const ushort DefaultPort = 32422;

		/// <summary>
		///   The duration in milliseconds that the proxy waits for a new packet from the server before the connection is
		///   considered to be dropped.
		/// </summary>
		public const int DroppedTimeout = 15000;

		/// <summary>
		///   The duration in milliseconds that the proxy waits for a new packet from the server before the connection is
		///   considered to be lagging.
		/// </summary>
		public const int LaggingTimeout = 500;

		/// <summary>
		///   Provides the time that is used to check whether a connection is lagging or dropped.
		/// </summary>
		private readonly Clock _clock = Clock.Create();

		/// <summary>
		///   The delivery manager that is used to enforce the delivery guarantees of all incoming and outgoing messages.
		/// </summary>
		private readonly DeliveryManager _deliveryManager = new DeliveryManager();

		/// <summary>
		///   The message queue that queues outgoing messages and sends them to the server, ensuring that reliable messages are
		///   resent for as long as their reception has not been acknowledged.
		/// </summary>
		private readonly MessageQueue _messageQueue;

		/// <summary>
		///   The process that observes the connection state.
		/// </summary>
		private readonly IProcess _observerProcess;

		/// <summary>
		///   The packet factory that is used to create incoming and outgoing packets.
		/// </summary>
		private readonly IPacketFactory _packetFactory;

		/// <summary>
		///   The process that handles incoming packets from the server.
		/// </summary>
		private readonly IProcess _receiveProcess;

		/// <summary>
		///   The process that sends queued messages to the server.
		/// </summary>
		private readonly IProcess _sendProcess;

		/// <summary>
		///   The endpoint of the server.
		/// </summary>
		private readonly IPEndPoint _serverEndPoint;

		/// <summary>
		///   The Udp socket that is used for the communication with the server.
		/// </summary>
		private readonly UdpSocket _socket;

		/// <summary>
		///   The time when the last packet has been received from the server.
		/// </summary>
		private double _lastPacketTimestamp;

		/// <summary>
		///   The current state of the virtual connection to the server.
		/// </summary>
		private State _state = State.Disconnected;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="packetFactory">The packet factory that should be used to create incoming and outgoing packets.</param>
		/// ///
		/// <param name="serverEndPoint">The endpoint of the server.</param>
		/// <param name="scheduler">The scheduler that should be used to schedule the proxy's internal processes.</param>
		public ServerProxy(IPacketFactory packetFactory, IPEndPoint serverEndPoint, ProcessScheduler scheduler)
		{
			Assert.ArgumentNotNull(packetFactory, () => packetFactory);
			Assert.ArgumentNotNull(serverEndPoint, () => serverEndPoint);
			Assert.ArgumentNotNull(scheduler, () => scheduler);

			_packetFactory = packetFactory;
			_socket = new UdpSocket(_packetFactory);
			_messageQueue = new MessageQueue(packetFactory, _deliveryManager);
			_serverEndPoint = serverEndPoint;
			_receiveProcess = scheduler.CreateProcess(ReceiveAsync);
			_sendProcess = scheduler.CreateProcess(SendAsync);
			_observerProcess = scheduler.CreateProcess(ObserveConnectionState);
		}

		/// <summary>
		///   Gets the remaining time in milliseconds before the connection will be dropped, if the connection is currently
		///   lagging.
		/// </summary>
		public double TimeToDrop
		{
			get { return DroppedTimeout - (_clock.Milliseconds - _lastPacketTimestamp); }
		}

		/// <summary>
		///   Gets a value indicating whether a connection to the server is established.
		/// </summary>
		public bool IsConnected
		{
			get { return _state == State.Connected; }
		}

		/// <summary>
		///   Gets a value indicating whether the connection to the server has been established and the game state is currently
		///   being synced.
		/// </summary>
		public bool IsSyncing
		{
			get { return _state == State.Syncing; }
		}

		/// <summary>
		///   Gets a value indicating whether the connection to the server is lagging.
		/// </summary>
		public bool IsLagging
		{
			get { return _clock.Milliseconds - _lastPacketTimestamp > LaggingTimeout; }
		}

		/// <summary>
		///   Gets a value indicating whether the connection to the server is faulted.
		/// </summary>
		public bool IsFaulted
		{
			get { return _state == State.Faulted; }
		}

		/// <summary>
		///   Gets a value indicating whether the connection to the server has been dropped.
		/// </summary>
		public bool IsDropped
		{
			get { return _state == State.Dropped; }
		}

		/// <summary>
		///   Gets a value indicating whether the server is full and cannot accept any further clients.
		/// </summary>
		public bool ServerIsFull
		{
			get { return _state == State.Full; }
		}

		/// <summary>
		///   Raised when a message has been received from the server.
		/// </summary>
		public event Action<IMessage> MessageReceived;

		/// <summary>
		///   Sends a Connect message to the server.
		/// </summary>
		/// <param name="context">The context in which the connection should be established.</param>
		public async Task Connect(ProcessContext context)
		{
			Assert.That(_state == State.Disconnected, "The proxy is not disconnected.");
			_state = State.Connecting;

			try
			{
				var attempts = 0;
				NetworkLog.ClientInfo("Connecting to {0}.", _serverEndPoint);

				_messageQueue.Enqueue(ConnectMessage.Create());
				while (_state == State.Connecting && attempts < MaxConnectionAttempts)
				{
					_socket.Send(_messageQueue.CreatePacket(), _serverEndPoint);

					++attempts;
					await context.Delay(RetryDelay);
				}

				if (attempts >= MaxConnectionAttempts)
				{
					_state = State.Faulted;
					NetworkLog.ClientError("Failed to connect to {0}. The server did not respond.", _serverEndPoint);
				}
				else
					NetworkLog.ClientInfo("Connected to {0}.", _serverEndPoint);
			}
			catch (SocketOperationException e)
			{
				_state = State.Faulted;
				NetworkLog.ClientError("Unable to establish a connection to the server: {0}", e.Message);
			}
		}

		/// <summary>
		///   Sends the queued messages to the server.
		/// </summary>
		/// <param name="context">The context in which the queued messages should be sent.</param>
		private async Task SendAsync(ProcessContext context)
		{
			try
			{
				while (!context.IsCanceled && _state < State.Syncing)
					await context.NextFrame();

				while (!context.IsCanceled && _state == State.Syncing)
				{
					_socket.Send(_messageQueue.CreatePacket(), _serverEndPoint);
					await context.NextFrame();
				}

				while (!context.IsCanceled)
				{
					if (_messageQueue.HasPendingData)
						_socket.Send(_messageQueue.CreatePacket(), _serverEndPoint);

					await context.NextFrame();
				}
			}
			catch (SocketOperationException e)
			{
				NetworkLog.ClientError("The connection to the server has been terminated due to an error: {0}", e.Message);
				_state = State.Faulted;
			}
		}

		/// <summary>
		///   Handles incoming packets from the server.
		/// </summary>
		/// <param name="context">The context in which the incoming packets should be handled.</param>
		private async Task ReceiveAsync(ProcessContext context)
		{
			try
			{
				var sender = new IPEndPoint(IPAddress.Any, 0);

				while (!context.IsCanceled)
				{
					if (_state == State.Faulted)
						break;

					if (_state == State.Disconnected)
					{
						await context.NextFrame();
						continue;
					}

					IncomingPacket packet;
					while (_socket.TryReceive(ref sender, out packet))
					{
						using (packet)
						{
							// TODO: Check server address
							if (sender.Port != _serverEndPoint.Port)
							{
								NetworkLog.ClientWarn("Received a packet from {0}, but expecting packets from {1} only. Packet was ignored.",
													  sender, _serverEndPoint);
								continue;
							}

							HandleMessages(packet);
						}
					}

					await context.NextFrame();
				}
			}
			catch (SocketOperationException e)
			{
				NetworkLog.ClientError("The connection to the server has been terminated due to an error: {0}", e.Message);
				_state = State.Faulted;
			}
		}

		/// <summary>
		///   Handles all messages in the incoming packet.
		/// </summary>
		/// <param name="packet">The packet from which the messages should be deserialized.</param>
		private void HandleMessages(IncomingPacket packet)
		{
			Assert.ArgumentNotNull(packet, () => packet);
			Assert.That(MessageReceived != null, "No one is listening for received messages.");

			var buffer = packet.Reader;
			var header = Header.Create(buffer);

			if (header == null)
				return;

			_lastPacketTimestamp = _clock.Milliseconds;
			_deliveryManager.UpdateLastAckedSequenceNumber(header.Value.Acknowledgement);
			var allowUnreliableDelivery = _deliveryManager.AllowUnreliableDelivery(header.Value.Timestamp);

			while (!buffer.EndOfBuffer)
			{
				var type = (MessageType)buffer.ReadByte();
				IMessage message = null;

				if (type.IsReliable())
					message = HandleReliableMessage(buffer, type);
				else if (type.IsUnreliable())
					message = HandleUnreliableMessage(buffer, type, allowUnreliableDelivery);
				else
					Assert.That(false, "Unclassified message type.");

				// Only raise the event if we actually have a valid message to handle
				if (message != null)
					using (message)
						MessageReceived(message);
			}
		}

		/// <summary>
		///   Deserializes a reliable message of the given type from the buffer, returning null if the message should
		///   be ignored.
		/// </summary>
		/// <param name="buffer">The buffer the message should be deserialized from.</param>
		/// <param name="type">The type of the reliable message.</param>
		private IReliableMessage HandleReliableMessage(BufferReader buffer, MessageType type)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			Assert.ArgumentInRange(type, () => type);
			Assert.ArgumentSatisfies(type.IsReliable(), () => type, "Not a reliable message type.");

			if (!buffer.CanRead(sizeof(uint)))
				return null;

			IReliableMessage message;

			var sequenceNumber = buffer.ReadUInt32();
			var allowDelivery = _deliveryManager.AllowReliableDelivery(sequenceNumber);

			switch (type)
			{
				case MessageType.Join:
					message = JoinMessage.Create(buffer);

					// If this is the first packet that we received from the server, it means we're syncing the game state
					if (allowDelivery && sequenceNumber == 1)
					{
						((JoinMessage)message).IsLocalPlayer = true;
						_state = State.Syncing;
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
					if (allowDelivery && _state != State.Syncing)
						NetworkLog.ClientWarn("Ignored an unexpected synced message.");
					else if (allowDelivery)
						_state = State.Connected;
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
					message = UpdateMessage.Create(buffer);
					break;
				case MessageType.Full:
					message = FullMessage.Create(buffer);

					// Only the first message can be a server full message
					if (allowDelivery && _state != State.Connecting)
						NetworkLog.ClientWarn("Ignored an unexpected server full message.");
					else if (allowDelivery)
						_state = State.Full;
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
		///   Observes the connection state, setting the state to Lagging if no packet has been received for a short amount of time
		///   and eventually to Dropped if no more packets are received. If another packet is received before the Dropped timeout
		///   occurs, the connection state is reset to Connected.
		/// </summary>
		/// <param name="context">The context in which the connection state should be observed.</param>
		private async Task ObserveConnectionState(ProcessContext context)
		{
			while (!context.IsCanceled)
			{
				if (_state != State.Syncing && _state != State.Connected)
				{
					await context.NextFrame();
					continue;
				}

				var delta = _clock.Milliseconds - _lastPacketTimestamp;
				if (delta > DroppedTimeout)
					_state = State.Dropped;

				await context.NextFrame();
			}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_sendProcess.SafeDispose();
			_receiveProcess.SafeDispose();
			_observerProcess.SafeDispose();
			_messageQueue.SafeDispose();
			_deliveryManager.SafeDispose();
			_clock.SafeDispose();
			_socket.SafeDispose();

			if (_state == State.Connected || _state == State.Syncing)
				NetworkLog.ClientInfo("Disconnected from {0}.", _serverEndPoint);
		}

		/// <summary>
		///   Sends the given unreliable message to the server.
		/// </summary>
		/// <param name="message">The message that should be sent.</param>
		public void Send(IUnreliableMessage message)
		{
			_messageQueue.Enqueue(message);
		}

		/// <summary>
		///   Sends the given reliable message to the server.
		/// </summary>
		/// <param name="message">The message that should be sent.</param>
		public void Send(IReliableMessage message)
		{
			_messageQueue.Enqueue(message);
		}

		/// <summary>
		///   Describes the state of the virtual connection to the server.
		/// </summary>
		private enum State
		{
			/// <summary>
			///   Indicates that the connection is not established.
			/// </summary>
			Disconnected = 0,

			/// <summary>
			///   Indicates that a connection attempt has been started.
			/// </summary>
			Connecting = 1,

			/// <summary>
			///   Indicates that a connection has been established and that the proxy is waiting for Synced message.
			/// </summary>
			Syncing = 2,

			/// <summary>
			///   Indicates that a connection is established and the game state is fully synced.
			/// </summary>
			Connected = 3,

			/// <summary>
			///   Indicates that a connection is faulted due to an error and can no longer be used to send and receive any data.
			/// </summary>
			Faulted,

			/// <summary>
			///   Indicates that the server is full and cannot accept any further clients.
			/// </summary>
			Full,

			/// <summary>
			///   Indicates that a connection has been dropped after no packets have been received from the server for a specific
			///   amount of time.
			/// </summary>
			Dropped
		}
	}
}