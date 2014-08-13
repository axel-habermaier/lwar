namespace Pegasus.Framework.UserInterface.Controls
{
	using System;

	/// <summary>
	///     Indicates the whether the window is minimized, maximized, or neither minimized nor maximized.
	/// </summary>
	public enum WindowMode
	{
		/// <summary>
		///     Indicates that the window is neither minimized nor maximized.
		/// </summary>
		Normal = 1,

		/// <summary>
		///     Indicates that the window is maximized, filling the entire screen.
		/// </summary>
		Maximized = 2,

		/// <summary>
		///     Indicates that the window is minimized and invisible.
		/// </summary>
		Minimized = 3,

		/// <summary>
		///     Indicates that the window is in borderless fullscreen mode, filling the entire screen.
		/// </summary>
		Fullscreen = 4
	}
}