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
		Kill = 11,
		Stats = 101,
		Input = 103,
		Full = 104,
		Collision = 105,
		Update = 110,
		UpdatePosition = 111,
		UpdateRay = 112,
		UpdateCircle = 113
	}
}