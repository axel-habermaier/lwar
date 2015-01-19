namespace Pegasus.Platform.SDL2
{
	using System;

	/// <summary>
	///     Describes the type of an SDL window event.
	/// </summary>
	internal enum WindowEventType : byte
	{
		None,
		Shown,
		Hidden,
		Exposed,
		Moved,
		Resized,
		SizeChanged,
		Minimized,
		Maximized,
		Restored,
		Enter,
		Leave,
		FocusGained,
		FocusLost,
		Close
	}
}