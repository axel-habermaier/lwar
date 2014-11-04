namespace Lwar.Gameplay.Server.Components
{
	using System;

	/// <summary>
	///     Describes the state of the after burner.
	/// </summary>
	public enum AfterBurnerState
	{
		/// <summary>
		///     Indicates that the after burner is fully charged and can be activated.
		/// </summary>
		FullyCharged,

		/// <summary>
		///     Indicates that the after burner is inactive and recharging. It can only be activated if the minimum required energy
		///     level has been recharged.
		/// </summary>
		Recharging,

		/// <summary>
		///     Indicates that the after burner is currently active.
		/// </summary>
		Active,

		/// <summary>
		///     Indicates that the after burner is inactive and waiting to be recharged once the recharge delay has passed.
		/// </summary>
		WaitingForRecharging
	}
}