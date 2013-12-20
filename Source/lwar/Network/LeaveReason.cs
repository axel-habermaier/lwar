namespace Lwar.Network
{
	using System;

	/// <summary>
	///     Indicates why a player has left a game session.
	/// </summary>
	public enum LeaveReason
	{
		/// <summary>
		///     Indicates that the player quit the game session.
		/// </summary>
		Quit = 1,

		/// <summary>
		///     Indicates that the connection to the client has been dropped.
		/// </summary>
		ConnectionDropped = 2,

		/// <summary>
		///     Indicates that a network specification violation has occurred.
		/// </summary>
		Misbehaved = 3
	}
}