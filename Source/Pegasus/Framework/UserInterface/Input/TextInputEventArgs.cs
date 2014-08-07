namespace Pegasus.Framework.UserInterface.Input
{
	using System;

	/// <summary>
	///     Provides information about text input..
	/// </summary>
	public class TextInputEventArgs : RoutedEventArgs
	{
		/// <summary>
		///     A cached instance of the event argument class that should be used to reduce the pressure on the garbage collector.
		/// </summary>
		private static readonly TextInputEventArgs CachedInstance = new TextInputEventArgs();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private TextInputEventArgs()
		{
		}

		/// <summary>
		///     Gets the character that was entered.
		/// </summary>
		public char Character { get; private set; }

		/// <summary>
		///     Gets the scan code of the key that generated the character. The scan code is independent of the keyboard layout but
		///     may differ between operating systems.
		/// </summary>
		public int ScanCode { get; private set; }

		/// <summary>
		///     Initializes a cached instance.
		/// </summary>
		/// <param name="character">The character that was entered.</param>
		/// <param name="scanCode">The key's scan code.</param>
		internal static TextInputEventArgs Create(char character, int scanCode)
		{
			CachedInstance.Reset();
			CachedInstance.Character = character;
			CachedInstance.ScanCode = scanCode;

			return CachedInstance;
		}
	}
}