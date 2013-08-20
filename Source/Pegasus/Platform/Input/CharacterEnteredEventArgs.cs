using System;

namespace Pegasus.Platform.Input
{
	/// <summary>
	///   Provides information about character entered event.
	/// </summary>
	public struct CharacterEnteredEventArgs
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="character">The character that was entered.</param>
		/// <param name="scanCode">The key's scan code.</param>
		public CharacterEnteredEventArgs(char character, int scanCode)
			: this()
		{
			Character = character;
			ScanCode = scanCode;
		}

		/// <summary>
		///   Gets the character that was entered.
		/// </summary>
		public char Character { get; private set; }

		/// <summary>
		///   Gets the scan code of the key that generated the character. The scan code is independent of the keyboard layout but
		///   may differ between operating systems.
		/// </summary>
		public int ScanCode { get; private set; }
	}
}