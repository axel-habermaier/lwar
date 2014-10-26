namespace Lwar.Network.Messages
{
	using System;
	using System.Linq;
	using Pegasus.Entities;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Notifies a server and all of its clients about the ship selection and weapon load out of a client.
	/// </summary>
	[ReliableTransmission(MessageType.PlayerLoadout)]
	public sealed class PlayerLoadoutMessage : Message
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static PlayerLoadoutMessage()
		{
			ConstructorCache.Set(() => new PlayerLoadoutMessage());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private PlayerLoadoutMessage()
		{
			WeaponTypes = new EntityType[NetworkProtocol.WeaponSlotCount];
		}

		/// <summary>
		///     Gets the player whose ship and weapons types are changed.
		/// </summary>
		public Identity Player { get; private set; }

		/// <summary>
		///     Gets the selected ship type.
		/// </summary>
		public EntityType ShipType { get; private set; }

		/// <summary>
		///     Gets the selected weapon types.
		/// </summary>
		public EntityType[] WeaponTypes { get; private set; }

		/// <summary>
		///     Serializes the message using the given writer.
		/// </summary>
		/// <param name="writer">The writer that should be used to serialize the message.</param>
		public override void Serialize(BufferWriter writer)
		{
			writer.WriteIdentifier(Player);
			writer.WriteByte((byte)ShipType);

			for (var i = 0; i < NetworkProtocol.WeaponSlotCount; ++i)
				writer.WriteByte((byte)WeaponTypes[i]);
		}

		/// <summary>
		///     Deserializes the message using the given reader.
		/// </summary>
		/// <param name="reader">The reader that should be used to deserialize the message.</param>
		public override void Deserialize(BufferReader reader)
		{
			Player = reader.ReadIdentifier();
			ShipType = (EntityType)reader.ReadByte();

			for (var i = 0; i < NetworkProtocol.WeaponSlotCount; ++i)
				WeaponTypes[i] = (EntityType)reader.ReadByte();
		}

		/// <summary>
		///     Dispatches the message to the given dispatcher.
		/// </summary>
		/// <param name="handler">The dispatcher that should be used to dispatch the message.</param>
		/// <param name="sequenceNumber">The sequence number of the message.</param>
		public override void Dispatch(IMessageHandler handler, uint sequenceNumber)
		{
			handler.OnPlayerSelection(this);
		}

		/// <summary>
		///     Creates a message that instructs the server to change the ship and weapon types of the given player.
		/// </summary>
		/// <param name="poolAllocator">The pool allocator that should be used to allocate the message.</param>
		/// <param name="player">The player whose ship and weapon types should be changed.</param>
		/// <param name="ship">The new ship type.</param>
		/// <param name="weapons">The types of the selected weapons.</param>
		public static PlayerLoadoutMessage Create(PoolAllocator poolAllocator, Identity player, EntityType ship, EntityType[] weapons)
		{
			Assert.ArgumentNotNull(poolAllocator);
			Assert.ArgumentInRange(ship);
			Assert.ArgumentNotNull(weapons);
			Assert.ArgumentSatisfies(weapons.Length == NetworkProtocol.WeaponSlotCount, "Unexpected array size.");

			var message = poolAllocator.Allocate<PlayerLoadoutMessage>();
			message.Player = player;
			message.ShipType = ship;

			for (var i = 0; i < NetworkProtocol.WeaponSlotCount; ++i)
				message.WeaponTypes[i] = weapons[i];

			return message;
		}

		/// <summary>
		///     Returns a string that represents the message.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0}, Player={1}, ShipType={2}, {3}", MessageType, Player, ShipType,
				String.Join(", ", WeaponTypes.Select((weapon, index) => String.Format("WeaponType{0}={1}", index + 1, weapon))));
		}
	}
}