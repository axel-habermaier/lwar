using System;

namespace Lwar.Client.Network.Messages
{
	using System.Runtime.InteropServices;
	using Gameplay;

	/// <summary>
	///   Holds the payload of a Stats message.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct StatsMessage
	{
		/// <summary>
		///   The number of deaths of the player.
		/// </summary>
		public ushort Deaths;

		/// <summary>
		///   The number of kills scored by the player.
		/// </summary>
		public ushort Kills;

		/// <summary>
		///   The player's network latency.
		/// </summary>
		public ushort Ping;

		/// <summary>
		///   The player whose stats are updated.
		/// </summary>
		public Identifier Player;
	}
}