using System;

namespace Pegasus.Framework.Platform.Input
{
	/// <summary>
	///   Indicates the current mode of a logical input device.
	/// </summary>
	[Flags]
	public enum InputModes
	{
		/// <summary>
		///   Indicates that a logical input device is inactive.
		/// </summary>
		None = 0,

		/// <summary>
		///   Indicates that a logical input device provides input for a console.
		/// </summary>
		Console = 1,

		/// <summary>
		///   Indicates that a logical input device provides input for a game session.
		/// </summary>
		Game = 2,

		/// <summary>
		///   Indicates that a logical input device provides input for a debug session.
		/// </summary>
		Debug = 4,

		/// <summary>
		///   Indicates that a logical input device provides input for a menu.
		/// </summary>
		Menu = 8,

		/// <summary>
		///   Indicates that a logical input device provides input for a chat input.
		/// </summary>
		Chat = 16,

		/// <summary>
		///   Indicates that a logical input device provides input for everything.
		/// </summary>
		All = Console | Game | Debug | Menu | Chat
	}
}