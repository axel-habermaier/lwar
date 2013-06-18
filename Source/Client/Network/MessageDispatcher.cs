using System;

namespace Lwar.Client.Network
{
	using Gameplay;
	using Gameplay.Entities;
	using Messages;
	using Pegasus.Framework;

	/// <summary>
	///   Dispatches messages received from the server.
	/// </summary>
	public sealed class MessageDispatcher
	{
		/// <summary>
		///   The game session the messages are dispatched to.
		/// </summary>
		private readonly GameSession _gameSession;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the messages should be dispatched to.</param>
		public MessageDispatcher(GameSession gameSession)
		{
			_gameSession = gameSession;
		}

		/// <summary>
		///   Dispatches the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		public void Dispatch(Message message)
		{
			switch (message.Type)
			{
				case MessageType.Chat:
					_gameSession.EventMessages.AddChatMessage(message.Chat.Player, message.Chat.Message);
					break;
				case MessageType.Name:
					// Ignore the server player
					if (IsServerPlayer(message.Name.Player))
						break;

					// Add the event message first, otherwise the player name will already have been changed
					_gameSession.EventMessages.AddNameChangeMessage(message.Name.Player, message.Name.Name);
					_gameSession.Players.ChangeName(message.Name.Player, message.Name.Name);
					break;
				case MessageType.Selection:
					break;
				case MessageType.Add:
					AddEntity(message.Add.Entity, message.Add.Type, message.Add.Player);
					break;
				case MessageType.Collision:
					_gameSession.Entities.OnCollision(message.Collision.Entity1, message.Collision.Entity2, message.Collision.Position);
					break;
				case MessageType.Join:
					// Give the server player a default name
					var name = IsServerPlayer(message.Join.Player) ? "Server" : message.Join.Name;
					Assert.NotNullOrWhitespace(name);

					_gameSession.Players.Add(message.Join.Player, name, message.Join.IsLocalPlayer);

					// Don't show the message for the local player and the server player
					if (!message.Join.IsLocalPlayer && !IsServerPlayer(message.Join.Player))
						_gameSession.EventMessages.AddJoinMessage(message.Join.Player);
					break;
				case MessageType.Leave:
					// Add the event message first, otherwise the player will already have been removed
					switch (message.Leave.Reason)
					{
						case LeaveReason.ConnectionDropped:
							_gameSession.EventMessages.AddTimeoutMessage(message.Leave.Player);
							break;
						case LeaveReason.Misbehaved:
							_gameSession.EventMessages.AddKickedMessage(message.Leave.Player, "Network protocol violation.");
							break;
						case LeaveReason.Quit:
							_gameSession.EventMessages.AddLeaveMessage(message.Leave.Player);
							break;
						default:
							Assert.InRange(message.Leave.Reason);
							break;
					}

					_gameSession.Players.Remove(message.Leave.Player);
					break;
				case MessageType.Remove:
					_gameSession.Entities.Remove(message.Remove);
					break;
				case MessageType.Kill:
					_gameSession.EventMessages.AddKillMessage(message.Kill.Killer, message.Kill.Victim);
					break;
				case MessageType.Stats:
					break;
				case MessageType.Update:
				case MessageType.UpdatePosition:
				case MessageType.UpdateRay:
				case MessageType.UpdateCircle:
					_gameSession.Entities.RemoteUpdate(ref message);
					break;
				default:
					throw new InvalidOperationException("Unexpected message type.");
			}
		}

		/// <summary>
		///   Adds a new entity to the game session.
		/// </summary>
		/// <param name="entityId">The identifier of the new entity.</param>
		/// <param name="type">The type of the new entity.</param>
		/// <param name="playerId">The identifier of the player the new entity belongs to.</param>
		private void AddEntity(Identifier entityId, EntityType type, Identifier playerId)
		{
			var player = _gameSession.Players[playerId];
			IEntity entity;

			switch (type)
			{
				case EntityType.Ship:
					entity = Ship.Create(entityId, player);
					break;
				case EntityType.Planet:
					entity = Planet.Create(entityId, Templates.Planet);
					break;
				case EntityType.Mars:
					entity = Planet.Create(entityId, Templates.Mars);
					break;
				case EntityType.Moon:
					entity = Planet.Create(entityId, Templates.Moon);
					break;
				case EntityType.Jupiter:
					entity = Planet.Create(entityId, Templates.Jupiter);
					break;
				case EntityType.Sun:
					entity = Sun.Create(entityId);
					break;
				case EntityType.Bullet:
					entity = Bullet.Create(entityId);
					break;
				case EntityType.Rocket:
					entity = Rocket.Create(entityId);
					break;
				case EntityType.Phaser:
					entity = Phaser.Create(entityId);
					break;
				case EntityType.Ray:
					entity = Ray.Create(entityId);
					break;
				case EntityType.Shockwave:
					entity = Shockwave.Create(entityId);
					break;
				default:
					throw new InvalidOperationException("Unexpected entity type.");
			}

			_gameSession.Entities.Add(entity);
		}

		/// <summary>
		///   Checks whether the player with the given identifier is the player that represents the server.
		/// </summary>
		/// <param name="player">The player identifier that should be checked.</param>
		private static bool IsServerPlayer(Identifier player)
		{
			return player.Identity == Specification.ServerPlayerId;
		}
	}
}