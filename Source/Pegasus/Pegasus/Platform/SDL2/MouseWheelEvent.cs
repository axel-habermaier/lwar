namespace Pegasus.Platform.SDL2
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	///     Provides information about mouse wheel movement.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct MouseWheelEvent
	{
		/// <summary>
		///     Indicates the mouse the event is raised for.
		/// </summary>
		public readonly uint Which;

		/// <summary>
		///     The X position of the mouse.
		/// </summary>
		public readonly int X;

		/// <summary>
		///     The Y position of the mouse.
		/// </summary>
		public readonly int Y;
	}
}