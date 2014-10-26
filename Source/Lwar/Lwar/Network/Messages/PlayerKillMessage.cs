namespace Lwar.Network.Messages
{
	using System;
	using Pegasus.Entities;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Informs a client about a player kill.
	/// </summary>
	[ReliableTransmission(MessageType.PlayerKill)]
	public sealed class PlayerKillMessage : Message
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static PlayerKillMessage()
		{
			ConstructorCache.Set(() => new PlayerKillMessage());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private PlayerKillMessage()
		{
		}

		/// <summary>
		///     Gets the player that was killed.
		/// </summary>
		public Identity Victim { get; private set; }

		/// <summary>
		///     Gets the player that scored the kill.
		/// </summary>
		public Identity Killer { get; private set; }

		/// <summary>
		///     Serializes the message using the given writer.
		/// </summary>
		/// <param name="writer">The writer that should be used to serialize the message.</param>
		public override void Serialize(BufferWriter writer)
		{
			writer.WriteIdentifier(Killer);
			writer.WriteIdentifier(Victim);
		}

		/// <summary>
		///     Deserializes the message using the given reader.
		/// </summary>
		/// <param name="reader">The reader that should be used to deserialize the message.</param>
		public override void Deserialize(BufferReader reader)
		{
			Killer = reader.ReadIdentifier();
			Victim = reader.ReadIdentifier();
		}

		/// <summary>
		///     Dispatches the message to the given dispatcher.
		/// </summary>
		/// <param name="handler">The dispatcher that should be used to dispatch the message.</param>
		/// <param name="sequenceNumber">The sequence number of the message.</param>
		public override void Dispatch(IMessageHandler handler, uint sequenceNumber)
		{
			handler.OnPlayerKill(this);
		}

		/// <summary>
		///     Creates a kill message that the server broadcasts to all clients.
		/// </summary>
		/// <param name="poolAllocator">The pool allocator that should be used to allocate the message.</param>
		/// <param name="killer">The entity that scored the kill.</param>
		/// <param name="victim">The entity that was killed.</param>
		public static Message Create(PoolAllocator poolAllocator, Identity killer, Identity victim)
		{
			Assert.ArgumentNotNull(poolAllocator);

			var message = poolAllocator.Allocate<PlayerKillMessage>();
			message.Killer = killer;
			message.Victim = victim;
			return message;
		}

		/// <summary>
		///     Returns a string that represents the message.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0}, Killer={1}, Victim={2}", MessageType, Killer, Victim);
		}
	}
}