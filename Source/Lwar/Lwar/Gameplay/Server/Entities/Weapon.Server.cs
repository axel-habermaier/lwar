namespace Lwar.Gameplay.Server.Entities
{
	using System;
	using Behaviors;

	public partial class Weapon
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
	}
}