using System;

namespace Pegasus.Framework.Network
{
	/// <summary>
	///   Provides configuration constants for packets.
	/// </summary>
	public static class Packet
	{
		/// <summary>
		///   The maximum size of a packet. The value is chosen such that packet fragmentation is very unlikely.
		/// </summary>
		public const int MaxSize = 512;
	}
}