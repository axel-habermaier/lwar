using System;

namespace Lwar.Client.Network.Messages
{
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;

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
		/// <param name="session">The game session that should be updated.</param>
		public override void Process(GameSession session)
		{
			Assert.ArgumentNotNull(session, () => session);

			var player = session.Players[_playerId];
			IEntity entity;

			switch (_template)
			{
				case EntityTemplate.Ship:
					var ship = Ship.Create(player);
					entity = ship;

					if (_template == EntityTemplate.Ship && entity.Player == session.LocalPlayer)
						session.LocalPlayer.Ship = ship;
					break;
				case EntityTemplate.Bullet:
					entity = Bullet.Create(player);
					break;
				case EntityTemplate.Planet:
					entity = Planet.Create(player);
					break;
				case EntityTemplate.Rocket:
					entity = Rocket.Create(player);
					break;
				case EntityTemplate.Ray:
					// TODO
					entity = Bullet.Create(player);
					break;
				case EntityTemplate.ShockWave:
					// TODO
					entity = Bullet.Create(player);
					break;
				case EntityTemplate.Gun:
					// TODO
					entity = Bullet.Create(player);
					break;
				default:
					throw new InvalidOperationException("Unknown entity template.");
			}

			entity.Id = _entityId;
			session.Entities.Add(entity);
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