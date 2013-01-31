using System;

namespace Lwar.Client.Network
{
	/// <summary>
	///   Provides access to lwar network specification constants.
	/// </summary>
	public static class Specification
	{
		/// <summary>
		///   The maximum allowed byte length of an UTF8-encoded player name.
		/// </summary>
		public const int MaxPlayerNameLength = 32;

		/// <summary>
		///   The maximum allowed byte length of an UTF8-encoded chat message.
		/// </summary>
		public const int MaxChatMessageLength = 128;

		/// <summary>
		///   The maximum allowed packet size in bytes.
		/// </summary>
		public const int MaxPacketSize = 512;

		/// <summary>
		///   The size of the packet header in bytes.
		/// </summary>
		public const int HeaderSize = 12;

		/// <summary>
		///   The maximum allowed number of concurrently active players.
		/// </summary>
		public const int MaxPlayers = 8;

		/// <summary>
		///   The frequency of the update input message that is sent to the server in Hz.
		/// </summary>
		public const int UpdateInputFrequency = 30;
	}
}