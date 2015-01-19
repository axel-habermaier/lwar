namespace Lwar.Gameplay.Server.Entities
{
	using System;
	using Network;
	using Network.Messages;
	using Pegasus.Platform.Memory;
	using Templates;

	/// <summary>
	///     Represents a player ship.
	/// </summary>
	internal partial class Ship : Entity
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
	}
}