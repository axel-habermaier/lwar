using System;

namespace Lwar.Client.Network.Messages
{
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;

	public class ChangePlayerState : PooledObject<ChangePlayerState>, IReliableMessage
	{
		/// <summary>
		///   The identifier of the player that changed his or her name.
		/// </summary>
		private Identifier _playerId;

		/// <summary>
		///   The new ship type.
		/// </summary>
		private byte _shipType;

		/// <summary>
		///   The new weapon type.
		/// </summary>
		private byte _weaponType;

		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="session">The game session that should be updated.</param>
		public void Process(GameSession session)
		{
		}

		/// <summary>
		///   Writes the message into the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the message should be written to.</param>
		public void Write(BufferWriter buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);

			buffer.WriteByte((byte)MessageType.ChangePlayerName);
			buffer.WriteUInt32(SequenceNumber);
			buffer.WriteIdentifier(_playerId);
			buffer.WriteByte(_shipType);
			buffer.WriteByte(_weaponType);
		}

		/// <summary>
		///   Gets or sets the sequence number of the message.
		/// </summary>
		public uint SequenceNumber { get; set; }

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="buffer">The buffer from which the instance should be deserialized.</param>
		public static ChangePlayerState Create(BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);

			var message = GetInstance();
			message._playerId = buffer.ReadIdentifier();
			message._shipType = buffer.ReadByte();
			message._weaponType = buffer.ReadByte();
			return message;
		}
	}
}