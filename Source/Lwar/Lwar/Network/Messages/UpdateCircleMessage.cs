namespace Lwar.Network.Messages
{
	using System;
	using Pegasus.Entities;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Informs a client about an update to a circle.
	/// </summary>
	[UnreliableTransmission(MessageType.UpdateCircle, EnableBatching = true)]
	public sealed class UpdateCircleMessage : Message
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static UpdateCircleMessage()
		{
			ConstructorCache.Set(() => new UpdateCircleMessage());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private UpdateCircleMessage()
		{
		}

		/// <summary>
		///     Gets the entity that is updated.
		/// </summary>
		public Identity Entity { get; private set; }

		/// <summary>
		///     Gets the new circle center.
		/// </summary>
		public Vector2 Center { get; private set; }

		/// <summary>
		///     Gets the new circle radius.
		/// </summary>
		public float Radius { get; private set; }

		/// <summary>
		///     Serializes the message using the given writer.
		/// </summary>
		/// <param name="writer">The writer that should be used to serialize the message.</param>
		public override void Serialize(BufferWriter writer)
		{
			writer.WriteIdentifier(Entity);
			writer.WriteVector2(Center);
			writer.WriteUInt16((ushort)Radius);
		}

		/// <summary>
		///     Deserializes the message using the given reader.
		/// </summary>
		/// <param name="reader">The reader that should be used to deserialize the message.</param>
		public override void Deserialize(BufferReader reader)
		{
			Entity = reader.ReadIdentifier();
			Center = reader.ReadVector2();
			Radius = reader.ReadUInt16();
		}

		/// <summary>
		///     Dispatches the message to the given dispatcher.
		/// </summary>
		/// <param name="handler">The dispatcher that should be used to dispatch the message.</param>
		/// <param name="sequenceNumber">The sequence number of the message.</param>
		public override void Dispatch(IMessageHandler handler, uint sequenceNumber)
		{
			handler.OnUpdateCircle(this, sequenceNumber);
		}

		/// <summary>
		///     Creates an update message that the server broadcasts to all players.
		/// </summary>
		/// <param name="poolAllocator">The pool allocator that should be used to allocate the message.</param>
		/// <param name="entity">The entity that is updated.</param>
		/// <param name="center">The updated center of the circle entity.</param>
		/// <param name="radius">The updated radius of the circle entity.</param>
		public static Message Create(PoolAllocator poolAllocator, Identity entity, Vector2 center, float radius)
		{
			Assert.ArgumentNotNull(poolAllocator);

			var message = poolAllocator.Allocate<UpdateCircleMessage>();
			message.Entity = entity;
			message.Center = center;
			message.Radius = radius;
			return message;
		}

		/// <summary>
		///     Returns a string that represents the message.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0}, Entity={1}, Center={{{2}}}, Radius={3}", MessageType, Entity, Center, Radius);
		}
	}
}