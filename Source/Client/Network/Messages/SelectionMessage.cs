using System;

namespace Lwar.Client.Network.Messages
{
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;

	public class SelectionMessage : Message<SelectionMessage>, IReliableMessage
	{
		/// <summary>
		///   The identifier of the player that changed his or her state.
		/// </summary>
		private Identifier _playerId;

		/// <summary>
		///   The new ship type.
		/// </summary>
		private EntityTemplate _shipType;

		/// <summary>
		///   The new weapon type.
		/// </summary>
		private EntityTemplate _weaponType1;
		private EntityTemplate _weaponType2;
		private EntityTemplate _weaponType3;
		private EntityTemplate _weaponType4;


		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="session">The game session that should be updated.</param>
		public override void Process(GameSession session)
		{
			// TODO
		}

		/// <summary>
		///   Writes the message into the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the message should be written to.</param>
		public override bool Write(BufferWriter buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			return buffer.TryWrite(this, (b, m) =>
				{
					b.WriteByte((byte)MessageType.Selection);
					b.WriteUInt32(m.SequenceNumber);
					b.WriteIdentifier(m._playerId);
					b.WriteByte((byte)m._shipType);
					b.WriteByte((byte)m._weaponType1);
					b.WriteByte((byte)m._weaponType2);
					b.WriteByte((byte)m._weaponType3);
					b.WriteByte((byte)m._weaponType4);
				});
		}

		/// <summary>
		///   Gets or sets the sequence number of the message.
		/// </summary>
		public uint SequenceNumber { get; set; }

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="buffer">The buffer from which the instance should be deserialized.</param>
		public static SelectionMessage Create(BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			return Deserialize(buffer, (b, m) =>
				{
					m._playerId = b.ReadIdentifier();
					m._shipType = (EntityTemplate)b.ReadByte();
					m._weaponType1 = (EntityTemplate)b.ReadByte();
					m._weaponType2 = (EntityTemplate)b.ReadByte();
					m._weaponType3 = (EntityTemplate)b.ReadByte();
					m._weaponType4 = (EntityTemplate)b.ReadByte();

					Assert.InRange(m._shipType);
				});
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="playerId">The identifier of the player that changed his or her state.</param>
		/// <param name="shipType">The new ship type.</param>
		/// <param name="weaponTemplate">The new weapon type.</param>
		public static SelectionMessage Create(Identifier playerId, EntityTemplate shipType,
		                                      EntityTemplate weaponTemplate1, EntityTemplate weaponTemplate2,
		                                      EntityTemplate weaponTemplate3, EntityTemplate weaponTemplate4)
		{
			var message = GetInstance();
			message._playerId = playerId;
			message._shipType = shipType;
			message._weaponType1 = weaponTemplate1;
			message._weaponType2 = weaponTemplate2;
			message._weaponType3 = weaponTemplate3;
			message._weaponType4 = weaponTemplate3;
			return message;
		}
	}
}