namespace Lwar.Network.Messages
{
	using System;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Informs a client that the game state is now fully synced.
	/// </summary>
	[ReliableTransmission(MessageType.ClientSynced)]
	internal sealed class ClientSyncedMessage : Message
	{
		/// <summary>
		///     Gets the identity of the synced client's local player.
		/// </summary>
		public NetworkIdentity LocalPlayer { get; private set; }

		/// <summary>
		///     Serializes the message using the given writer.
		/// </summary>
		/// <param name="writer">The writer that should be used to serialize the message.</param>
		public override void Serialize(ref BufferWriter writer)
		{
			WriteIdentifier(ref writer, LocalPlayer);
		}

		/// <summary>
		///     Deserializes the message using the given reader.
		/// </summary>
		/// <param name="reader">The reader that should be used to deserialize the message.</param>
		public override void Deserialize(ref BufferReader reader)
		{
			LocalPlayer = ReadIdentifier(ref reader);
		}

		/// <summary>
		///     Dispatches the message to the given dispatcher.
		/// </summary>
		/// <param name="handler">The dispatcher that should be used to dispatch the message.</param>
		/// <param name="sequenceNumber">The sequence number of the message.</param>
		public override void Dispatch(IMessageHandler handler, uint sequenceNumber)
		{
			handler.OnSynced(this);
		}

		/// <summary>
		///     Creates a synced message that signals a client that the game state is now fully synced.
		/// </summary>
		/// <param name="poolAllocator">The pool allocator that should be used to allocate the message.</param>
		/// <param name="localPlayer">The identity of the synced client's local player.</param>
		public static Message Create(PoolAllocator poolAllocator, NetworkIdentity localPlayer)
		{
			Assert.ArgumentNotNull(poolAllocator);

			var message = poolAllocator.Allocate<ClientSyncedMessage>();
			message.LocalPlayer = localPlayer;
			return message;
		}

		/// <summary>
		///     Returns a string that represents the message.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0}, LocalPlayer={1}", MessageType, LocalPlayer);
		}
	}
}