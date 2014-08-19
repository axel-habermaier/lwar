namespace Pegasus.Framework.UserInterface.Input
{
	using System;
	using System.Text;

	/// <summary>
	///     Represents a configurable input.
	/// </summary>
	public struct ConfigurableInput
	{
		/// <summary>
		///     A cached string builder instance.
		/// </summary>
		private static readonly StringBuilder Builder = new StringBuilder();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="key">The key that triggers the input.</param>
		/// <param name="modifiers">The modifier keys that must be pressed for the input to trigger.</param>
		public ConfigurableInput(Key key, KeyModifiers modifiers)
			: this()
		{
			Key = key;
			Modifiers = modifiers;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="T:System.Object" /> class.
		/// </summary>
		/// <param name="mouseButton">The mouse button that triggers the input.</param>
		/// <param name="modifiers">The modifier keys that must be pressed for the input to trigger.</param>
		public ConfigurableInput(MouseButton mouseButton, KeyModifiers modifiers)
			: this()
		{
			MouseButton = mouseButton;
			Modifiers = modifiers;
		}

		/// <summary>
		///     Gets the key that triggers the input, or null if a mouse button triggers the input.
		/// </summary>
		public Key? Key { get; private set; }

		/// <summary>
		///     Gets the mouse button that triggers the input, or null if a key triggers the input.
		/// </summary>
		public MouseButton? MouseButton { get; private set; }

		/// <summary>
		///     Gets the modifier keys that must be pressed for the input to trigger.
		/// </summary>
		public KeyModifiers Modifiers { get; private set; }

		/// <summary>
		///     Creates a copy of the current instance with the given modifiers.
		/// </summary>
		/// <param name="modifiers">The modifier keys that must be pressed for the input to trigger.</param>
		public ConfigurableInput WithModifiers(KeyModifiers modifiers)
		{
			return new ConfigurableInput { Key = Key, MouseButton = MouseButton, Modifiers = modifiers };
		}

		/// <summary>
		///     Implicitly creates a configurable input for the given key.
		/// </summary>
		/// <param name="key">The key the configurable input should be created for.</param>
		public static implicit operator ConfigurableInput(Key key)
		{
			Assert.ArgumentInRange(key);
			return new ConfigurableInput(key, KeyModifiers.None);
		}

		/// <summary>
		///     Implicitly creates a configurable input for the given key.
		/// </summary>
		/// <param name="mouseButton">The mouse button the configurable input should be created for.</param>
		public static implicit operator ConfigurableInput(MouseButton mouseButton)
		{
			Assert.ArgumentInRange(mouseButton);
			return new ConfigurableInput(mouseButton, KeyModifiers.None);
		}

		/// <summary>
		///     Returns a string representation of the configurable input.
		/// </summary>
		public override string ToString()
		{
			Builder.Clear();
			Builder.Append("[");

			if (Key != null)
				Builder.Append("Key." + Key.Value);

			if (MouseButton != null)
				Builder.Append("Mouse." + MouseButton.Value);

			if ((Modifiers & KeyModifiers.Alt) == KeyModifiers.Alt)
				Builder.Append("+Alt");

			if ((Modifiers & KeyModifiers.Shift) == KeyModifiers.Shift)
				Builder.Append("+Shift");

			if ((Modifiers & KeyModifiers.Control) == KeyModifiers.Control)
				Builder.Append("+Control");

			Builder.Append("]");
			return Builder.ToString();
		}
	}
}