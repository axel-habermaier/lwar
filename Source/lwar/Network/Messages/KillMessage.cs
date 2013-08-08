using System;

namespace Lwar.Network.Messages
{
	using System.Runtime.InteropServices;
	using Gameplay;

	/// <summary>
	///   Holds the payload of a Kill message.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct KillMessage
	{
		/// <summary>
		///   The player that was killed.
		/// </summary>
		public Identifier Victim;

		/// <summary>
		///   The player that scored the kill.
		/// </summary>
		public Identifier Killer;
	}
}