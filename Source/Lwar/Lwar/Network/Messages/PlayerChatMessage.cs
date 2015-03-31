namespace Lwar.Network.Messages
{
	using System;
	using System.Text;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     A chat message sent between clients.
	/// </summary>
	[ReliableTransmission(MessageType.PlayerChat)]
	internal sealed class PlayerChatMessage : Message
	{
		/// <summary>
		///     Gets the message sent by the player.
		/// </summary>
		public string Message { get; private set; }

		/// <summary>
		///     Gets the player that sent the message.
		/// </summary>
		public NetworkIdentity Player { get; private set; }

		/// <summary>
		///     Serializes the message using the given writer.
		/// </summary>
		/// <param name="writer">The writer that should be used to serialize the message.</param>
		public override void Serialize(ref BufferWriter writer)
		{
			WriteIdentifier(ref writer, Player);
			writer.WriteString(Message, NetworkProtocol.ChatMessageLength);
		}

		/// <summary>
		///     Deserializes the message using the given reader.
		/// </summary>
		/// <param name="reader">The reader that should be used to deserialize the message.</param>
		public override void Deserialize(ref BufferReader reader)
		{
			Player = ReadIdentifier(ref reader);
			Message = reader.ReadString(NetworkProtocol.ChatMessageLength);
		}

		/// <summary>
		///     Dispatches the message to the given dispatcher.
		/// </summary>
		/// <param name="handler">The dispatcher that should be used to dispatch the message.</param>
		/// <param name="sequenceNumber">The sequence number of the message.</param>
		public override void Dispatch(IMessageHandler handler, uint sequenceNumber)
		{
			handler.OnPlayerChatMessage(this);
		}

		/// <summary>
		///     Creates a chat message that the server broadcasts to all players.
		/// </summary>
		/// <param name="poolAllocator">The pool allocator that should be used to allocate the message.</param>
		/// <param name="player">The player who wrote the chat message.</param>
		/// <param name="message">The message that should be sent.</param>
		public static PlayerChatMessage Create(PoolAllocator poolAllocator, NetworkIdentity player, string message)
		{
			Assert.ArgumentNotNull(poolAllocator);
			Assert.ArgumentNotNullOrWhitespace(message);
			Assert.That(Encoding.UTF8.GetByteCount(message) <= NetworkProtocol.ChatMessageLength, "Chat message is too long.");

			var chatMessage = poolAllocator.Allocate<PlayerChatMessage>();
			chatMessage.Player = player;
			chatMessage.Message = message;
			return chatMessage;
		}

		/// <summary>
		///     Returns a string that represents the message.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0}, Player={1}, Message='{2}'", MessageType, Player, Message);
		}
	}
}