namespace Lwar.Gameplay.Entities
{
	using System;
	using Actors;
	using Pegasus;
	using Pegasus.Math;

	/// <summary>
	///     Represents a ship that is controlled by a local or remote player.
	/// </summary>
	public class Ship : Entity<Ship>
	{
		/// <summary>
		///     Gets or sets the player the ship belongs to.
		/// </summary>
		public Player Player { get; private set; }

		/// <summary>
		///     Gets or sets the hull integrity of the ship in the range [0,100].
		/// </summary>
		public int HullIntegrity { get; set; }

		/// <summary>
		///     Gets or sets the shield energy level of the ship in the range [0,100].
		/// </summary>
		public int Shields { get; set; }

		/// <summary>
		///     Gets or sets the energy level of the ship's first weapon slot in the range [0,100].
		/// </summary>
		public int Energy1 { get; set; }

		/// <summary>
		///     Gets or sets the energy level of the ship's second weapon slot in the range [0,100].
		/// </summary>
		public int Energy2 { get; set; }

		/// <summary>
		///     Gets or sets theenergy level of the ship's third weapon slot in the range [0,100].
		/// </summary>
		public int Energy3 { get; set; }

		/// <summary>
		///     Gets or sets the energy level of the ship's fourth weapon slot in the range [0,100].
		/// </summary>
		public int Energy4 { get; set; }

		/// <summary>
		///     Invoked when the entity collided another entity.
		/// </summary>
		/// <param name="other">The other entity this instance collided with.</param>
		/// <param name="impact">The position of the impact.</param>
		public override void CollidedWith(IEntity other, Vector2 impact)
		{
			GameSession.Actors.Add(Shield.Create(GameSession, this, impact));
		}

		/// <summary>
		///     Invoked when the actor is removed from the game session.
		/// </summary>
		/// <remarks>This method is not called when the game session is shut down.</remarks>
		protected override void OnRemoved()
		{
			GameSession.Actors.Add(Explosion.Create(GameSession, Position));

			if (Player.Ship == this)
				Player.Ship = null;
		}

		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the instance should be created for.</param>
		/// <param name="id">The generational identifier of the ship.</param>
		/// <param name="player">The player the ship belongs to.</param>
		public static Ship Create(GameSession gameSession, Identifier id, Player player)
		{
			Assert.ArgumentNotNull(gameSession);
			Assert.ArgumentNotNull(player);

			var ship = gameSession.Allocate<Ship>();
			ship.Identifier = id;
			ship.Player = player;
			ship.Template = EntityTemplates.Ship;
			player.Ship = ship;
			return ship;
		}

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return String.Format("Ship {0} of Player {1}", Identifier, Player);
		}
	}
}