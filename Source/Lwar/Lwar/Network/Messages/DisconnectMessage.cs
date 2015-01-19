namespace Lwar.Network.Messages
{
	using System;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Informs a remote peer about a disconnect.
	/// </summary>
	[UnreliableTransmission(MessageType.Disconnect)]
	internal sealed class DisconnectMessage : Message
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static DisconnectMessage()
		{
			ConstructorCache.Register(() => new DisconnectMessage());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private DisconnectMessage()
		{
		}

		/// <summary>
		///     Serializes the message using the given writer.
		/// </summary>
		/// <param name="writer">The writer that should be used to serialize the message.</param>
		public override void Serialize(ref BufferWriter writer)
		{
			// No payload
		}

		/// <summary>
		///     Deserializes the message using the given reader.
		/// </summary>
		/// <param name="reader">The reader that should be used to deserialize the message.</param>
		public override void Deserialize(ref BufferReader reader)
		{
			// No payload
		}

		/// <summary>
		///     Dispatches the message to the given dispatcher.
		/// </summary>
		/// <param name="handler">The dispatcher that should be used to dispatch the message.</param>
		/// <param name="sequenceNumber">The sequence number of the message.</param>
		public override void Dispatch(IMessageHandler handler, uint sequenceNumber)
		{
			handler.OnDisconnect(this);
		}

		/// <summary>
		///     Creates a disconnect message.
		/// </summary>
		/// <param name="poolAllocator">The pool allocator that should be used to allocate the message.</param>
		public static DisconnectMessage Create(PoolAllocator poolAllocator)
		{
			Assert.ArgumentNotNull(poolAllocator);
			return poolAllocator.Allocate<DisconnectMessage>();
		}

		/// <summary>
		///     Returns a string that represents the message.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0}", MessageType);
		}
	}
}