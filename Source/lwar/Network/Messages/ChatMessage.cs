using System;

namespace Lwar.Network.Messages
{
	using System.Runtime.InteropServices;
	using System.Text;
	using Gameplay;
	using Pegasus.Framework;

	/// <summary>
	///   Holds the payload of a Chat message.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct ChatMessage
	{
		/// <summary>
		///   The message sent by the player.
		/// </summary>
		public string Message;

		/// <summary>
		///   The player that sent the message.
		/// </summary>
		public Identifier Player;

		/// <summary>
		///   Creates a chat message that the server broadcasts to all players.
		/// </summary>
		/// <param name="player">The player who wrote the chat message.</param>
		/// <param name="message">The message that should be sent.</param>
		public static Message Create(Player player, string message)
		{
			Assert.ArgumentNotNull(player);
			Assert.ArgumentNotNullOrWhitespace(message);
			Assert.That(Encoding.UTF8.GetByteCount(message) <= Specification.ChatMessageLength, "Chat message is too long.");

			return new Message
			{
				Type = MessageType.Chat,
				Chat = new ChatMessage
				{
					Player = player.Id,
					Message = message
				}
			};
		}
	}
}