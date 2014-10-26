namespace Lwar.Gameplay.Server
{
	using System;
	using System.Collections.Generic;
	using Components;
	using Network;
	using Network.Messages;
	using Pegasus.Entities;
	using Pegasus.Math;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;
	using Scripts;

	/// <summary>
	///     Implements the server logic for handling incoming client messages and the synchronization of client game states.
	/// </summary>
	public class ServerLogic
	{
		/// <summary>
		///     If tracing is enabled, all server-specific gameplay events are shown in the debug output.
		/// </summary>
		private const bool EnableTracing = true;

		/// <summary>
		///     The allocator that is used to allocate pooled objects.
		/// </summary>
		private readonly PoolAllocator _allocator;

		/// <summary>
		///     The game session that is being played.
		/// </summary>
		private readonly GameSession _gameSession;

		/// <summary>
		///     The players that have been removed from the game session. They have to be kept active for as long as there still are
		///     active entities belonging to them.
		/// </summary>
		private readonly List<Player> _inactivePlayers = new List<Player>();

		/// <summary>
		///     The allocator for networked identities.
		/// </summary>
		private readonly IdentityAllocator _networkIdentities = new IdentityAllocator(UInt16.MaxValue);

		/// <summary>
		///     A cached set that is used to sync parent entities before their children. The set contains the network identities of all
		///     entities that have already been synced.
		/// </summary>
		private readonly HashSet<Identity> _syncedIdentities = new HashSet<Identity>();

		/// <summary>
		///     A cached list that is used to sync parent entities before their children. The queue contains all entities that have not
		///     yet been synced.
		/// </summary>
		private readonly Queue<Entity> _unsyncedEntities = new Queue<Entity>();

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
		}

		/// <summary>
		///     Gets the number of players currently connected to the game session.
		/// </summary>
		public int PlayerCount
		{
			get { return _gameSession.Players.Count; }
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

			// Synchronize all network-synced entities
			foreach (var entity in _gameSession.Entities)
				_unsyncedEntities.Enqueue(entity);

			SendEntities(connection);
			_unsyncedEntities.Clear();
			_syncedIdentities.Clear();

			// Mark the end of the synchronization
			var syncedMessage = ClientSyncedMessage.Create(_allocator, clientPlayer.Identity);
			connection.Send(syncedMessage);
			Log.DebugIf(EnableTracing, "(Server)    Sync completed.");
		}

		/// <summary>
		///     Sends add messages for all active entities to the given connection. This method ensures that a child entity is always
		///     synced after its parent.
		/// </summary>
		/// <param name="connection">The connection the add messages should be sent to.</param>
		private void SendEntities(Connection connection)
		{
			// Note: This algorithm will not terminate if the entity graph contains cycles.
			while (_unsyncedEntities.Count > 0)
			{
				var entity = _unsyncedEntities.Dequeue();
				var network = entity.GetComponent<NetworkSync>();
				if (network == null)
					continue;

				var relativeTransform = entity.GetComponent<RelativeTransform>();
				if (relativeTransform != null)
				{
					var childNetwork = relativeTransform.ParentEntity.GetComponent<NetworkSync>();
					if (childNetwork != null && !_syncedIdentities.Contains(childNetwork.Identity))
					{
						_unsyncedEntities.Enqueue(entity);
						continue;
					}
				}

				var message = CreateEntityAddMessage(entity);
				_syncedIdentities.Add(network.Identity);
				connection.Send(message);

				Log.DebugIf(EnableTracing, "(Server)    {0}", message);
			}
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

			// Broadcast the news about the new player to all clients (this message is not sent to the new client)
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
			_inactivePlayers.Add(player);
		}

		/// <summary>
		///     Removes all inactive players that no longer have any active entities.
		/// </summary>
		public void RemoveInactivePlayers()
		{
			for (var i = _inactivePlayers.Count - 1; i >= 0; --i)
			{
				var player = _inactivePlayers[i];
				var hasActiveEntities = false;

				foreach (var entity in _gameSession.Entities)
				{
					var owner = entity.GetComponent<Owner>();
					if (owner == null || owner.Player != player)
						continue;

					_gameSession.Entities.Remove(entity);
					hasActiveEntities = true;
				}

				if (hasActiveEntities)
				{
					Log.DebugIf(EnableTracing, "(Server) Delayed removal of inactive player '{0}' ({1}) because of active player entities.",
						player.Name, player.Identity);

					continue;
				}

				Broadcast(PlayerLeaveMessage.Create(_allocator, player.Identity, player.LeaveReason));
				_gameSession.Players.Remove(player);
				_inactivePlayers.RemoveAt(i);

				Log.DebugIf(EnableTracing, "(Server) Removed player '{0}' ({1}).", player.Name, player.Identity);
			}
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

			var entity = player.ControlledEntity;
			if (!entity.IsAlive)
				return;

			var input = entity.GetComponent<PlayerInput>();
			Assert.NotNull(input);

			input.Target = inputMessage.Target;
			input.Forward = (inputMask & inputMessage.Forward) != 0;
			input.Backward = (inputMask & inputMessage.Backward) != 0;
			input.TurnLeft = (inputMask & inputMessage.TurnLeft) != 0;
			input.TurnRight = (inputMask & inputMessage.TurnRight) != 0;
			input.StrafeLeft = (inputMask & inputMessage.StrafeLeft) != 0;
			input.StrafeRight = (inputMask & inputMessage.StrafeRight) != 0;

			for (var i = 0; i < NetworkProtocol.WeaponSlotCount; ++i)
				input.FireWeapons[i] = (inputMask & inputMessage.FireWeapons[i]) != 0;
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
			if (player.ControlledEntity.IsAlive)
				return;

			Log.DebugIf(EnableTracing, "(Server) Respawning player '{0}' ({1}).", player.Name, player.Identity);
			player.ControlledEntity = _gameSession.EntityFactory.CreateShip(player, position: new Vector2(0, 30000));
			var scripts = player.ControlledEntity.GetComponent<ScriptCollection>();

			for (var i = 0; i < NetworkProtocol.WeaponSlotCount; ++i)
				AddWeaponScript(player, scripts, i);
		}

		/// <summary>
		///     Adds a weapon script to the given entity for the given input index.
		/// </summary>
		/// <param name="player">The player the weapon script should be added for.</param>
		/// <param name="scripts">The script collection the weapon script should be added to.</param>
		/// <param name="inputIndex">The input index that should trigger the weapon.</param>
		private void AddWeaponScript(Player player, ScriptCollection scripts, int inputIndex)
		{
			Assert.ArgumentNotNull(player);
			Assert.ArgumentNotNull(scripts);
			Assert.ArgumentInRange(inputIndex, 0, NetworkProtocol.WeaponSlotCount - 1);

			switch (player.WeaponTypes[inputIndex])
			{
				case EntityType.Gun:
					scripts.Add(FireBulletScript.Create(_allocator, inputIndex, cooldown: 0.15f));
					break;
				case EntityType.Phaser:
					scripts.Add(FirePhaserScript.Create(_allocator, inputIndex));
					break;
				default:
					throw new InvalidOperationException("Unsupported weapon type.");
			}
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
		///     Synchronizes the addition of given new entity to all connected clients.
		/// </summary>
		/// <param name="entity">The entity that has been added.</param>
		public void EntityAdded(Entity entity)
		{
			var networkSync = entity.GetComponent<NetworkSync>();
			networkSync.Identity = _networkIdentities.Allocate();

			var message = CreateEntityAddMessage(entity);
			Assert.NotNull(message);

			Log.DebugIf(EnableTracing, "(Server) +{1} {0}", message.Entity, message.EntityType);

			Broadcast(message);
		}

		/// <summary>
		///     Synchronizes the removal of the given entity to all connected clients.
		/// </summary>
		/// <param name="entity">The entity that has been removed.</param>
		public void EntityRemoved(Entity entity)
		{
			var networkSync = entity.GetComponent<NetworkSync>();
			Assert.NotNull(networkSync);

			Log.DebugIf(EnableTracing, "(Server) -{1} {0}", networkSync.Identity, networkSync.EntityType);
			Broadcast(EntityRemoveMessage.Create(_allocator, networkSync.Identity));
			_networkIdentities.Free(networkSync.Identity);
		}

		/// <summary>
		///     Sends the given entity update to all connected clients.
		/// </summary>
		/// <param name="message">The update message that should be sent.</param>
		public void SendEntityUpdate(Message message)
		{
			Assert.ArgumentNotNull(message);
			Broadcast(message);
		}

		/// <summary>
		///     Creates an entity add message for the given entity.
		/// </summary>
		/// <param name="entity">The entity the message should be created for.</param>
		private EntityAddMessage CreateEntityAddMessage(Entity entity)
		{
			var networkSync = entity.GetComponent<NetworkSync>();
			var owner = entity.GetComponent<Owner>();

			if (networkSync == null || owner == null)
				return null;

			var parentEntity = NetworkProtocol.ReservedEntityIdentity;
			var relativeTransform = entity.GetComponent<RelativeTransform>();
			if (relativeTransform != null)
			{
				var network = relativeTransform.ParentEntity.GetComponent<NetworkSync>();
				if (network != null)
					parentEntity = network.Identity;
			}

			return EntityAddMessage.Create(_allocator, networkSync.Identity, owner.Player.Identity, parentEntity, networkSync.EntityType);
		}
	}
}