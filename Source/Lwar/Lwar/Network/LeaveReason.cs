namespace Lwar.Network
{
	using System;

	/// <summary>
	///     Indicates why a player has left a game session.
	/// </summary>
	public enum LeaveReason
	{
		/// <summary>
		///     Indicates that the reason is unknown.
		/// </summary>
		Unknown = 0,

		/// <summary>
		///     Indicates that the player has disconnected from the game session.
		/// </summary>
		Disconnect = 1,

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