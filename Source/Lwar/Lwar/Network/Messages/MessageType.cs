namespace Lwar.Network.Messages
{
	using System;

	/// <summary>
	///     Identifies the type of a message.
	/// </summary>
	internal enum MessageType : byte
	{
		ClientConnect = 1,
		ClientRejected = 107,
		ClientSynced = 10,

		Disconnect = 106,

		EntityAdd = 6,
		EntityCollision = 105,
		EntityRemove = 7,

		PlayerChat = 5,
		PlayerInput = 103,
		PlayerJoin = 3,
		PlayerKill = 11,
		PlayerLeave = 4,
		PlayerLoadout = 8,
		PlayerName = 9,
		PlayerStats = 101,

		UpdateCircle = 113,
		UpdatePosition = 111,
		UpdateRay = 112,
		UpdateShip = 114,
		UpdateTransform = 110,
	}
}