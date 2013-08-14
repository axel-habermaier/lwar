using System;

namespace Lwar.Network.Messages
{
	using System.Runtime.InteropServices;
	using Gameplay;
	using Gameplay.Entities;
	using Pegasus.Framework;

	/// <summary>
	///   Holds the payload of a Selection message.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct SelectionMessage
	{
		/// <summary>
		///   The player whose ship and weapons types are changed.
		/// </summary>
		public Identifier Player;

		/// <summary>
		///   The selected ship type.
		/// </summary>
		public EntityType ShipType;

		/// <summary>
		///   The selected weapon type for the first weapon slot.
		/// </summary>
		public EntityType WeaponType1;

		/// <summary>
		///   The selected weapon type for the second weapon slot.
		/// </summary>
		public EntityType WeaponType2;

		/// <summary>
		///   The selected weapon type for the third weapon slot.
		/// </summary>
		public EntityType WeaponType3;

		/// <summary>
		///   The selected weapon type for the fourth weapon slot.
		/// </summary>
		public EntityType WeaponType4;

		/// <summary>
		///   Creates a message that instructs the server to change the ship and weapon types of the given player.
		/// </summary>
		/// <param name="player">The player whose ship and weapon types should be changed.</param>
		/// <param name="ship">The new ship type.</param>
		/// <param name="weapon1">The type of the weapon in the first weapon slot.</param>
		/// <param name="weapon2">The type of the weapon in the second weapon slot.</param>
		/// <param name="weapon3">The type of the weapon in the third weapon slot.</param>
		/// <param name="weapon4">The type of the weapon in the fourth weapon slot.</param>
		public static Message Create(Player player, EntityType ship,
									 EntityType weapon1, EntityType weapon2,
									 EntityType weapon3, EntityType weapon4)
		{
			Assert.ArgumentNotNull(player);
			Assert.ArgumentInRange(ship);
			Assert.ArgumentInRange(weapon1);
			Assert.ArgumentInRange(weapon2);
			Assert.ArgumentInRange(weapon3);
			Assert.ArgumentInRange(weapon4);

			return new Message
			{
				Type = MessageType.Selection,
				Selection = new SelectionMessage
				{
					Player = player.Identifier,
					ShipType = ship,
					WeaponType1 = weapon1,
					WeaponType2 = weapon2,
					WeaponType3 = weapon3,
					WeaponType4 = weapon4
				}
			};
		}
	}
}