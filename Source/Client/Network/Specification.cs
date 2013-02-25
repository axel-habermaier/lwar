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
		public const int MaximumPlayerNameLength = 32;

		/// <summary>
		///   The maximum allowed byte length of an UTF8-encoded chat message.
		/// </summary>
		public const int MaximumChatMessageLength = 128;

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
		///   The frequency in Hz that determines how often the user input is sent to the server.
		/// </summary>
		public const int InputUpdateFrequency = 30;

		/// <summary>
		///   The default server port.
		/// </summary>
		public const ushort DefaultServerPort = 32422;
	}
}