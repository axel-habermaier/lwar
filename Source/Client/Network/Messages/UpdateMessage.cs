using System;

namespace Lwar.Client.Network.Messages
{
	using System.Collections.Generic;
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;

	public class UpdateMessage : Message<UpdateMessage>, IUnreliableMessage
	{
		/// <summary>
		///   The new entity data.
		/// </summary>
		private readonly List<Data> _data = new List<Data>();

		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="session">The game session that should be updated.</param>
		public override void Process(GameSession session)
		{
			Assert.ArgumentNotNull(session, () => session);

			foreach (var entityUpdate in _data)
			{
				var entity = session.Entities.Find(entityUpdate.Entity);

				// Entity generation mismatch
				if (entity == null)
					continue;

				entity.Position = entityUpdate.Position;
				entity.Rotation = MathUtils.DegToRad(entityUpdate.Rotation);
				entity.Health = entityUpdate.Health;
			}
		}

		/// <summary>
		///   Gets or sets the timestamp of the message.
		/// </summary>
		public uint Timestamp { get; set; }

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="buffer">The buffer from which the instance should be deserialized.</param>
		public static UpdateMessage Create(BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			return Deserialize(buffer, (b, m) =>
				{
					m._data.Clear();

					var count = buffer.ReadByte();
					for (var i = 0; i < count; ++i)
					{
						m._data.Add(new Data
						{
							Entity = b.ReadIdentifier(),
							Position = new Vector2(b.ReadInt16(), b.ReadInt16()),
							Velocity = new Vector2(b.ReadInt16(), b.ReadInt16()),
							Rotation = b.ReadUInt16(),
							Health = b.ReadByte()
						});
					}
				});
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