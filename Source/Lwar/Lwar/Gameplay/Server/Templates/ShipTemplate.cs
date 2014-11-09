namespace Lwar.Gameplay.Server.Templates
{
	using System;

	/// <summary>
	///     The configurable parameters of a ship.
	/// </summary>
	public struct ShipTemplate
	{
		/// <summary>
		///     The template of the default ship.
		/// </summary>
		public static ShipTemplate DefaultShip = new ShipTemplate
		{
			MaxAcceleration = 7000,
			MaxSpeed = 1000,
			MaxRotationSpeed = 7,
			Drag = 0.1f,
			WarpDrive = new WarpDriveTemplate
			{
				MaxSpeed = 7000,
				MaxEnergy = 1000,
				ActivationEnergy = 500,
				RechargeDelay = 2,
				RechargeSpeed = 200,
				DepleteSpeed = 300,
				Cooldown = 5,
				Drag = 0.8f
			}
		};

		/// <summary>
		///     The drag that slows the ship down to a halt when not acceleration.
		/// </summary>
		public float Drag;

		/// <summary>
		///     The maximum allowed acceleration of the ship.
		/// </summary>
		public float MaxAcceleration;

		/// <summary>
		///     The maximum rotational speed of the ship.
		/// </summary>
		public float MaxRotationSpeed;

		/// <summary>
		///     The maximum speed of the ship when the warp drive is disabled.
		/// </summary>
		public float MaxSpeed;

		/// <summary>
		///     The template parameters of the ship's warp drive.
		/// </summary>
		public WarpDriveTemplate WarpDrive;
	}
}