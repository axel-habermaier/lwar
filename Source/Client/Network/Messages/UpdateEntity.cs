using System;

namespace Lwar.Client.Network.Messages
{
	using System.Collections.Generic;
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;

	public class UpdateEntity : PooledObject<UpdateEntity>, IUnreliableMessage
	{
		/// <summary>
		///   The new entity data.
		/// </summary>
		private readonly List<Data> _data = new List<Data>();

		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="session">The game session that should be updated.</param>
		public void Process(GameSession session)
		{
			Assert.ArgumentNotNull(session, () => session);

			foreach (var entityUpdate in _data)
			{
				var entity = session.Entities.Find(entityUpdate.Entity);
				entity.Position = entityUpdate.Position;
			}
		}

		/// <summary>
		///   Writes the message into the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the message should be written to.</param>
		public void Write(BufferWriter buffer)
		{
			Assert.That(false, "The client cannot send this type of message.");
		}

		/// <summary>
		///   Gets or sets the timestamp of the message.
		/// </summary>
		public uint Timestamp { get; set; }

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="buffer">The buffer from which the instance should be deserialized.</param>
		public static UpdateEntity Create(BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);

			var message = GetInstance();
			message._data.Clear();

			var count = buffer.ReadByte();
			for (var i = 0; i < count; ++i)
			{
				message._data.Add(new Data
				{
					Entity = buffer.ReadIdentifier(),
					Position = new Vector2(buffer.ReadInt16(), buffer.ReadInt16()),
					Velocity = new Vector2(buffer.ReadInt16(), buffer.ReadInt16()),
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
			public Vector2 Position;

			/// <summary>
			///   The updated rotation.
			/// </summary>
			public ushort Rotation;

			/// <summary>
			///   The updated velocity.
			/// </summary>
			public Vector2 Velocity;
		}
	}
}