namespace Lwar.Gameplay.Server.Entities
{
	using System;
	using Behaviors;
	using Network;
	using Network.Messages;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;
	using Templates;

	/// <summary>
	///     Represents a player ship.
	/// </summary>
	public partial class Ship : Entity
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Ship()
		{
			ConstructorCache.Register(() => new Ship());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private Ship()
		{
			NetworkType = EntityType.Ship;
			UpdateMessageType = MessageType.UpdateShip;
			Weapons = new Weapon[NetworkProtocol.WeaponSlotCount];
		}

		/// <summary>
		///     Gets the ship's weapons.
		/// </summary>
		public Weapon[] Weapons { get; private set; }

		/// <summary>
		///     Gets the current warp drive energy level.
		/// </summary>
		public float WarpDriveEnergy { get; private set; }

		/// <summary>
		///     Gets the template providing the configurable parameters of the ship.
		/// </summary>
		public ShipTemplate Template { get; private set; }

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the entity belongs to.</param>
		/// <param name="template">The template providing the configurable parameters of the ship.</param>
		/// <param name="player">The player the ship belongs to.</param>
		/// <param name="position">The initial position of the ship.</param>
		/// <param name="orientation">The initial orientation of the ship.</param>
		public static Ship Create(GameSession gameSession, ShipTemplate template, Player player,
								  Vector2 position = default(Vector2), float orientation = 0)
		{
			Assert.ArgumentNotNull(gameSession);

			var ship = gameSession.Allocate<Ship>();
			ship.GameSession = gameSession;
			ship.Player = player;
			ship.Position2D = position;
			ship.Template = template;
			ship.Orientation = orientation;
			ship.Velocity = Vector2.Zero;
			ship.WarpDriveEnergy = template.WarpDrive.MaxEnergy;
			ship._acceleration = Vector2.Zero;
			ship._remainingRechargeDelay = 0;
			ship._rotationVelocity = 0;
			ship._remainingWarpCooldown = 0;
			ship._propulsionVelocity = Vector2.Zero;
			ship._warpDriveState = WarpDriveState.FullyCharged;
			ship.AddBehavior(BoundaryBehavior.Create(gameSession.Allocator));
			return ship;
		}
	}
}