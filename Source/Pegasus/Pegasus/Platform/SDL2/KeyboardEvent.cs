namespace Pegasus.Platform.SDL2
{
	using System;
	using System.Runtime.InteropServices;
	using UserInterface.Input;

	/// <summary>
	///     Provides information about a keyboard event.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct KeyboardEvent
	{
		/// <summary>
		///     The state of the key.
		/// </summary>
		public readonly byte State;

		/// <summary>
		///     Indicates whether the key was pressed repeatedly.
		/// </summary>
		public readonly byte Repeat;

		/// <summary>
		///     Padding that can be ignored.
		/// </summary>
		private unsafe fixed byte _padding [2];

		/// <summary>
		///     The scan code of the key that raised the event.
		/// </summary>
		public readonly ScanCode ScanCode;

		/// <summary>
		///     The key that raised the event.
		/// </summary>
		public readonly Key Key;

		/// <summary>
		///     The modifiers that are pressed.
		/// </summary>
		public readonly ushort Modifiers;
	}
}