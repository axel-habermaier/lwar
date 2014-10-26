namespace Lwar.Network.Messages
{
	using System;
	using Pegasus.Entities;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Informs a client about an update to a ray.
	/// </summary>
	[UnreliableTransmission(MessageType.UpdateRay, EnableBatching = true)]
	public sealed class UpdateRayMessage : Message
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static UpdateRayMessage()
		{
			ConstructorCache.Set(() => new UpdateRayMessage());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private UpdateRayMessage()
		{
		}

		/// <summary>
		///     Gets the entity that is updated.
		/// </summary>
		public Identity Entity { get; private set; }

		/// <summary>
		///     Gets the orientation of the ray.
		/// </summary>
		public float Orientation { get; private set; }

		/// <summary>
		///     Gets the new ray length.
		/// </summary>
		public float Length { get; private set; }

		/// <summary>
		///     Gets the new ray origin.
		/// </summary>
		public Vector2 Origin { get; private set; }

		/// <summary>
		///     Gets the target entity that is hit by the ray, if any.
		/// </summary>
		public Identity Target { get; private set; }

		/// <summary>
		///     Serializes the message using the given writer.
		/// </summary>
		/// <param name="writer">The writer that should be used to serialize the message.</param>
		public override void Serialize(BufferWriter writer)
		{
			writer.WriteIdentifier(Entity);
			writer.WriteVector2(Origin);
			writer.WriteOrientation(Orientation);
			writer.WriteUInt16((ushort)Length);
			writer.WriteIdentifier(Target);
		}

		/// <summary>
		///     Deserializes the message using the given reader.
		/// </summary>
		/// <param name="reader">The reader that should be used to deserialize the message.</param>
		public override void Deserialize(BufferReader reader)
		{
			Entity = reader.ReadIdentifier();
			Origin = reader.ReadVector2();
			Orientation = reader.ReadOrientation();
			Length = reader.ReadUInt16();
			Target = reader.ReadIdentifier();
		}

		/// <summary>
		///     Dispatches the message to the given dispatcher.
		/// </summary>
		/// <param name="handler">The dispatcher that should be used to dispatch the message.</param>
		/// <param name="sequenceNumber">The sequence number of the message.</param>
		public override void Dispatch(IMessageHandler handler, uint sequenceNumber)
		{
			handler.OnUpdateRay(this, sequenceNumber);
		}

		/// <summary>
		///     Creates an update message that the server broadcasts to all players.
		/// </summary>
		/// <param name="poolAllocator">The pool allocator that should be used to allocate the message.</param>
		/// <param name="entity">The entity that is updated.</param>
		/// <param name="target">The targeted entity of the ray.</param>
		/// <param name="origin">The origin of the ray.</param>
		/// <param name="length">The length of the ray.</param>
		/// <param name="orientation">The orientation of the ray.</param>
		public static Message Create(PoolAllocator poolAllocator, Identity entity, Identity target, Vector2 origin,
									 float length, float orientation)
		{
			Assert.ArgumentNotNull(poolAllocator);

			var message = poolAllocator.Allocate<UpdateRayMessage>();
			message.Entity = entity;
			message.Target = target;
			message.Origin = origin;
			message.Length = length;
			message.Orientation = orientation;
			return message;
		}

		/// <summary>
		///     Returns a string that represents the message.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0}, Entity={1}, Target={{{2}}}, Origin={{{3}}}, Length={4}, Orientation={5}",
				MessageType, Entity, Target, Origin, Length, Orientation);
		}
	}
}