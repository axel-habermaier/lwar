using System;

namespace Lwar.Client.Network.Messages
{
	using Gameplay;
	using Gameplay.Entities;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Rendering;

	public class AddMessage : Message<AddMessage>, IReliableMessage
	{
		/// <summary>
		///   The identifier of the entity that is added.
		/// </summary>
		private Identifier _entityId;

		/// <summary>
		///   The identifier of the player that the new entity belongs to.
		/// </summary>
		private Identifier _playerId;

		/// <summary>
		///   The type template of the entity that is added.
		/// </summary>
		private EntityTemplate _template;

		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="gameSession">The game session that should be affected by the message.</param>
		/// <param name="renderContext">The render context that should be affected by the message.</param>
		public override void Process(GameSession gameSession, RenderContext renderContext)
		{
			//IEntity entity;
			//switch (_template)
			//{
			//	case EntityTemplate.Ship:
			//		var ship = Ship.Create(_entityId, session.GameSession.PlayerMap[_playerId]);
			//		entity = ship;

			//		if (_template == EntityTemplate.Ship && ship.Player == session.GameSession.LocalPlayer)
			//			session.GameSession.LocalPlayer.Ship = ship;
			//		break;
			//	case EntityTemplate.Bullet:
			//		entity = Bullet.Create(_entityId);
			//		break;
			//	case EntityTemplate.Planet:
			//		entity = Planet.Create(_entityId);
			//		break;
			//	case EntityTemplate.Rocket:
			//	case EntityTemplate.Ray:
			//	case EntityTemplate.ShockWave:
			//	case EntityTemplate.Gun:
			//		// TODO
			//		entity = null;
			//		break;
			//	default:
			//		throw new InvalidOperationException("Unknown entity template.");
			//}

			//session.GameSession.Entities.Add(entity);
			//session.GameSession.EntityMap.Add(entity);
			//entity.Added(session.GameSession, session.RenderContext);
		}

		/// <summary>
		///   Gets or sets the sequence number of the message.
		/// </summary>
		public uint SequenceNumber { get; set; }

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="buffer">The buffer from which the instance should be deserialized.</param>
		public static AddMessage Create(BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			return Deserialize(buffer, (b, m) =>
				{
					m._entityId = b.ReadIdentifier();
					m._playerId = b.ReadIdentifier();
					m._template = (EntityTemplate)b.ReadByte();
				});
		}
	}
}