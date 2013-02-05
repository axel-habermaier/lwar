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
		Join = 3,
		Leave = 4,
		Chat = 5,
		Add = 6,
		Remove = 7,
		Selection = 8,
		Name = 9,
		Synced = 10,
		Stats = 101,
		Update = 102,
		Input = 103,
		Full = 104,
		Collision = 105
	}
}