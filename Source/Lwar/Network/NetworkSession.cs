namespace Lwar.Network
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Messages;
	using Pegasus;
	using Pegasus.Platform.Memory;
	using Pegasus.Platform.Network;
	using Scripting;

	/// <summary>
	///     Represents a network session that implements the lwar network protocol.
	/// </summary>
	public class NetworkSession : DisposableObject
	{
		/// <summary>
		///     The number of disconnect messages that are sent to the server.
		/// </summary>
		private const int DisconnectMessageCount = 3;

		/// <summary>
		///     The time (in milliseconds) to wait between sending two disconnect messages to server.
		/// </summary>
		private const int DisconnectSendInterval = 60;

		/// <summary>
		///     The connection to the remote server.
		/// </summary>
		private readonly ServerConnection _connection;

		/// <summary>
		///     The delivery manager is responsible for the delivery guarantees of all incoming and outgoing messages.
		/// </summary>
		private readonly DeliveryManager _deliveryManager;

		/// <summary>
		///     The message queue is responsible for packing all queued outgoing messages into a packet. Reliable messages will be
		///     resent until their reception has been acknowledged by the remote peer.
		/// </summary>
		private readonly MessageQueue _outgoingMessages;

		/// <summary>
		///     A cached queue that is used to retrieve the received messages from the server connection.
		/// </summary>
		private readonly Queue<Message> _receivedMessages = new Queue<Message>();

		/// <summary>
		///     The cached state of the server connection.
		/// </summary>
		private ConnectionState _connectionState;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="serverEndPoint">The remote end point of the server.</param>
		public NetworkSession(IPEndPoint serverEndPoint)
		{
			_deliveryManager = new DeliveryManager();
			_connection = new ServerConnection(serverEndPoint);
			_connectionState = _connection.State;

			_outgoingMessages = new MessageQueue(_deliveryManager);
			Send(ConnectMessage.Create(Cvars.PlayerName));
		}

		/// <summary>
		///     Gets the endpoint of the server.
		/// </summary>
		public IPEndPoint ServerEndPoint
		{
			get { return _connection.ServerEndPoint; }
		}

		/// <summary>
		///     Gets the remaining time in milliseconds before the network session will be dropped.
		/// </summary>
		public double TimeToDrop
		{
			get { return _connection.TimeToDrop; }
		}

		/// <summary>
		///     Gets a value indicating whether a connection to the server is established.
		/// </summary>
		public bool IsConnected
		{
			get { return _connection.State == ConnectionState.Connected; }
		}

		/// <summary>
		///     Gets a value indicating whether the connection to the server has been established and the game state is currently
		///     being synced.
		/// </summary>
		public bool IsSyncing
		{
			get { return _connection.State == ConnectionState.Syncing; }
		}

		/// <summary>
		///     Gets a value indicating whether the connection to the server is lagging.
		/// </summary>
		public bool IsLagging
		{
			get { return _connection.IsLagging; }
		}

		/// <summary>
		///     Gets a value indicating whether the connection to the server is faulted.
		/// </summary>
		public bool IsFaulted
		{
			get { return _connection.State == ConnectionState.Faulted; }
		}

		/// <summary>
		///     Gets a value indicating whether the connection to the server has been dropped.
		/// </summary>
		public bool IsDropped
		{
			get { return _connection.State == ConnectionState.Dropped; }
		}

		/// <summary>
		///     Gets a value indicating whether the server is full and cannot accept any further clients.
		/// </summary>
		public bool ServerIsFull
		{
			get { return _connection.State == ConnectionState.Full; }
		}

		/// <summary>
		///     Gets a value indicating whether the server implements a newer or older network protocol revision.
		/// </summary>
		public bool VersionMismatch
		{
			get { return _connection.State == ConnectionState.VersionMismatch; }
		}

		/// <summary>
		///     Raises the appropriate state event.
		/// </summary>
		private void HandleConnectionStateChange()
		{
			if (_connectionState == _connection.State)
				return;

			_connectionState = _connection.State;

			switch (_connectionState)
			{
				case ConnectionState.Connecting:
				case ConnectionState.Syncing:
					break;
				case ConnectionState.Connected:
					if (OnConnected != null)
						OnConnected();
					break;
				case ConnectionState.Dropped:
					if (OnDropped != null)
						OnDropped();
					break;
				case ConnectionState.Faulted:
					if (OnFaulted != null)
						OnFaulted();
					break;
				case ConnectionState.Full:
					if (OnRejected != null)
						OnRejected(RejectReason.Full);
					break;
				case ConnectionState.VersionMismatch:
					if (OnRejected != null)
						OnRejected(RejectReason.VersionMismatch);
					break;
				default:
					Assert.NotReached("Unknown connection state.");
					break;
			}
		}

		/// <summary>
		///     Raised when the connection to the server has been successfully established and the game state has been synchronized.
		/// </summary>
		public event Action OnConnected;

		/// <summary>
		///     Raised when a connection error occurred.
		/// </summary>
		public event Action OnFaulted;

		/// <summary>
		///     Raised when the connection was dropped.
		/// </summary>
		public event Action OnDropped;

		/// <summary>
		///     Raised when a connection attempt has been rejected.
		/// </summary>
		public event Action<RejectReason> OnRejected;

		/// <summary>
		///     Updates the state of the network session.
		/// </summary>
		/// <param name="dispatcher">The message dispatcher that should be used to dispatch the received server messages.</param>
		public void Update(MessageDispatcher dispatcher)
		{
			Assert.ArgumentNotNull(dispatcher);

			HandleConnectionStateChange();

			_connection.Send(_outgoingMessages);
			_connection.Receive(_receivedMessages, _deliveryManager);
			_connection.Update();

			while (_receivedMessages.Count != 0)
				dispatcher.Dispatch(_receivedMessages.Dequeue());
		}

		/// <summary>
		///     Sends the given reliable message to the server.
		/// </summary>
		/// <param name="message">The message that should be sent.</param>
		public void Send(Message message)
		{
			_outgoingMessages.Enqueue(ref message);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			// Send a couple of Disconnect messages to the server
			for (var i = 0; i < DisconnectMessageCount && !IsDropped && !IsFaulted; ++i)
			{
				Send(new Message { Type = MessageType.Disconnect });

				_connection.Send(_outgoingMessages);
				_connection.Update();
				Thread.Sleep(DisconnectSendInterval);
			}

			_connection.SafeDispose();
		}
	}
}