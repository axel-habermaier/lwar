using System;

namespace Lwar.Client.Network.Messages
{
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;

	public class ChatMessage : Message<ChatMessage>, IReliableMessage
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
		public override void Process(GameSession session)
		{
			Assert.ArgumentNotNull(session, () => session);
			var player = session.Players[_playerId];

			Log.Info("{0}: {1}", player.Name, _message);
		}

		/// <summary>
		///   Writes the message into the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the message should be written to.</param>
		public override bool Write(BufferWriter buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			return buffer.TryWrite(this, (b, m) =>
				{
					b.WriteByte((byte)MessageType.Chat);
					b.WriteUInt32(m.SequenceNumber);
					b.WriteIdentifier(m._playerId);
					b.WriteString(m._message, Specification.MaximumChatMessageLength);
				});
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
			return Deserialize(buffer, (b, m) =>
				{
					m._playerId = b.ReadIdentifier();
					m._message = b.ReadString(Specification.MaximumChatMessageLength);
				});
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="player">The player that entered the message that should be sent.</param>
		/// <param name="message">The message that should be sent.</param>
		public static ChatMessage Create(Identifier player, string message)
		{
			Assert.ArgumentNotNullOrWhitespace(message, () => message);

			var chatMessage = GetInstance();
			chatMessage._playerId = player;
			chatMessage._message = message.TruncateUtf8(Specification.MaximumChatMessageLength);
			;
			return chatMessage;
		}
	}
}