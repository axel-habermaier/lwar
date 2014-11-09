namespace Lwar.Gameplay.Server
{
	using System;
	using Entities;
	using Network;
	using Network.Messages;
	using Pegasus.Math;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;
	using Templates;

	/// <summary>
	///     Implements the server logic for handling incoming client messages and the synchronization of client game states.
	/// </summary>
	public class ServerLogic
	{
		/// <summary>
		///     If tracing is enabled, all server-specific gameplay events are shown in the debug output.
		/// </summary>
		private const bool EnableTracing = false;

		/// <summary>
		///     The allocator that is used to allocate pooled objects.
		/// </summary>
		private readonly PoolAllocator _allocator;

		/// <summary>
		///     A cached array that indicates which weapons a ship should fire.
		/// </summary>
		private readonly bool[] _fireWeapons = new bool[NetworkProtocol.WeaponSlotCount];

		/// <summary>
		///     The game session that is being played.
		/// </summary>
		private readonly GameSession _gameSession;

		/// <summary>
		///     A cached array that stores the weapon energy levels of ship updates.
		/// </summary>
		// TODO: Remove this
		private readonly int[] _weaponEnergyLevels = new int[NetworkProtocol.WeaponSlotCount];

		/// <summary>
		///     The allocator for networked identities.
		/// </summary>
		private NetworkIdentityAllocator _networkIdentities = new NetworkIdentityAllocator(UInt16.MaxValue);

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate pooled objects.</param>
		/// <param name="gameSession">The game session that is being played.</param>
		public ServerLogic(PoolAllocator allocator, GameSession gameSession)
		{
			Assert.ArgumentNotNull(allocator);
			Assert.ArgumentNotNull(gameSession);

			_allocator = allocator;
			_gameSession = gameSession;

			_gameSession.EntityAdded += OnEntityAdded;
			_gameSession.EntityRemoved += OnEntityRemoved;
		}

		/// <summary>
		///     Gets the number of players currently connected to the game session.
		/// </summary>
		public int PlayerCount
		{
			get { return _gameSession.Players.Count; }
		}

		/// <summary>
		///     Synchronizes the added entity with all clients.
		/// </summary>
		/// <param name="entity">The entity that has been added.</param>
		private void OnEntityAdded(Entity entity)
		{
			Assert.InRange(entity.NetworkType);
			Assert.InRange(entity.UpdateMessageType);

			entity.NetworkIdentity = _networkIdentities.Allocate();

			var message = CreateEntityAddMessage(entity);
			Broadcast(message);

			Log.DebugIf(EnableTracing, "(Server) +{1} {0}", message.Entity, message.EntityType);
			entity.OnServerAdded();
		}

		/// <summary>
		///     Synchronizes the removed entity with all clients.
		/// </summary>
		/// <param name="entity">The entity that has been removed.</param>
		private void OnEntityRemoved(Entity entity)
		{
			Log.DebugIf(EnableTracing, "(Server) -{1} {0}", entity.NetworkIdentity, entity.NetworkType);
			entity.OnServerRemoved();

			Broadcast(EntityRemoveMessage.Create(_allocator, entity.NetworkIdentity));
			_networkIdentities.Free(entity.NetworkIdentity);
		}

		/// <summary>
		///     Raised when a message should be broadcast to all connected clients.
		/// </summary>
		public event Action<Message> Broadcast;

		/// <summary>
		///     Sends a snapshot of the current game state to the given connection.
		/// </summary>
		/// <param name="connection">The connection the state snapshot should be sent to.</param>
		/// <param name="clientPlayer">The player that represents the client.</param>
		public void SendStateSnapshot(Connection connection, Player clientPlayer)
		{
			Assert.ArgumentNotNull(connection);
			Assert.ArgumentNotNull(clientPlayer);

			Log.DebugIf(EnableTracing, "(Server) Sending game state snapshot to {0}, player '{1}' ({2}).",
				connection.RemoteEndPoint, clientPlayer.Name, clientPlayer.Identity);

			// Synchronize all players
			foreach (var player in _gameSession.Players)
			{
				var message = PlayerJoinMessage.Create(_allocator, player.Identity, player.Name);
				connection.Send(message);

				Log.DebugIf(EnableTracing, "(Server)    {0}", message);
			}

			// Synchronize all entities
			foreach (var entity in _gameSession.SceneGraph.EnumeratePreOrder<Entity>())
			{
				var message = CreateEntityAddMessage(entity);
				connection.Send(message);

				Log.DebugIf(EnableTracing, "(Server)    {0}", message);
			}

			// Mark the end of the synchronization
			var syncedMessage = ClientSyncedMessage.Create(_allocator, clientPlayer.Identity);
			connection.Send(syncedMessage);

			Log.DebugIf(EnableTracing, "(Server)    Sync completed.");
		}

		/// <summary>
		///     Creates a new player with the given name.
		/// </summary>
		/// <param name="playerName">The name of the new player.</param>
		public Player CreatePlayer(string playerName)
		{
			Assert.ArgumentNotNullOrWhitespace(playerName);

			// TODO: Assign unique names to all players.
			// TODO: (only take those players into account with player.LeaveReason == null when checking for shared names)

			var player = Player.Create(_allocator, playerName);
			_gameSession.Players.Add(player);

			// Broadcast the news about the new player to all clients (this message is not sent to the new client yet)
			Broadcast(PlayerJoinMessage.Create(_allocator, player.Identity, playerName));

			Log.DebugIf(EnableTracing, "(Server) Created player '{0}' ({1})", playerName, player.Identity);
			return player;
		}

		/// <summary>
		///     Removes the given player from the game session.
		/// </summary>
		/// <param name="player">The player that should be removed.</param>
		public void RemovePlayer(Player player)
		{
			Assert.ArgumentNotNull(player);

			foreach (var entity in _gameSession.SceneGraph.EnumeratePostOrder<Entity>())
			{
				if (entity.Player == player)
					entity.Remove();
			}

			Broadcast(PlayerLeaveMessage.Create(_allocator, player.Identity, player.LeaveReason));
			_gameSession.Players.Remove(player);

			Log.DebugIf(EnableTracing, "(Server) Removed player '{0}' ({1}).", player.Name, player.Identity);
		}

		/// <summary>
		///     Handles the given player input.
		/// </summary>
		/// <param name="player">The player that generated the input.</param>
		/// <param name="inputMessage">The input that should be handled.</param>
		/// <param name="inputMask">
		///     The input mask that should be used to determine which of the eight state values per input must be
		///     considered.
		/// </param>
		public void HandlePlayerInput(Player player, PlayerInputMessage inputMessage, byte inputMask)
		{
			Assert.ArgumentNotNull(player);
			Assert.ArgumentNotNull(inputMessage);

			if (player.Ship == null || player.Ship.IsRemoved)
				return;

			for (var i = 0; i < NetworkProtocol.WeaponSlotCount; ++i)
				_fireWeapons[i] = (inputMask & inputMessage.FireWeapons[i]) != 0;

			player.Ship.HandlePlayerInput(
				inputMessage.Target,
				(inputMask & inputMessage.Forward) != 0,
				(inputMask & inputMessage.Backward) != 0,
				(inputMask & inputMessage.StrafeLeft) != 0,
				(inputMask & inputMessage.StrafeRight) != 0,
				(inputMask & inputMessage.AfterBurner) != 0,
				_fireWeapons);
		}

		/// <summary>
		///     Changes the loadout of the given player.
		/// </summary>
		/// <param name="player">The player with the new loadout.</param>
		/// <param name="loadout">The new loadout of the player.</param>
		public void ChangeLoadout(Player player, PlayerLoadoutMessage loadout)
		{
			Assert.ArgumentNotNull(player);
			Assert.ArgumentNotNull(loadout);

			var hasChanged = player.ShipType != loadout.ShipType;

			for (var i = 0; i < NetworkProtocol.WeaponSlotCount; ++i)
				hasChanged |= player.WeaponTypes[i] != loadout.WeaponTypes[i];

			player.ShipType = loadout.ShipType;

			for (var i = 0; i < NetworkProtocol.WeaponSlotCount; ++i)
				player.WeaponTypes[i] = loadout.WeaponTypes[i];

			// Broadcast the changes to all clients
			if (hasChanged)
			{
				Log.DebugIf(EnableTracing, "(Server) Changing loadout of player '{0}' ({1}): {2}.", player.Name, player.Identity, loadout);
				Broadcast(loadout);
			}

			// Respawn the player if necessary
			if (player.Ship != null && !player.Ship.IsRemoved)
				return;

			Log.DebugIf(EnableTracing, "(Server) Respawning player '{0}' ({1}).", player.Name, player.Identity);

			player.Ship.SafeDispose();
			player.Ship = Ship.Create(_gameSession, ShipTemplate.DefaultShip, player, position: new Vector2(0, 30000));
			player.Ship.AcquireOwnership();
			player.Ship.AttachTo(_gameSession.SceneGraph.Root);
		}

		/// <summary>
		///     Changes the name of the given player.
		/// </summary>
		/// <param name="player">The player whose name should be changed.</param>
		/// <param name="playerName">The new name of the player.</param>
		public void ChangePlayerName(Player player, string playerName)
		{
			Assert.ArgumentNotNull(player);
			Assert.ArgumentNotNullOrWhitespace(playerName);

			if (player.Name == playerName)
				return;

			// TODO: Assign unique names to all players.
			// TODO: (only take those players into account with player.LeaveReason == null when checking for shared names)

			Log.DebugIf(EnableTracing, "(Server) Player '{0}' ({1}) is renamed to '{2}'.", player.Name, player.Identity, playerName);
			player.Name = playerName;
			Broadcast(PlayerNameMessage.Create(_allocator, player.Identity, playerName));
		}

		/// <summary>
		///     Handles a chat message sent by the given player.
		/// </summary>
		/// <param name="player">The player that sent the chat message.</param>
		/// <param name="message">The chat message that has been sent.</param>
		public void Chat(Player player, string message)
		{
			Assert.ArgumentNotNull(player);
			Assert.ArgumentNotNullOrWhitespace(message);

			Broadcast(PlayerChatMessage.Create(_allocator, player.Identity, message));
			Log.DebugIf(EnableTracing, "(Server) Player '{0}' ({1}): {2}", player.Name, player.Identity, message);
		}

		/// <summary>
		///     Creates an entity add message for the given entity.
		/// </summary>
		/// <param name="entity">The entity the message should be created for.</param>
		private EntityAddMessage CreateEntityAddMessage(Entity entity)
		{
			var parentIdentity = NetworkProtocol.ReservedEntityIdentity;
			var parentEntity = entity.Parent as Entity;
			if (parentEntity != null)
				parentIdentity = parentEntity.NetworkIdentity;

			return EntityAddMessage.Create(_allocator, entity.NetworkIdentity, entity.Player.Identity, parentIdentity, entity.NetworkType);
		}

		/// <summary>
		///     Broadcasts all entity updates to all connected clients.
		/// </summary>
		public void BroadcastEntityUpdates()
		{
			foreach (var entity in _gameSession.SceneGraph.EnumeratePreOrder<Entity>())
			{
				Assert.InRange(entity.NetworkType);
				Assert.That(entity.UpdateMessageType == MessageType.UpdateCircle ||
							entity.UpdateMessageType == MessageType.UpdatePosition ||
							entity.UpdateMessageType == MessageType.UpdateTransform ||
							entity.UpdateMessageType == MessageType.UpdateShip ||
							entity.UpdateMessageType == MessageType.UpdateRay, "Unsupported update message type.");

				Broadcast(CreateUpdateMessage(entity));
			}
		}

		/// <summary>
		///     Creates the update message for the given entity.
		/// </summary>
		/// <param name="entity">The entity the update message should be created for.</param>
		private Message CreateUpdateMessage(Entity entity)
		{
			var orientation = MathUtils.RadToDeg360(-entity.Orientation);

			switch (entity.UpdateMessageType)
			{
				case MessageType.UpdateTransform:
					return UpdateTransformMessage.Create(_allocator, entity.NetworkIdentity, entity.Position2D, orientation);
				case MessageType.UpdatePosition:
					return UpdatePositionMessage.Create(_allocator, entity.NetworkIdentity, entity.Position2D);
				case MessageType.UpdateRay:
					var target = NetworkProtocol.ReservedEntityIdentity;
					return UpdateRayMessage.Create(_allocator, entity.NetworkIdentity, target, entity.Position2D, 2000, orientation);
				case MessageType.UpdateShip:
					return UpdateShipMessage.Create(_allocator, entity.NetworkIdentity, entity.Position2D, orientation, 100, 0, _weaponEnergyLevels);
				case MessageType.UpdateCircle:
					throw new NotImplementedException();
				default:
					throw new InvalidOperationException("Unsupported update message type.");
			}
		}
	}
}