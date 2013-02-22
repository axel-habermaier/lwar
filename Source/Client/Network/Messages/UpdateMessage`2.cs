using System;

namespace Lwar.Client.Network.Messages
{
	using System.Collections.Generic;
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;

	public abstract class UpdateMessage<TUpdateData, TMessage> : Message<TMessage>, IUnreliableMessage
		where TMessage : UpdateMessage<TUpdateData, TMessage>, new()
	{
		/// <summary>
		///   The updated entity data.
		/// </summary>
		private readonly List<EntityUpdate> _updates = new List<EntityUpdate>();

		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="session">The game session that should be updated.</param>
		public override sealed void Process(GameSession session)
		{
			Assert.ArgumentNotNull(session, () => session);

			foreach (var update in _updates)
			{
				var entity = session.Entities.Find(update.Entity);

				// Entity generation mismatch
				if (entity == null)
					continue;

				Process(session, entity, update.Data);
			}
		}

		/// <summary>
		///   Gets or sets the timestamp of the message.
		/// </summary>
		public uint Timestamp { get; set; }

		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="session">The game session that should be updated.</param>
		/// <param name="entity">The entity that should be updated.</param>
		/// <param name="data">The updated data.</param>
		protected abstract void Process(GameSession session, IEntity entity, TUpdateData data);

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="buffer">The buffer from which the instance should be deserialized.</param>
		public static TMessage Create(BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			return Deserialize(buffer, (b, m) =>
				{
					m._updates.Clear();

					var count = buffer.ReadByte();
					for (var i = 0; i < count; ++i)
						m._updates.Add(new EntityUpdate(b.ReadIdentifier(), m.Extract(b)));
				});
		}

		/// <summary>
		///   Extracts an update set from the buffer.
		/// </summary>
		/// <param name="buffer">The buffer from which the update set should be extracted.</param>
		protected abstract TUpdateData Extract(BufferReader buffer);

		private struct EntityUpdate
		{
			/// <summary>
			///   The updated data.
			/// </summary>
			public readonly TUpdateData Data;

			/// <summary>
			///   The identifier of the entity whose data is updated.
			/// </summary>
			public readonly Identifier Entity;

			public EntityUpdate(Identifier entity, TUpdateData data)
			{
				Entity = entity;
				Data = data;
			}
		}
	}
}