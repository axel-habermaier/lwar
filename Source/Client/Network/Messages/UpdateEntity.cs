using System;

namespace Lwar.Client.Network.Messages
{
	using System.Collections.Generic;
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;

	public class UpdateEntity : PooledObject<UpdateEntity>, IReliableMessage
	{
		/// <summary>
		///   The new entity data.
		/// </summary>
		private readonly List<Data> _stats = new List<Data>();

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
		public static UpdateEntity Create(BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);

			if (!buffer.CanRead(sizeof(uint) + sizeof(byte)))
				return null;

			var message = GetInstance();
			message.SequenceNumber = buffer.ReadUInt32();
			message._stats.Clear();

			var count = buffer.ReadByte();
			if (!buffer.CanRead(count * (Identifier.Size + 4 * sizeof(short) + sizeof(ushort) + sizeof(byte))))
				return null;

			for (var i = 0; i < count; ++i)
			{
				message._stats.Add(new Data
				{
					Entity = buffer.ReadIdentifier(),
					Position = new Vector2i(buffer.ReadInt16(), buffer.ReadInt16()),
					Velocity = new Vector2i(buffer.ReadInt16(), buffer.ReadInt16()),
					Rotation = buffer.ReadUInt16(),
					Health = buffer.ReadByte()
				});
			}

			return message;
		}

		private struct Data
		{
			/// <summary>
			///   The identifier of the entity whose data is updated.
			/// </summary>
			public Identifier Entity;

			/// <summary>
			///   The updated health value.
			/// </summary>
			public byte Health;

			/// <summary>
			///   The updated position.
			/// </summary>
			public Vector2i Position;

			/// <summary>
			///   The updated rotation.
			/// </summary>
			public ushort Rotation;

			/// <summary>
			///   The updated velocity.
			/// </summary>
			public Vector2i Velocity;
		}
	}
}