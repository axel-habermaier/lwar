namespace Lwar.Gameplay.Server
{
	using System;
	using Network;
	using Network.Messages;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a connection to a client.
	/// </summary>
	public partial class Client : UniquePooledObject, IMessageHandler
	{
		/// <summary>
		///     The allocator that is used to allocate game objects.
		/// </summary>
		private PoolAllocator _allocator;

		/// <summary>
		///     The connection to the client.
		/// </summary>
		private Connection _connection;

		/// <summary>
		///     The client's frame number the last received input was generated in.
		/// </summary>
		private uint _lastInputFrame;

		/// <summary>
		///     The player that represents the client within the game session.
		/// </summary>
		private Player _player;

		/// <summary>
		///     The server logic that handles the communication between the server and the clients.
		/// </summary>
		private ServerLogic _serverLogic;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Client()
		{
			ConstructorCache.Register(() => new Client());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private Client()
		{
		}

		/// <summary>
		///     Gets a value indicating whether the client is disconnected and can be removed from the server's list of active clients.
		/// </summary>
		public bool IsDisconnected
		{
			get
			{
				Assert.NotPooled(this);
				return _connection.IsFaulted;
			}
		}

		/// <summary>
		///     Gets a value indicating whether the client has been synced and can receive broadcast messages.
		/// </summary>
		public bool IsSynced { get; private set; }

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void IMessageHandler.OnConnect(ClientConnectMessage message)
		{
			Assert.IsNull(_player, "The client is already connected.");
			Assert.NotPooled(this);

			if (NetworkProtocol.Revision != message.NetworkRevision)
			{
				_connection.Send(ClientRejectedMessage.Create(_allocator, RejectReason.VersionMismatch));
				_connection.Disconnect();
			}
			else if (_serverLogic.PlayerCount >= NetworkProtocol.MaxPlayers)
			{
				_connection.Send(ClientRejectedMessage.Create(_allocator, RejectReason.Full));
				_connection.Disconnect();
			}
			else
			{
				_player = _serverLogic.CreatePlayer(message.PlayerName);
				_serverLogic.SendStateSnapshot(_connection, _player);
				IsSynced = true;
			}
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void IMessageHandler.OnDisconnect(DisconnectMessage message)
		{
			Assert.NotPooled(this);

			// We always accept disconnect messages, as they might arrive at any time, 
			// even after we've already removed the client (in which case we re-added
			// the client just to remove it again).
			if (_player != null)
				_player.LeaveReason = LeaveReason.Disconnect;

			_connection.Disconnect();
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void IMessageHandler.OnPlayerInput(PlayerInputMessage message)
		{
			Assert.NotPooled(this);

			if (!IsClientConnected(message) || !IsClientPlayer(message, message.Player))
				return;

			if (message.FrameNumber <= _lastInputFrame)
				return;

			var inputMask = (byte)(~(0xff << (int)(message.FrameNumber - _lastInputFrame)));
			_lastInputFrame = message.FrameNumber;

			_serverLogic.HandlePlayerInput(_player, message, inputMask);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void IMessageHandler.OnPlayerChatMessage(PlayerChatMessage message)
		{
			Assert.NotPooled(this);

			if (!IsClientConnected(message) || !IsClientPlayer(message, message.Player))
				return;

			_serverLogic.Chat(_player, message.Message);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void IMessageHandler.OnPlayerName(PlayerNameMessage message)
		{
			Assert.NotPooled(this);

			if (!IsClientConnected(message) || !IsClientPlayer(message, message.Player))
				return;

			_serverLogic.ChangePlayerName(_player, message.PlayerName);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		void IMessageHandler.OnPlayerSelection(PlayerLoadoutMessage message)
		{
			Assert.NotPooled(this);

			if (!IsClientConnected(message) || !IsClientPlayer(message, message.Player))
				return;

			_serverLogic.ChangeLoadout(_player, message);
		}

		/// <summary>
		///     Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override void OnReturning()
		{
			if (_player != null)
			{
				if (_player.LeaveReason == LeaveReason.Unknown)
					_player.LeaveReason = LeaveReason.ConnectionDropped;

				_serverLogic.RemovePlayer(_player);
			}

			_lastInputFrame = 0;
			_connection.SafeDispose();
			IsSynced = false;
		}

		/// <summary>
		///     Dispatches all messages received from the client.
		/// </summary>
		public void DispatchReceivedMessages()
		{
			Assert.NotPooled(this);

			if (!IsDisconnected)
				_connection.DispatchReceivedMessages(this);
		}

		/// <summary>
		///     Sends the given message to client.
		/// </summary>
		/// <param name="message">The message that should be sent.</param>
		public void Send(Message message)
		{
			Assert.NotPooled(this);

			if (!IsDisconnected)
				_connection.Send(message);
		}

		/// <summary>
		///     Sends all queued messages to the client.
		/// </summary>
		public void SendQueuedMessages()
		{
			Assert.NotPooled(this);

			if (!IsDisconnected)
				_connection.SendQueuedMessages();
		}

		/// <summary>
		///     Disconnects the connection to the client.
		/// </summary>
		public void Disconnect()
		{
			Assert.NotPooled(this);

			if (!IsDisconnected)
				_connection.Disconnect();
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate game objects.</param>
		/// <param name="connection">The connection to the client.</param>
		/// <param name="serverLogic">The server logic that handles the communication between the server and the clients.</param>
		public static Client Create(PoolAllocator allocator, Connection connection, ServerLogic serverLogic)
		{
			Assert.ArgumentNotNull(allocator);
			Assert.ArgumentNotNull(serverLogic);
			Assert.ArgumentNotNull(connection);

			var handler = allocator.Allocate<Client>();
			handler._allocator = allocator;
			handler._serverLogic = serverLogic;
			handler._connection = connection;
			handler._player = null;
			return handler;
		}

		/// <summary>
		///     Returns true if the client is fully connected to the game session. Otherwise, returns false and kicks the player because
		///     of this protocol misbehavior.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		private bool IsClientConnected(Message message)
		{
			if (_player != null && ! _connection.IsFaulted)
				return true;

			Log.Error("Received an unexpected message of type '{0}' from client at '{1}'.", message.MessageType, _connection.RemoteEndPoint);

			DisconnectAfterMisbehavior();
			return false;
		}

		/// <summary>
		///     Returns true if the given player matches the client's player. Otherwise, returns false and kicks the player because of
		///     this protocol misbehavior.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		/// <param name="player">The player that was referenced in the message.</param>
		private bool IsClientPlayer(Message message, NetworkIdentity player)
		{
			if (_player != null && player == _player.Identity)
				return true;

			Log.Error("Client '{0}' has been kicked: Client message of type '{3}' references invalid player {1} instead of client's player {2}.",
				_connection.RemoteEndPoint, player.Identifier, _player.Identity, message.MessageType);

			DisconnectAfterMisbehavior();
			return false;
		}

		/// <summary>
		///     Disconnects the client after a misbehavior.
		/// </summary>
		private void DisconnectAfterMisbehavior()
		{
			if (_player != null)
			{
				_player.LeaveReason = LeaveReason.Misbehaved;
				_connection.Send(PlayerLeaveMessage.Create(_allocator, _player.Identity, LeaveReason.Misbehaved));
			}

			Disconnect();
		}
	}
}