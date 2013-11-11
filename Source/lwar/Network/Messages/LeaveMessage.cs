namespace Lwar.Network.Messages
{
	using System;
	using System.Runtime.InteropServices;
	using Gameplay;

	/// <summary>
	///   Holds the payload of a Leave message.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct LeaveMessage
	{
		/// <summary>
		///   The player that has left the game session.
		/// </summary>
		public Identifier Player;

		/// <summary>
		///   The reason explaining why the player has left the game session.
		/// </summary>
		public LeaveReason Reason;
	}
}