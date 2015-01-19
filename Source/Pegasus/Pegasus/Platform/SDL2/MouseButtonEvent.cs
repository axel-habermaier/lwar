namespace Pegasus.Platform.SDL2
{
	using System;
	using System.Runtime.InteropServices;
	using UserInterface.Input;

	/// <summary>
	///     Provides information about a mouse button event.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct MouseButtonEvent
	{
		/// <summary>
		///     Indicates which mouse the event is raised for.
		/// </summary>
		public readonly uint Which;

		/// <summary>
		///     The mouse button the event was raised for.
		/// </summary>
		public readonly MouseButton Button;

		/// <summary>
		///     The state of the mouse button.
		/// </summary>
		public readonly byte State;

		/// <summary>
		///     The click count of the mouse button. For instance, a click count of 2 indicates a double click.
		/// </summary>
		public readonly byte Clicks;

		/// <summary>
		///     Padding that can be ignored.
		/// </summary>
		private readonly byte _padding;

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