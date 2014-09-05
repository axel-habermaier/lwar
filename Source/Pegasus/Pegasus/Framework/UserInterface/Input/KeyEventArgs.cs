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
		///     The state of the key that was pressed or released.
		/// </summary>
		private InputState _state;

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
		///     Gets a value indicating whether the key or button is currently being pressed down.
		/// </summary>
		public bool IsPressed
		{
			get { return _state.IsPressed; }
		}

		/// <summary>
		///     Gets a value indicating whether the key or button was pressed during the current frame. WentDown is
		///     only true during the single frame when IsPressed changed from false to true.
		/// </summary>
		public bool WentDown
		{
			get { return _state.WentDown; }
		}

		/// <summary>
		///     Gets a value indicating whether the key or button was released during the current frame. WentUp is
		///     only true during the single frame when IsPressed changed from true to false.
		/// </summary>
		public bool WentUp
		{
			get { return _state.WentUp; }
		}

		/// <summary>
		///     Gets a value indicating whether a key or button repeat event occurred. IsRepeated is also true
		///     when the key or button is pressed, i.e., when WentDown is true.
		/// </summary>
		public bool IsRepeated
		{
			get { return _state.IsRepeated; }
		}

		/// <summary>
		///     Gets the set of key modifiers that were pressed when the event was raised.
		/// </summary>
		public KeyModifiers Modifiers { get; private set; }

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
			CachedInstance.Key = key;
			CachedInstance.ScanCode = scanCode;
			CachedInstance._state = state;
			CachedInstance.Modifiers = keyboard.GetModifiers();

			return CachedInstance;
		}
	}
}