namespace Lwar.Gameplay.Client
{
	using System;
	using Entities;
	using Network;
	using Network.Messages;
	using Pegasus.Platform.Network;
	using Pegasus.Utilities;

	/// <summary>
	///     Dispatches messages received from the server to the game session.
	/// </summary>
	internal partial class MessageHandler : IMessageHandler
	{
		/// <summary>
		///     The game session the messages are dispatched to.
		/// </summary>
		private readonly ClientGameSession _gameSession;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the messages should be dispatched to.</param>
		public MessageHandler(ClientGameSession gameSession)
		{
			Assert.ArgumentNotNull(gameSession);
			_gameSession = gameSession;
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		public void OnDisconnect(DisconnectMessage message)
		{
			throw new ServerQuitException();
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		public void OnEntityAdded(EntityAddMessage message)
		{
			_gameSession.Entities.Add(message.Entity, message.Player, message.ParentEntity, message.EntityType);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		public void OnPlayerChatMessage(PlayerChatMessage message)
		{
			_gameSession.EventMessages.AddChatMessage(message.Player, message.Message);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		public void OnEntityCollision(EntityCollisionMessage message)
		{
			_gameSession.Entities.OnCollision(message.Entity1, message.Entity2, message.ImpactPosition);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		public void OnPlayerJoin(PlayerJoinMessage message)
		{
			_gameSession.Players.Add(message.Player, message.PlayerName);
			_gameSession.EventMessages.AddJoinMessage(message.Player);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		public void OnPlayerKill(PlayerKillMessage message)
		{
			_gameSession.EventMessages.AddKillMessage(message.Killer, message.Victim);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		public void OnPlayerLeave(PlayerLeaveMessage message)
		{
			// Add the event message first, otherwise the player will already have been removed
			switch (message.Reason)
			{
				case LeaveReason.ConnectionDropped:
					_gameSession.EventMessages.AddTimeoutMessage(message.Player);
					break;
				case LeaveReason.Misbehaved:
					_gameSession.EventMessages.AddKickedMessage(message.Player, "Network protocol violation.");
					break;
				case LeaveReason.Disconnect:
					_gameSession.EventMessages.AddLeaveMessage(message.Player);
					break;
				default:
					Assert.InRange(message.Reason);
					break;
			}

			_gameSession.Players.Remove(message.Player);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		public void OnPlayerName(PlayerNameMessage message)
		{
			// Ignore the server player
			if (message.Player == NetworkProtocol.ServerPlayerIdentity)
				return;

			var player = _gameSession.Players[message.Player];
			Assert.NotNull(player);

			var previousName = player.DisplayName;
			_gameSession.Players.ChangeName(message.Player, message.PlayerName);
			_gameSession.EventMessages.AddNameChangeMessage(message.Player, previousName);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		public void OnReject(ClientRejectedMessage message)
		{
			switch (message.Reason)
			{
				case RejectReason.Full:
					throw new ServerFullException();
				case RejectReason.VersionMismatch:
					throw new ProtocolMismatchException();
				default:
					throw new InvalidOperationException("Unknown reject reason.");
			}
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		public void OnEntityRemove(EntityRemoveMessage message)
		{
			_gameSession.Entities.Remove(message.Entity);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		public void OnPlayerSelection(PlayerLoadoutMessage message)
		{
			// TODO
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		/// <param name="sequenceNumber">The sequence number of the dispatched message.</param>
		public void OnPlayerStats(PlayerStatsMessage message, uint sequenceNumber)
		{
			_gameSession.Players.UpdateStats(message.Player, message.Kills, message.Deaths, message.Ping, sequenceNumber);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		public void OnSynced(ClientSyncedMessage message)
		{
			Assert.IsNull(_gameSession.Players.LocalPlayer, "A local player has already been set.");

			_gameSession.IsSynced = true;

			var player = _gameSession.Players[message.LocalPlayer];
			Assert.NotNull(player, "Unknown local player.");

			player.IsLocalPlayer = true;
			_gameSession.Players.LocalPlayer = player;
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		/// <param name="sequenceNumber">The sequence number of the dispatched message.</param>
		public void OnUpdateCircle(UpdateCircleMessage message, uint sequenceNumber)
		{
			var entity = _gameSession.Entities[message.Entity] as ICircleEntity;
			Assert.NotNull(entity, "Received a circle update message for a non-circle entity.");

			if (entity.AcceptUpdate(sequenceNumber))
				entity.RemoteCircleUpdate(message.Center, message.Radius);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		/// <param name="sequenceNumber">The sequence number of the dispatched message.</param>
		public void OnUpdateTransform(UpdateTransformMessage message, uint sequenceNumber)
		{
			var entity = _gameSession.Entities[message.Entity];
			if (entity == null || !entity.AcceptUpdate(sequenceNumber))
				return;

			entity.RemotePositionUpdate(message.Position);
			entity.RemoteOrientationUpdate(message.Orientation);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		/// <param name="sequenceNumber">The sequence number of the dispatched message.</param>
		public void OnUpdatePosition(UpdatePositionMessage message, uint sequenceNumber)
		{
			var entity = _gameSession.Entities[message.Entity];
			if (entity != null && entity.AcceptUpdate(sequenceNumber))
				entity.RemotePositionUpdate(message.Position);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		/// <param name="sequenceNumber">The sequence number of the dispatched message.</param>
		public void OnUpdateRay(UpdateRayMessage message, uint sequenceNumber)
		{
			var entity = _gameSession.Entities[message.Entity] as IRayEntity;
			Assert.NotNull(entity, "Received a ray update message for a non-ray entity.");

			if (!entity.AcceptUpdate(sequenceNumber))
				return;

			IEntity target = null;
			if (message.Target != NetworkProtocol.ReservedEntityIdentity)
				target = _gameSession.Entities[message.Target];

			entity.RemoteRayUpdate(message.Origin, message.Orientation, message.Length, target);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		/// <param name="sequenceNumber">The sequence number of the dispatched message.</param>
		public void OnUpdateShip(UpdateShipMessage message, uint sequenceNumber)
		{
			var ship = _gameSession.Entities[message.Ship] as ShipEntity;
			Assert.NotNull(ship, "Received a ship update message for a non-ship entity.");

			if (!ship.AcceptUpdate(sequenceNumber))
				return;

			ship.RemotePositionUpdate(message.Position);
			ship.RemoteOrientationUpdate(message.Orientation);

			ship.HullIntegrity = message.HullIntegrity;
			ship.Shields = message.Shields;
		}
	}
}