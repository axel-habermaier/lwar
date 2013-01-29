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
	}
}