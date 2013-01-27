using System;

namespace Lwar.Client.Network
{
	/// <summary>
	///   Identifies the type of a message.
	/// </summary>
	public enum MessageType
	{
		Connect = 1,
		Join = 2,
		Leave = 3,
		Chat = 4,
		Input = 5,
		Add = 6,
		Remove = 7,
		Update = 8
	}
}