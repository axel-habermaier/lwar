namespace Pegasus.Platform.SDL2
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	///     Provides information about a text input event.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct TextInputEvent
	{
		/// <summary>
		///     The maximum length of the entered text.
		/// </summary>
		private const int TextSize = 32;

		/// <summary>
		///     The text that was entered.
		/// </summary>
		public unsafe fixed byte Text [TextSize];
	}
}