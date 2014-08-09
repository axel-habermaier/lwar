namespace Pegasus.Framework.UserInterface.Input
{
	using System;

	/// <summary>
	///     Provides information about key press and release events.
	/// </summary>
	public sealed class KeyEventArgs : RoutedEventArgs
	{
		/// <summary>
		///     A cached instance of the event argument class that should be used to reduce the pressure on the garbage collector.
		/// </summary>
		private static readonly KeyEventArgs CachedInstance = new KeyEventArgs();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private KeyEventArgs()
		{
		}

		/// <summary>
		///     Gets the key that was pressed or released. The key depends on the keyboard layout.
		/// </summary>
		public Key Key { get; private set; }

		/// <summary>
		///     Gets the key's scan code. The scan code is independent of the keyboard layout but may differ between
		///     operating systems.
		/// </summary>
		public int ScanCode { get; private set; }

		/// <summary>
		///     Gets the state of the key that was pressed or released.
		/// </summary>
		public InputState State { get; private set; }

		/// <summary>
		///     Gets the keyboard device that raised the event.
		/// </summary>
		public Keyboard Keyboard { get; private set; }

		/// <summary>
		///     Initializes a cached instance.
		/// </summary>
		/// <param name="keyboard">The keyboard device that raised the event.</param>
		/// <param name="key">The key that was pressed or released.</param>
		/// <param name="scanCode">The key's scan code.</param>
		/// <param name="state">The state of the key.</param>
		internal static KeyEventArgs Create(Keyboard keyboard, Key key, int scanCode, InputState state)
		{
			Assert.ArgumentNotNull(keyboard);
			Assert.ArgumentInRange(key);

			CachedInstance.Reset();
			CachedInstance.Keyboard = keyboard;
			CachedInstance.Key = key;
			CachedInstance.ScanCode = scanCode;
			CachedInstance.State = state;

			return CachedInstance;
		}
	}
}