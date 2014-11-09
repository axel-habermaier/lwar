namespace Lwar.Gameplay.Server.Templates
{
	using System;

	/// <summary>
	///     The configurable parameters of a warp drive.
	/// </summary>
	public struct WarpDriveTemplate
	{
		/// <summary>
		///     The minimum amount of energy required to activate the warp drive.
		/// </summary>
		public float ActivationEnergy;

		/// <summary>
		///     The amount of time in seconds to wait before the warp drive can be activated again.
		/// </summary>
		public float Cooldown;

		/// <summary>
		///     The amount of energy to deplete per second when the warp drive is enabled.
		/// </summary>
		public float DepleteSpeed;

		/// <summary>
		///     The drag that slows the ship down from warp speed to normal speed.
		/// </summary>
		public float Drag;

		/// <summary>
		///     The maximum energy level of the warp drive.
		/// </summary>
		public float MaxEnergy;

		/// <summary>
		///     The maximum speed of the ship when the warp drive is enabled.
		/// </summary>
		public float MaxSpeed;

		/// <summary>
		///     The amount of time (in seconds) to wait before the warp drive energy is starting to recharge.
		/// </summary>
		public float RechargeDelay;

		/// <summary>
		///     The amount of energy to recharge per second when the warp drive is not enabled.
		/// </summary>
		public float RechargeSpeed;
	}
}