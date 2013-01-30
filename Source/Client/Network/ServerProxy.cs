using System;

namespace Lwar.Client.Network
{
	using System.Collections.Generic;
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
		///   The connection state check frequency in Hz. The connection state check determines whether a connection is lagging or
		///   dropped.
		/// </summary>
		public const int ConnectionStateCheckFrequency = 10;

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
		private readonly UdpSocket _socket = new UdpSocket();

		/// <summary>
		///   The time when the last packet has been received from the server.
		/// </summary>
		private double _lastPacketTimestamp;

		/// <summary>
		///   The current state of the virtual connection to the server.
		/// </summary>
		private State _state = State.Disconnected;

		/// <summary>
		///   Provides the time that is used to check whether a connection is lagging or dropped.
		/// </summary>
		private Time _time = new Time();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="serverEndPoint">The endpoint of the server.</param>
		/// <param name="scheduler">The scheduler that should be used to schedule the proxy's internal processes.</param>
		public ServerProxy(IPEndPoint serverEndPoint, ProcessScheduler scheduler)
		{
			Assert.ArgumentNotNull(serverEndPoint, () => serverEndPoint);
			Assert.ArgumentNotNull(scheduler, () => scheduler);

			_messageQueue = new MessageQueue(_deliveryManager);
			_serverEndPoint = serverEndPoint;
			_receiveProcess = scheduler.CreateProcess(Receive);
			_sendProcess = scheduler.CreateProcess(Send);
			_observerProcess = scheduler.CreateProcess(ObserveConnectionState);
		}

		/// <summary>
		///   Gets the remaining time in milliseconds before the connection will be dropped, if the connection is currently
		///   lagging.
		/// </summary>
		public double TimeToDrop { get; private set; }

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
			get { return _state == State.Lagging; }
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

				while (_state != State.Connected && attempts < MaxConnectionAttempts)
				{
					_messageQueue.Enqueue(Messages.Connect.Create());
					++attempts;
					await context.Delay(RetryDelay);
				}

