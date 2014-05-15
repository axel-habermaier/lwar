namespace Lwar.Network.Messages
{
	using System;
	using System.Runtime.InteropServices;
	using Gameplay;

	/// <summary>
	///     Holds the payload of a Join message.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct JoinMessage
	{
		/// <summary>
		///     The name of the player that joined.
		/// </summary>
		public string Name;

		/// <summary>
		///     Indicates whether the joined player is the local player.
		/// </summary>
		public bool IsLocalPlayer;

		/// <summary>
		///     The player that joined the game session.
		/// </summary>
		public Identifier Player;
	}
}