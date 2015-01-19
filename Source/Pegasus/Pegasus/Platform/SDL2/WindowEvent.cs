namespace Pegasus.Platform.SDL2
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	///     Provides information about a window event.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct WindowEvent
	{
		/// <summary>
		///     The type of the window event.
		/// </summary>
		public readonly WindowEventType EventType;

		/// <summary>
		///     Padding that can be ignored.
		/// </summary>
		private unsafe fixed byte _padding [3];

		/// <summary>
		///     The first part of the data associated with the event.
		/// </summary>
		public readonly int Data1;

		/// <summary>
		///     The second part of the data associated with the event.
		/// </summary>
		public readonly int Data2;
	}
}