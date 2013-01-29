﻿using System;

namespace Lwar.Client.Network.Messages
{
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;

	public class AddEntity : PooledObject<AddEntity>, IReliableMessage
	{
		/// <summary>
		///   The size of the message in bytes.
		/// </summary>
		private const int Size = sizeof(uint) + 2 * Identifier.Size + sizeof(byte);

		/// <summary>
		///   The identifier of the entity that is added.
		/// </summary>
		private Identifier _entityId;

		/// <summary>
		///   The identifier of the player that the new entity belongs to.
		/// </summary>
		private Identifier _playerId;

		/// <summary>
		///   The type template of the entity that is added.
		/// </summary>
		private EntityTemplate _template;

		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="session">The game session that should be updated.</param>
		public void Process(GameSession session)
		{
		}

		/// <summary>
		///   Serializes the message into the given buffer, returning false if the message did not fit.
		/// </summary>
		/// <param name="buffer">The buffer the message should be written to.</param>
		public bool Serialize(BufferWriter buffer)
		{
			Assert.That(false, "The client cannot send this type of message.");
			return true;
		}

		/// <summary>
		///   Gets or sets the sequence number of the message.
		/// </summary>
		public uint SequenceNumber { get; set; }

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="buffer">The buffer from which the instance should be deserialized.</param>
		public static AddEntity Create(BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);

			if (!buffer.CanRead(Size))
				return null;

			var message = GetInstance();
			message.SequenceNumber = buffer.ReadUInt32();
			message._entityId = buffer.ReadIdentifier();
			message._playerId = buffer.ReadIdentifier();
			message._template = (EntityTemplate)buffer.ReadByte();

			return message;
		}
	}
}