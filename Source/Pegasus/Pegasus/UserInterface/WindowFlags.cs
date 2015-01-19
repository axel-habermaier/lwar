namespace Pegasus.UserInterface
{
	using System;

	/// <summary>
	///     Describes a set of properties of a native window.
	/// </summary>
	[Flags]
	internal enum WindowFlags
	{
		/// <summary>
		///     Indicates that the window is in fullscreen mode with a possible display mode change.
		/// </summary>
		Fullscreen = 0x1,

		/// <summary>
		///     Indicates that the  supports OpenGL rendering.
		/// </summary>
		OpenGL = 0x2,

		/// <summary>
		///     Indicates that the  is hidden.
		/// </summary>
		Hidden = 0x8,

		/// <summary>
		///     Indicates that the  is resizable.
		/// </summary>
		Resizable = 0x20,

		/// <summary>
		///     Indicates that the  is minimized.
		/// </summary>
		Minimized = 0x40,

		/// <summary>
		///     Indicates that the  is maximized.
		/// </summary>
		Maximized = 0x80,

		/// <summary>
		///     Indicates that the has grabbed input.
		/// </summary>
		InputGrabbed = 0x100,

		/// <summary>
		///     Indicates that the  is in fullscreen mode without changing the display mode.
		/// </summary>
		FullscreenDesktop = (Fullscreen | 0x1000)
	}
}