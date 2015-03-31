﻿namespace Lwar.Gameplay.Server.Entities
{
	using System;
	using Network;
	using Network.Messages;
	using Templates;

	/// <summary>
	///     Represents a player ship.
	/// </summary>
	internal partial class Ship : Entity
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public Ship()
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