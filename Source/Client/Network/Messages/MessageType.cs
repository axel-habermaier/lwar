using System;

namespace Lwar.Client.Network.Messages
{
	/// <summary>
	///   Identifies the type of a message.
	/// </summary>
	public enum MessageType
	{
		Connect = 101,
		Disconnect = 102,
		AddPlayer = 103,
		RemovePlayer = 104,
		ChatMessage = 105,
		AddEntity = 106,
		RemoveEntity = 107,
		ChangePlayerState = 108,
		ChangePlayerName = 109,
		Synced = 110,
		ServerFull = 111,
		UpdatePlayerStats = 201,
		UpdateEntity = 202,
		UpdateClientInput = 203
	}
}