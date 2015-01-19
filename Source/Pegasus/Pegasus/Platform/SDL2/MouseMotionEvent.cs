namespace Pegasus.Platform.SDL2
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	///     Provides information about a mouse movement.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct MouseMotionEvent
	{
		/// <summary>
		///     Indicates which mouse was moved.
		/// </summary>
		public readonly uint Which;

		/// <summary>
		///     The state of the mouse button that raised the event.
		/// </summary>
		public readonly byte State;

		/// <summary>
		///     Padding that can be ignored.
		/// </summary>
		private unsafe fixed byte _padding [3];

		/// <summary>
		///     The absolute X position of the mouse.
		/// </summary>
		public readonly int X;

		/// <summary>
		///     The absolute Y position of the mouse.
		/// </summary>
		public readonly int Y;

		/// <summary>
		///     The relative X position of the mouse.
		/// </summary>
		public readonly int RelativeX;

		/// <summary>
		///     The relative Y position of the mouse.
		/// </summary>
		public readonly int RelativeY;
	}
}