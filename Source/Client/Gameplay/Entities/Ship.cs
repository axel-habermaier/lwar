using System;

namespace Lwar.Client.Gameplay.Entities
{
	using Actors;
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;

	/// <summary>
	///   Represents a ship that is controlled by a local or remote player.
	/// </summary>
	public class Ship : Entity<Ship>
	{
		/// <summary>
		///   Gets or sets the player the ship belongs to.
		/// </summary>
		public Player Player { get; private set; }

		/// <summary>
		///   The health of the ship.
		/// </summary>
		public int Health { get; private set; }

		/// <summary>
		///   Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="message">The update message that should be processed.</param>
		public override void RemoteUpdate(ref Message message)
		{
			Assert.That(message.Type == MessageType.Update, "Unsupported update type.");

			Position = message.Update.Position;
			Rotation = MathUtils.DegToRad(message.Update.Rotation);
			Health = message.Update.Health;
		}

		/// <summary>
		///   Invoked when the entity collided another entity.
		/// </summary>
		/// <param name="other">The other entity this instance collided with.</param>
		/// <param name="impact">The position of the impact.</param>
		public override void CollidedWith(IEntity other, Vector2 impact)
		{
			GameSession.Actors.Add(Shield.Create(this, impact));
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="id">The generational identifier of the ship.</param>
		/// <param name="player">The player the ship belongs to.</param>
		public static Ship Create(Identifier id, Player player)
		{
			Assert.ArgumentNotNull(player);

			var ship = GetInstance();
			ship.Id = id;
			ship.Player = player;
			ship.Template = Templates.Ship;
			player.Ship = ship;
			return ship;
		}

		/// <summary>
		///   Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return String.Format("Ship {0}, Player: {1}", Id, Player);
		}
	}
}