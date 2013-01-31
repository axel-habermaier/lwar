using System;

namespace Lwar.Client.Network
{
	/// <summary>
	///   Identifies the type of a message.
	/// </summary>
	public enum MessageType
	{
		Connect = 1,
		Disconnect = 2,
		AddPlayer = 3,
		RemovePlayer = 4,
		ChatMessage = 5,
		AddEntity = 6,
		RemoveEntity = 7,
		ChangePlayerState = 8,
		ChangePlayerName = 9,
		Synced = 10,
		UpdatePlayerStats = 101,
		UpdateEntity = 102,
		UpdateClientInput = 103,
		ServerFull = 104
	}
}