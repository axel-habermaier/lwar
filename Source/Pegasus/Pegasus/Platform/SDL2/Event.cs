namespace Pegasus.Platform.SDL2
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	///     Provides information about an SDL event.
	/// </summary>
	[StructLayout(LayoutKind.Explicit, Size = 56)]
	internal struct Event
	{
		/// <summary>
		///     The type of the event.
		/// </summary>
		[FieldOffset(0)]
		public readonly EventType EventType;

		/// <summary>
		///     The timestamp of the event.
		/// </summary>
		[FieldOffset(4)]
		public readonly uint Timestamp;

		/// <summary>
		///     The ID of the window the event is raised for.
		/// </summary>
		[FieldOffset(8)]
		public readonly uint WindowID;

		/// <summary>
		///     Provides information about window events.
		/// </summary>
		[FieldOffset(12)]
		public WindowEvent Window;

		/// <summary>
		///     Provides information about keyboard events.
		/// </summary>
		[FieldOffset(12)]
		public KeyboardEvent Key;

		/// <summary>
		///     Provides information about text input events.
		/// </summary>
		[FieldOffset(12)]
		public TextInputEvent Text;

		/// <summary>
		///     Provides information about mouse motion events.
		/// </summary>
		[FieldOffset(12)]
		public MouseMotionEvent Motion;

		/// <summary>
		///     Provides information about mouse button events.
		/// </summary>
		[FieldOffset(12)]
		public MouseButtonEvent Button;

		/// <summary>
		///     Provides information about mouse wheel events.
		/// </summary>
		[FieldOffset(12)]
		public MouseWheelEvent Wheel;
	}
}