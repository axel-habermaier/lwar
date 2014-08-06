namespace Pegasus.Framework.UserInterface.Input
{
	using System;

	/// <summary>
	///     Provides information about key press and release events.
	/// </summary>
	public class KeyEventArgs : RoutedEventArgs<KeyEventArgs>
	{
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
		///     Gets the state of the key.
		/// </summary>
		public InputState State { get; private set; }

		/// <summary>
		///     Initializes a cached instance.
		/// </summary>
		/// <param name="key">The key that was pressed or released.</param>
		/// <param name="scanCode">The key's scan code.</param>
		/// <param name="state">The state of the key.</param>
		internal static KeyEventArgs Create(Key key, int scanCode, InputState state)
		{
			Assert.ArgumentInRange(key);

			CachedInstance.Key = key;
			CachedInstance.ScanCode = scanCode;
			CachedInstance.State = state;

			return CachedInstance;
		}
	}
}