namespace Lwar.Gameplay.Server.Entities
{
	using System;
	using Network.Messages;
	using Templates;

	/// <summary>
	///     Represents a ship weapon.
	/// </summary>
	internal partial class Weapon : Entity
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public Weapon()
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
	}
}