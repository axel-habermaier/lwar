using System;

namespace Pegasus.Framework.Platform.Input
{
    /// <summary>
    ///   Provides information about key press and release events.
    /// </summary>
    public struct KeyEventArgs
    {
		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name="key">The key that was pressed or released.</param>
		/// <param name="scanCode">The key's scan code.</param>
        public KeyEventArgs(Key key, int scanCode)
            : this()
        {
            Key = key;
            ScanCode = scanCode;
        }

        /// <summary>
        ///   Gets the key that was pressed or released. The key depends on the keyboard layout.
        /// </summary>
        public Key Key { get; private set; }

        /// <summary>
        ///   Gets the key's scan code. The scan code is independent of the keyboard layout but may differ between
        ///   operating systems.
        /// </summary>
        public int ScanCode { get; private set; }
    }
}