				if (attempts >= MaxConnectionAttempts)
				{
					_state = State.Faulted;
					NetworkLog.ClientError("Failed to connect to {0}. The server did no respond.", _serverEndPoint);
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
		private async Task Send(ProcessContext context)
		{
			while (!context.IsCanceled)
			{
				try
				{
					var packet = _messageQueue.CreatePacket();
					await _socket.SendAsync(context, packet, _serverEndPoint);
				}
				catch (SocketOperationException e)
				{
					// Ignore the error as Udp is connectionless anyway, so there's nothing we can do.
					// We can just hope for the best that the next send will work again, and if not,
					// the connection will eventually time out and the game session will end.
					NetworkLog.ClientDebug("Error while sending: {0}.", e.Message);
				}
			}
		}

		/// <summary>
		///   Handles incoming packets from the server.
		/// </summary>
		/// <param name="context">The context in which the incoming packets should be handled.</param>
		private async Task Receive(ProcessContext context)
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

				try
				{
					using (var packet = await _socket.ReceiveAsync(context, sender))
					{
						if (!sender.Equals(_serverEndPoint))
						{
							NetworkLog.ClientWarn("Received a packet from {0}, but expecting packets from {1} only. Packet was ignored.",
												  sender, _serverEndPoint);
							continue;
						}

						_lastPacketTimestamp = _time.Milliseconds;
						foreach (var message in DeserializeMessages(packet))
						{
							Assert.That(MessageReceived != null, "No one is listening for received messages.");

							MessageReceived(message);
							message.Dispose();
						}
					}
				}
				catch (SocketOperationException e)
				{
					// Ignore the error as Udp is connectionless anyway, so there's nothing we can do.
					// We can just hope for the best that the next receive will work again, and if not,
					// the connection will eventually time out and the game session will end.
					NetworkLog.ClientDebug("Error while receiving: {0}.", e.Message);
				}
			}
		}

		/// <summary>
		///   Deserializes all messages from the incoming packet.
		/// </summary>
		/// <param name="packet">The packet from which the messages should be deserialized.</param>
		private IEnumerable<IMessage> DeserializeMessages(IncomingPacket packet)
		{
			Assert.ArgumentNotNull(packet, () => packet);

			using (packet)
			{
				var buffer = packet.Reader;
				var header = Header.Create(buffer);

				if (header == null)
					yield break;

				var ignoreUnreliableMessages = _deliveryManager.AllowDelivery(header.Value.Timestamp);
				_deliveryManager.UpdateLastAckedSequenceNumber(header.Value.Acknowledgement);

				IReliableMessage reliableMessage = null;
				IUnreliableMessage unreliableMessage = null;

				while (!buffer.EndOfBuffer)
				{
					var type = (MessageType)buffer.ReadByte();
					switch (type)
					{
						case MessageType.AddPlayer:
							reliableMessage = AddPlayer.Create(buffer);

							// If this is the first packet that we receive from the server, it means we're syncing the game state.
							if (reliableMessage.SequenceNumber == 0)
								_state = State.Syncing;
							break;
						case MessageType.RemovePlayer:
							reliableMessage = RemovePlayer.Create(buffer);
							break;
						case MessageType.ChatMessage:
							reliableMessage = ChatMessage.Create(buffer);
							break;
						case MessageType.AddEntity:
							reliableMessage = AddEntity.Create(buffer);
							break;
						case MessageType.RemoveEntity:
							reliableMessage = RemoveEntity.Create(buffer);
							break;
						case MessageType.ChangePlayerState:
							reliableMessage = ChangePlayerState.Create(buffer);
							break;
						case MessageType.ChangePlayerName:
							reliableMessage = ChangePlayerName.Create(buffer);
							break;
						case MessageType.Synced:
							reliableMessage = Synced.Create(buffer);

							if (_state != State.Syncing)
								NetworkLog.ClientWarn("Ignored an unexpected synced message.");
							else
								_state = State.Connected;
							break;
						case MessageType.ServerFull:
							reliableMessage = ServerFull.Create(buffer);

							if (reliableMessage.SequenceNumber != 0)
								NetworkLog.ClientWarn("Ignored an unexpected server full message.");
							else
								_state = State.Full;
							break;
						case MessageType.UpdatePlayerStats:
							unreliableMessage = UpdatePlayerStats.Create(buffer);
							break;
						case MessageType.UpdateEntity:
							unreliableMessage = UpdateEntity.Create(buffer);
							break;
						case MessageType.Connect:
						case MessageType.Disconnect:
						case MessageType.UpdateClientInput:
							NetworkLog.ClientWarn("Received an unexpected message of type {0}. The rest of the packet is ignored.", type);
							yield break;
						default:
							NetworkLog.ClientWarn("Received a message of unknown type. The rest of the packet is ignored.");
							yield break;
					}

					if (reliableMessage == null && unreliableMessage == null)
					{
						NetworkLog.ClientWarn("Received incomplete message of type {0}. Message ignored.", type);
						yield break;
					}

					if (type.IsReliable() && _deliveryManager.AllowDelivery(reliableMessage))
						yield return reliableMessage;

					if (type.IsUnreliable() && !ignoreUnreliableMessages)
					{
						unreliableMessage.Timestamp = header.Value.Timestamp;
						yield return unreliableMessage;
					}
				}
			}
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
				var delta = _time.Milliseconds - _lastPacketTimestamp;

				if (delta > DroppedTimeout)
					_state = State.Dropped;
				else if (delta > LaggingTimeout)
				{
					_state = State.Lagging;
					TimeToDrop = DroppedTimeout - delta;
				}
				else
					_state = State.Connected;

				await context.Delay(1000 / ConnectionStateCheckFrequency);
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
			_socket.SafeDispose();

			if (_state == State.Connected || _state == State.Syncing)
				NetworkLog.ClientInfo("Disconnected from {0}.", _serverEndPoint);
		}

		/// <summary>
		///   Describes the state of the virtual connection to the server.
		/// </summary>
		private enum State
		{
			/// <summary>
			///   Indicates that the connection is not established.
			/// </summary>
			Disconnected,

			/// <summary>
			///   Indicates that a connection attempt has been started.
			/// </summary>
			Connecting,

			/// <summary>
			///   Indicates that a connection is established and the game state is fully synced.
			/// </summary>
			Connected,

			/// <summary>
			///   Indicates that a connection is faulted due to an error and can no longer be used to send and receive any data.
			/// </summary>
			Faulted,

			/// <summary>
			///   Indicates that the server is full and cannot accept any further clients.
			/// </summary>
			Full,

			/// <summary>
			///   Indicates that a connection has been established and that the proxy is waiting for Synced message.
			/// </summary>
			Syncing,

			/// <summary>
			///   Indicates that a connection is lagging, that is, no new packets have been received from the server for a short amount
			///   of time.
			/// </summary>
			Lagging,

			/// <summary>
			///   Indicates that a connection has been dropped after no packets have been received from the server for a specific
			///   amount of time.
			/// </summary>
			Dropped
		}
	}
}