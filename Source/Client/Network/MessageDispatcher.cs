using System;

namespace Lwar.Client.Network
{
	using Gameplay;
	using Gameplay.Entities;

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
					// TODO
					break;
				case MessageType.Name:
					_gameSession.Players.ChangeName(message.Name.Player, message.Name.Name);
					break;
				case MessageType.Selection:
					break;
				case MessageType.Add:
					AddEntity(message.Add.Entity, message.Add.Type, message.Add.Player);
					break;
				case MessageType.Collision:
					break;
				case MessageType.Join:
					_gameSession.Players.Add(message.Join.Player, message.Join.IsLocalPlayer);
					break;
				case MessageType.Leave:
					_gameSession.Players.Remove(message.Remove);
					break;
				case MessageType.Remove:
					_gameSession.Entities.Remove(message.Remove);
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
					entity = Sun.Create(entityId);// TODO: Create planet instead
					break;
				case EntityType.Sun:
					entity = Sun.Create(entityId);
					break;
				case EntityType.Bullet:
					entity = Bullet.Create(entityId);
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
	}
}