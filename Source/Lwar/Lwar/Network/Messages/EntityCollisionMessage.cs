namespace Lwar.Network.Messages
{
	using System;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Notifies a client about a collision between two entities.
	/// </summary>
	[UnreliableTransmission(MessageType.EntityCollision)]
	internal sealed class EntityCollisionMessage : Message
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static EntityCollisionMessage()
		{
			ConstructorCache.Register(() => new EntityCollisionMessage());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private EntityCollisionMessage()
		{
		}

		/// <summary>
		///     Gets the first entity involved in the collision.
		/// </summary>
		public NetworkIdentity Entity1 { get; private set; }

		/// <summary>
		///     Gets the second entity involved in the collision.
		/// </summary>
		public NetworkIdentity Entity2 { get; private set; }

		/// <summary>
		///     Gets the position of the impact.
		/// </summary>
		public Vector2 ImpactPosition { get; private set; }

		/// <summary>
		///     Serializes the message using the given writer.
		/// </summary>
		/// <param name="writer">The writer that should be used to serialize the message.</param>
		public override void Serialize(ref BufferWriter writer)
		{
			WriteIdentifier(ref writer, Entity1);
			WriteIdentifier(ref writer, Entity2);
			WriteVector2(ref writer, ImpactPosition);
		}

		/// <summary>
		///     Deserializes the message using the given reader.
		/// </summary>
		/// <param name="reader">The reader that should be used to deserialize the message.</param>
		public override void Deserialize(ref BufferReader reader)
		{
			Entity1 = ReadIdentifier(ref reader);
			Entity2 = ReadIdentifier(ref reader);
			ImpactPosition = ReadVector2(ref reader);
		}

		/// <summary>
		///     Dispatches the message to the given dispatcher.
		/// </summary>
		/// <param name="handler">The dispatcher that should be used to dispatch the message.</param>
		/// <param name="sequenceNumber">The sequence number of the message.</param>
		public override void Dispatch(IMessageHandler handler, uint sequenceNumber)
		{
			handler.OnEntityCollision(this);
		}

		/// <summary>
		///     Creates an collision message that the server broadcasts to all clients.
		/// </summary>
		/// <param name="poolAllocator">The pool allocator that should be used to allocate the message.</param>
		/// <param name="entity1">The first entity of the collision.</param>
		/// <param name="entity2">The second entity of the collision.</param>
		/// <param name="impactPosition">The position of the impact.</param>
		public static Message Create(PoolAllocator poolAllocator, NetworkIdentity entity1, NetworkIdentity entity2, Vector2 impactPosition)
		{
			Assert.ArgumentNotNull(poolAllocator);

			var message = poolAllocator.Allocate<EntityCollisionMessage>();
			message.Entity1 = entity1;
			message.Entity2 = entity2;
			message.ImpactPosition = impactPosition;
			return message;
		}

		/// <summary>
		///     Returns a string that represents the message.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0}, Entity1={1}, Entity2={2}, ImpactPosition={3}", MessageType, Entity1, Entity2, ImpactPosition);
		}
	}
}