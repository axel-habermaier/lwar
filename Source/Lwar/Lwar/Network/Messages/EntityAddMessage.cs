namespace Lwar.Network.Messages
{
	using System;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Notifies a client about a newly added entity.
	/// </summary>
	[ReliableTransmission(MessageType.EntityAdd)]
	internal sealed class EntityAddMessage : Message
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static EntityAddMessage()
		{
			ConstructorCache.Register(() => new EntityAddMessage());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private EntityAddMessage()
		{
		}

		/// <summary>
		///     Gets the entity that is added.
		/// </summary>
		public NetworkIdentity Entity { get; private set; }

		/// <summary>
		///     Gets the parent entity of the entity that is added. Can be the reserved entity to indicate that the
		///     added entity has no parent.
		/// </summary>
		public NetworkIdentity ParentEntity { get; private set; }

		/// <summary>
		///     Gets the player the entity belongs to.
		/// </summary>
		public NetworkIdentity Player { get; private set; }

		/// <summary>
		///     Gets the type of the entity that is added.
		/// </summary>
		public EntityType EntityType { get; private set; }

		/// <summary>
		///     Serializes the message using the given writer.
		/// </summary>
		/// <param name="writer">The writer that should be used to serialize the message.</param>
		public override void Serialize(ref BufferWriter writer)
		{
			WriteIdentifier(ref writer, Entity);
			WriteIdentifier(ref writer, Player);
			WriteIdentifier(ref writer, ParentEntity);
			writer.WriteByte((byte)EntityType);
		}

		/// <summary>
		///     Deserializes the message using the given reader.
		/// </summary>
		/// <param name="reader">The reader that should be used to deserialize the message.</param>
		public override void Deserialize(ref BufferReader reader)
		{
			Entity = ReadIdentifier(ref reader);
			Player = ReadIdentifier(ref reader);
			ParentEntity = ReadIdentifier(ref reader);
			EntityType = (EntityType)reader.ReadByte();
		}

		/// <summary>
		///     Dispatches the message to the given dispatcher.
		/// </summary>
		/// <param name="handler">The dispatcher that should be used to dispatch the message.</param>
		/// <param name="sequenceNumber">The sequence number of the message.</param>
		public override void Dispatch(IMessageHandler handler, uint sequenceNumber)
		{
			handler.OnEntityAdded(this);
		}

		/// <summary>
		///     Creates an add message that the server broadcasts to all clients.
		/// </summary>
		/// <param name="poolAllocator">The pool allocator that should be used to allocate the message.</param>
		/// <param name="entity">The entity that has been added.</param>
		/// <param name="player">The player the entity belongs to.</param>
		/// <param name="parentEntity">The parent entity of the entity that is added.</param>
		/// <param name="entityType">The type of the entity.</param>
		public static EntityAddMessage Create(PoolAllocator poolAllocator, NetworkIdentity entity, NetworkIdentity player,
											  NetworkIdentity parentEntity, EntityType entityType)
		{
			Assert.ArgumentNotNull(poolAllocator);

			var message = poolAllocator.Allocate<EntityAddMessage>();
			message.Entity = entity;
			message.ParentEntity = parentEntity;
			message.Player = player;
			message.EntityType = entityType;
			return message;
		}

		/// <summary>
		///     Returns a string that represents the message.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0}, Entity={1}, Player={2}, ParentEntity={3}, EntityType={4}",
				MessageType, Entity, Player, ParentEntity, EntityType);
		}
	}
}