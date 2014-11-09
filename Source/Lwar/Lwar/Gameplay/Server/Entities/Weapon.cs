namespace Lwar.Gameplay.Server.Entities
{
	using System;
	using Behaviors;
	using Network.Messages;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;
	using Templates;

	/// <summary>
	///     Represents a ship weapon.
	/// </summary>
	public partial class Weapon : Entity
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Weapon()
		{
			ConstructorCache.Register(() => new Weapon());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private Weapon()
		{
			UpdateMessageType = MessageType.UpdatePosition;
		}

		/// <summary>
		///     Gets or sets the current energy level of the weapon.
		/// </summary>
		public float Energy { get; set; }

		/// <summary>
		///     Gets the template providing the configurable parameters of the weapon.
		/// </summary>
		public WeaponTemplate Template { get; private set; }

		/// <summary>
		///     Gets the ship the weapon belongs to.
		/// </summary>
		public Ship Ship { get; private set; }

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the entity belongs to.</param>
		/// <param name="ship">The ship the weapon should be attached to.</param>
		/// <param name="template">The template providing the configurable parameters of the weapon.</param>
		/// <param name="player">The player the weapon belongs to.</param>
		/// <param name="behavior">The behavior of the weapon.</param>
		public static Weapon Create(GameSession gameSession, Ship ship, WeaponTemplate template, Player player, WeaponBehavior behavior)
		{
			Assert.ArgumentNotNull(gameSession);
			Assert.ArgumentNotNull(ship);
			Assert.ArgumentNotNull(player);
			Assert.ArgumentNotNull(behavior);

			var weapon = gameSession.Allocate<Weapon>();
			weapon.GameSession = gameSession;
			weapon.NetworkType = template.NetworkType;
			weapon.Player = player;
			weapon.Ship = ship;
			weapon.Position2D = Vector2.Zero;
			weapon.Template = template;
			weapon.Orientation = 0;
			weapon.Energy = template.MaxEnergy;
			weapon._behavior = behavior;
			weapon.AddBehavior(behavior);

			ship.AttachChild(weapon);
			return weapon;
		}
	}
}