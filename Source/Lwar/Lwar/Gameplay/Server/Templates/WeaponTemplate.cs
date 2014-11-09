namespace Lwar.Gameplay.Server.Templates
{
	using System;
	using Network;

	/// <summary>
	///     The configurable parameters of a weapon.
	/// </summary>
	public struct WeaponTemplate
	{
		/// <summary>
		///     The template of the gun that fires bullets.
		/// </summary>
		public static WeaponTemplate Gun = new WeaponTemplate
		{
			Cooldown = 0.1f,
			DepleteSpeed = 0,
			MaxEnergy = 1,
			ActivationEnergy = 0,
			RechargeDelay = Single.MaxValue,
			RechargeSpeed = 0,
			BaseSpeed = 3000,
			NetworkType = EntityType.Gun
		};

		/// <summary>
		///     The template of the phaser that creates a phaser beam.
		/// </summary>
		public static WeaponTemplate Phaser = new WeaponTemplate
		{
			Cooldown = -1,
			DepleteSpeed = 50,
			MaxEnergy = 1000,
			ActivationEnergy = 100,
			RechargeDelay = 1,
			RechargeSpeed = 50,
			Range = 2000,
			NetworkType = EntityType.Phaser
		};

		/// <summary>
		///     The minimum amount of energy required to fire the weapon.
		/// </summary>
		public float ActivationEnergy;

		/// <summary>
		///     The base speed of a projectile weapon, to which the ship's speed is added or subtracted, depending on the shooting
		///     direction.
		/// </summary>
		public float BaseSpeed;

		/// <summary>
		///     The amount of seconds to wait before to consecutive shots of the weapon. A negative value indicates that the weapon
		///     fires continuously.
		/// </summary>
		public float Cooldown;

		/// <summary>
		///     The amount of energy to deplete per second when the weapon is firing.
		/// </summary>
		public float DepleteSpeed;

		/// <summary>
		///     The maximum energy level of the weapon.
		/// </summary>
		public float MaxEnergy;

		/// <summary>
		///     The network type of the weapon.
		/// </summary>
		public EntityType NetworkType;

		/// <summary>
		///     The range of a range-based weapon.
		/// </summary>
		public float Range;

		/// <summary>
		///     The amount of time (in seconds) to wait before the weapon energy is starting to recharge. Single.NaN should be used
		///     to indicate that the energy does not recharge.
		/// </summary>
		public float RechargeDelay;

		/// <summary>
		///     The amount of energy to recharge per second when the weapon is not enabled.
		/// </summary>
		public float RechargeSpeed;

		/// <summary>
		///     Gets a value indicating whether the weapon fires continuously.
		/// </summary>
		public bool FiresContinuously
		{
			get { return Cooldown < 0; }
		}
	}
}