namespace Lwar.Network.Messages
{
	using System;
	using Pegasus.Entities;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Notifies a client about the removal of an entity.
	/// </summary>
	[ReliableTransmission(MessageType.EntityRemove)]
	public sealed class EntityRemoveMessage : Message
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static EntityRemoveMessage()
		{
			ConstructorCache.Set(() => new EntityRemoveMessage());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private EntityRemoveMessage()
		{
		}

		/// <summary>
		///     Gets the entity that is removed.
		/// </summary>
		public Identity Entity { get; private set; }

		/// <summary>
		///     Serializes the message using the given writer.
		/// </summary>
		/// <param name="writer">The writer that should be used to serialize the message.</param>
		public override void Serialize(BufferWriter writer)
		{
			writer.WriteIdentifier(Entity);
		}

		/// <summary>
		///     Deserializes the message using the given reader.
		/// </summary>
		/// <param name="reader">The reader that should be used to deserialize the message.</param>
		public override void Deserialize(BufferReader reader)
		{
			Entity = reader.ReadIdentifier();
		}

		/// <summary>
		///     Dispatches the message to the given dispatcher.
		/// </summary>
		/// <param name="handler">The dispatcher that should be used to dispatch the message.</param>
		/// <param name="sequenceNumber">The sequence number of the message.</param>
		public override void Dispatch(IMessageHandler handler, uint sequenceNumber)
		{
			handler.OnEntityRemove(this);
		}

		/// <summary>
		///     Creates a remove message that the server broadcasts to all clients.
		/// </summary>
		/// <param name="poolAllocator">The pool allocator that should be used to allocate the message.</param>
		/// <param name="entity">The entity that has been removed.</param>
		public static Message Create(PoolAllocator poolAllocator, Identity entity)
		{
			Assert.ArgumentNotNull(poolAllocator);

			var message = poolAllocator.Allocate<EntityRemoveMessage>();
			message.Entity = entity;
			return message;
		}

		/// <summary>
		///     Returns a string that represents the message.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0}, Entity={1}", MessageType, Entity);
		}
	}
}