namespace Lwar.Gameplay.Entities
{
	using System;
	using Actors;
	using Pegasus;
	using Pegasus.Math;

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
		/// <param name="position">The updated entity position.</param>
		/// <param name="rotation">The updated entity rotation.</param>
		/// <param name="health">The updated entity health.</param>
		public override void RemoteUpdate(Vector2 position, float rotation, int health)
		{
			base.RemoteUpdate(position, rotation, health);
			Health = health;
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
		///   Invoked when the actor is removed from the game session.
		/// </summary>
		/// <remarks>This method is not called when the game session is shut down.</remarks>
		protected override void OnRemoved()
		{
			GameSession.Actors.Add(Explosion.Create(Position));

			if (Player.Ship == this)
				Player.Ship = null;
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
			ship.Identifier = id;
			ship.Player = player;
			ship.Template = EntityTemplates.Ship;
			player.Ship = ship;
			return ship;
		}

		/// <summary>
		///   Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return String.Format("Ship {0} of Player {1}", Identifier, Player);
		}
	}
}