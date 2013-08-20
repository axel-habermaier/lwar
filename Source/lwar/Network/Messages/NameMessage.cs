using System;

namespace Lwar.Network.Messages
{
	using System.Runtime.InteropServices;
	using System.Text;
	using Gameplay;
	using Pegasus;

	/// <summary>
	///   Holds the payload of a Name message.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct NameMessage
	{
		/// <summary>
		///   The new name of the player.
		/// </summary>
		public string Name;

		/// <summary>
		///   The player whose name is changed.
		/// </summary>
		public Identifier Player;

		/// <summary>
		///   Creates a message that instructs the server to change the name of the given player.
		/// </summary>
		/// <param name="player">The player whose name should be changed.</param>
		/// <param name="playerName">The new player name.</param>
		public static Message Create(Player player, string playerName)
		{
			Assert.ArgumentNotNull(player);
			Assert.ArgumentNotNullOrWhitespace(playerName);
			Assert.That(Encoding.UTF8.GetByteCount(playerName) <= Specification.PlayerNameLength, "Player name is too long.");

			return new Message
			{
				Type = MessageType.Name,
				Name = new NameMessage
				{
					Player = player.Identifier,
					Name = playerName
				}
			};
		}
	}
}