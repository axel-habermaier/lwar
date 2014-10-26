namespace Lwar.Network.Messages
{
	using System;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Informs a client that its connection attempt has been rejected.
	/// </summary>
	[UnreliableTransmission(MessageType.ClientRejected)]
	public sealed class ClientRejectedMessage : Message
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static ClientRejectedMessage()
		{
			ConstructorCache.Set(() => new ClientRejectedMessage());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private ClientRejectedMessage()
		{
		}

		/// <summary>
		///     Gets the reason for the reject.
		/// </summary>
		public RejectReason Reason { get; private set; }

		/// <summary>
		///     Serializes the message using the given writer.
		/// </summary>
		/// <param name="writer">The writer that should be used to serialize the message.</param>
		public override void Serialize(BufferWriter writer)
		{
			writer.WriteByte((byte)Reason);
		}

		/// <summary>
		///     Deserializes the message using the given reader.
		/// </summary>
		/// <param name="reader">The reader that should be used to deserialize the message.</param>
		public override void Deserialize(BufferReader reader)
		{
			Reason = (RejectReason)reader.ReadByte();
		}

		/// <summary>
		///     Dispatches the message to the given dispatcher.
		/// </summary>
		/// <param name="handler">The dispatcher that should be used to dispatch the message.</param>
		/// <param name="sequenceNumber">The sequence number of the message.</param>
		public override void Dispatch(IMessageHandler handler, uint sequenceNumber)
		{
			handler.OnReject(this);
		}

		/// <summary>
		///     Creates a reject message that the server sends when it rejects a connection attempt from a client.
		/// </summary>
		/// <param name="poolAllocator">The pool allocator that should be used to allocate the message.</param>
		/// <param name="reason">The reason why the connection attempt was rejected.</param>
		public static Message Create(PoolAllocator poolAllocator, RejectReason reason)
		{
			Assert.ArgumentNotNull(poolAllocator);
			Assert.ArgumentInRange(reason);

			var message = poolAllocator.Allocate<ClientRejectedMessage>();
			message.Reason = reason;
			return message;
		}

		/// <summary>
		///     Returns a string that represents the message.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0}, Reason={1}", MessageType, Reason);
		}
	}
}