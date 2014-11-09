namespace Lwar.Network.Messages
{
	using System;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Informs a client about an updated entity position and orientation.
	/// </summary>
	[UnreliableTransmission(MessageType.UpdateTransform, EnableBatching = true)]
	public sealed class UpdateTransformMessage : Message
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static UpdateTransformMessage()
		{
			ConstructorCache.Register(() => new UpdateTransformMessage());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private UpdateTransformMessage()
		{
		}

		/// <summary>
		///     Gets the entity that is updated.
		/// </summary>
		public NetworkIdentity Entity { get; private set; }

		/// <summary>
		///     Gets the new entity position.
		/// </summary>
		public Vector2 Position { get; private set; }

		/// <summary>
		///     Gets the new entity rotation.
		/// </summary>
		public float Orientation { get; private set; }

		/// <summary>
		///     Serializes the message using the given writer.
		/// </summary>
		/// <param name="writer">The writer that should be used to serialize the message.</param>
		public override void Serialize(BufferWriter writer)
		{
			writer.WriteIdentifier(Entity);
			writer.WriteVector2(Position);
			writer.WriteOrientation(Orientation);
		}

		/// <summary>
		///     Deserializes the message using the given reader.
		/// </summary>
		/// <param name="reader">The reader that should be used to deserialize the message.</param>
		public override void Deserialize(BufferReader reader)
		{
			Entity = reader.ReadIdentifier();
			Position = reader.ReadVector2();
			Orientation = reader.ReadOrientation();
		}

		/// <summary>
		///     Dispatches the message to the given dispatcher.
		/// </summary>
		/// <param name="handler">The dispatcher that should be used to dispatch the message.</param>
		/// <param name="sequenceNumber">The sequence number of the message.</param>
		public override void Dispatch(IMessageHandler handler, uint sequenceNumber)
		{
			handler.OnUpdateTransform(this, sequenceNumber);
		}

		/// <summary>
		///     Creates an update message that the server broadcasts to all players.
		/// </summary>
		/// <param name="poolAllocator">The pool allocator that should be used to allocate the message.</param>
		/// <param name="entity">The entity that is updated.</param>
		/// <param name="position">The updated position of the entity.</param>
		/// <param name="orientation">The updated orientation of the entity in degrees.</param>
		public static Message Create(PoolAllocator poolAllocator, NetworkIdentity entity, Vector2 position, float orientation)
		{
			Assert.ArgumentNotNull(poolAllocator);

			var message = poolAllocator.Allocate<UpdateTransformMessage>();
			message.Entity = entity;
			message.Position = position;
			message.Orientation = orientation;
			return message;
		}

		/// <summary>
		///     Returns a string that represents the message.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0}, Entity={1}, Position={{{2}}}, Orientation={3}", MessageType, Entity, Position, Orientation);
		}
	}
}