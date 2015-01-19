namespace Lwar.Gameplay.Client.Entities
{
	using System;
	using Actors;
	using Network;
	using Pegasus.Math;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a ship that is controlled by a local or remote player.
	/// </summary>
	internal class ShipEntity : Entity<ShipEntity>
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
		///     Gets or sets the energy level of the ship's third weapon slot in the range [0,100].
		/// </summary>
		public int Energy3 { get; set; }

		/// <summary>
		///     Gets or sets the energy level of the ship's fourth weapon slot in the range [0,100].
		/// </summary>
		public int Energy4 { get; set; }

		/// <summary>
		///     Handles a collision between this entity and another entity at the given impact position.
		/// </summary>
		/// <param name="other">The other entity this instance collided with.</param>
		/// <param name="impactPosition">The position of the impact.</param>
		public override void OnCollision(IEntity other, Vector2 impactPosition)
		{
			ShieldActor.Create(GameSession, this, impactPosition);
		}

		/// <summary>
		///     Invoked when the actor is removed from the game session.
		/// </summary>
		/// <remarks>This method is not called when the game session is shut down.</remarks>
		protected override void OnRemoved()
		{
			ExplosionActor.Create(GameSession, Position);

			if (Player.Ship == this)
				Player.Ship = null;
		}

		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the instance should be created for.</param>
		/// <param name="id">The generational identity of the ship.</param>
		/// <param name="player">The player the ship belongs to.</param>
		public static ShipEntity Create(ClientGameSession gameSession, NetworkIdentity id, Player player)
		{
			Assert.ArgumentNotNull(gameSession);
			Assert.ArgumentNotNull(player);

			var ship = gameSession.Allocate<ShipEntity>();
			ship.Identifier = id;
			ship.Player = player;
			ship.Template = EntityTemplates.Ship;
			player.Ship = ship;

			gameSession.Entities.Add(ship);
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