namespace Lwar.Gameplay.Server.Entities
{
	using System;
	using Behaviors;
	using Pegasus.Math;
	using Pegasus.Utilities;
	using Templates;

	internal partial class Weapon
	{
		/// <summary>
		///     The behavior of the weapon.
		/// </summary>
		private WeaponBehavior _behavior;

		/// <summary>
		///     Handles the given player input.
		/// </summary>
		/// <param name="fireWeapon">Indicates whether the weapon should be fired.</param>
		public void HandlePlayerInput(bool fireWeapon)
		{
			_behavior.HandlePlayerInput(fireWeapon);
		}

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