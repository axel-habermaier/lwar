namespace Lwar.Network.Messages
{
	using System;
	using System.Linq;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Informs a client about the state of a ship.
	/// </summary>
	[UnreliableTransmission(MessageType.UpdateShip, EnableBatching = true)]
	internal sealed class UpdateShipMessage : Message
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static UpdateShipMessage()
		{
			ConstructorCache.Register(() => new UpdateShipMessage());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private UpdateShipMessage()
		{
			WeaponEnergyLevels = new int[NetworkProtocol.WeaponSlotCount];
		}

		/// <summary>
		///     Gets the new ship position.
		/// </summary>
		public Vector2 Position { get; private set; }

		/// <summary>
		///     Gets the new ship rotation.
		/// </summary>
		public float Orientation { get; private set; }

		/// <summary>
		///     Gets the ship that is updated.
		/// </summary>
		public NetworkIdentity Ship { get; private set; }

		/// <summary>
		///     Gets the new hull integrity of the ship in the range [0,100].
		/// </summary>
		public int HullIntegrity { get; private set; }

		/// <summary>
		///     Gets the new shield energy level of the ship in the range [0,100].
		/// </summary>
		public int Shields { get; private set; }

		/// <summary>
		///     Gets the new energy levels of the ship's weapons in the range [0,100].
		/// </summary>
		public int[] WeaponEnergyLevels { get; private set; }

		/// <summary>
		///     Serializes the message using the given writer.
		/// </summary>
		/// <param name="writer">The writer that should be used to serialize the message.</param>
		public override void Serialize(ref BufferWriter writer)
		{
			WriteIdentifier(ref writer, Ship);
			WriteVector2(ref writer, Position);
			WriteOrientation(ref writer, Orientation);
			writer.WriteByte((byte)HullIntegrity);
			writer.WriteByte((byte)Shields);

			for (var i = 0; i < NetworkProtocol.WeaponSlotCount; ++i)
				writer.WriteByte((byte)WeaponEnergyLevels[i]);
		}

		/// <summary>
		///     Deserializes the message using the given reader.
		/// </summary>
		/// <param name="reader">The reader that should be used to deserialize the message.</param>
		public override void Deserialize(ref BufferReader reader)
		{
			Ship = ReadIdentifier(ref reader);
			Position = ReadVector2(ref reader);
			Orientation = ReadOrientation(ref reader);
			HullIntegrity = reader.ReadByte();
			Shields = reader.ReadByte();

			for (var i = 0; i < NetworkProtocol.WeaponSlotCount; ++i)
				WeaponEnergyLevels[i] = reader.ReadByte();
		}

		/// <summary>
		///     Dispatches the message to the given dispatcher.
		/// </summary>
		/// <param name="handler">The dispatcher that should be used to dispatch the message.</param>
		/// <param name="sequenceNumber">The sequence number of the message.</param>
		public override void Dispatch(IMessageHandler handler, uint sequenceNumber)
		{
			handler.OnUpdateShip(this, sequenceNumber);
		}

		/// <summary>
		///     Creates an update message that the server broadcasts to all players.
		/// </summary>
		/// <param name="poolAllocator">The pool allocator that should be used to allocate the message.</param>
		/// <param name="ship">The ship that is updated.</param>
		/// <param name="position">The updated position of the ship.</param>
		/// <param name="orientation">The updated orientation of the ship in degrees.</param>
		/// <param name="hullIntegrity">The updated hull integrity of the ship.</param>
		/// <param name="shields">The updated shield energy of the ship.</param>
		/// <param name="weaponEnergyLevels">The updated energy levels of the ship.</param>
		public static Message Create(PoolAllocator poolAllocator, NetworkIdentity ship, Vector2 position, float orientation,
									 int hullIntegrity, int shields, int[] weaponEnergyLevels)
		{
			Assert.ArgumentNotNull(poolAllocator);
			Assert.ArgumentInRange(hullIntegrity, 0, 100);
			Assert.ArgumentInRange(shields, 0, 100);
			Assert.ArgumentNotNull(weaponEnergyLevels);
			Assert.ArgumentSatisfies(weaponEnergyLevels.Length == NetworkProtocol.WeaponSlotCount, "Unexpected array size.");
			Assert.ArgumentSatisfies(weaponEnergyLevels.All(e => e >= 0 && e <= 100), "Invalid weapon energy levels.");

			var message = poolAllocator.Allocate<UpdateShipMessage>();
			message.Ship = ship;
			message.Position = position;
			message.Orientation = orientation;
			message.HullIntegrity = hullIntegrity;
			message.Shields = shields;

			for (var i = 0; i < NetworkProtocol.WeaponSlotCount; ++i)
				message.WeaponEnergyLevels[i] = weaponEnergyLevels[i];

			return message;
		}

		/// <summary>
		///     Returns a string that represents the message.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0}, Ship={1}, Position={{{2}}}, Orientation={3}, HullIntegrity={4}, Shields={5}, {6}",
				MessageType, Ship, Position, Orientation, HullIntegrity, Shields,
				String.Join(", ", WeaponEnergyLevels.Select((level, index) => String.Format("WeaponEnergyLevel{0}={1}", index + 1, level))));
		}
	}
}