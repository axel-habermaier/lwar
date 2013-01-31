using System;

namespace Lwar.Client.Network.Messages
{
	using System.Text;
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;

	public class ChatMessage : PooledObject<ChatMessage>, IReliableMessage
	{
		/// <summary>
		///   The chat message that the player sent.
		/// </summary>
		private string _message;

		/// <summary>
		///   The identifier of the player that sent the message.
		/// </summary>
		private Identifier _playerId;

		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="session">The game session that should be updated.</param>
		public void Process(GameSession session)
		{
		}

		/// <summary>
		///   Serializes the message into the given buffer, returning false if the message did not fit.
		/// </summary>
		/// <param name="buffer">The buffer the message should be written to.</param>
		public void Write(BufferWriter buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);

			buffer.WriteByte((byte)MessageType.Connect);
			buffer.WriteUInt32(SequenceNumber);
			buffer.WriteIdentifier(_playerId);
			buffer.WriteString(_message, Specification.MaxChatMessageLength);
		}

		/// <summary>
		///   Gets or sets the sequence number of the message.
		/// </summary>
		public uint SequenceNumber { get; set; }

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="buffer">The buffer from which the instance should be deserialized.</param>
		public static ChatMessage Create(BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);

			var message = GetInstance();
			message._playerId = buffer.ReadIdentifier();
			message._message = buffer.ReadString(Specification.MaxChatMessageLength);
			return message;
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="player">The player that entered the message that should be sent.</param>
		/// <param name="message">The message that should be sent.</param>
		public static ChatMessage Create(Identifier player, string message)
		{
			Assert.ArgumentNotNullOrWhitespace(message, () => message);
			Assert.That(Encoding.UTF8.GetByteCount(message) < Specification.MaxChatMessageLength, "Message is too long.");

			var chatMessage = GetInstance();
			chatMessage._playerId = player;
			chatMessage._message = message;
			return chatMessage;
		}
	}
}