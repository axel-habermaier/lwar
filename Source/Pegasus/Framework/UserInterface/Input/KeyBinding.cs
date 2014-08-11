namespace Pegasus.Framework.UserInterface.Input
{
	using System;
	using Platform.Logging;

	/// <summary>
	///     Represents an input binding that is triggered by a key down event.
	/// </summary>
	public sealed class KeyBinding : InputBinding
	{
		/// <summary>
		///     The key that activates the binding.
		/// </summary>
		private Key _key;

		/// <summary>
		///     The key modifiers that must be pressed for the binding to trigger.
		/// </summary>
		private KeyModifiers _modifiers;

		/// <summary>
		///     Gets or sets the key that activates the binding.
		/// </summary>
		public Key Key
		{
			get { return _key; }
			set
			{
				Assert.ArgumentInRange(value);
				Assert.NotSealed(this);

				_key = value;
			}
		}

		/// <summary>
		///     Gets or sets the key modifiers that must be pressed for the binding to trigger.
		/// </summary>
		public KeyModifiers Modifiers
		{
			get { return _modifiers; }
			set
			{
				Assert.NotSealed(this);
				_modifiers = value;
			}
		}

		/// <summary>
		///     Checks whether the given event triggers the input binding.
		/// </summary>
		/// <param name="args">The arguments of the event that should be checked.</param>
		protected override bool IsTriggered(RoutedEventArgs args)
		{
			var keyEventArgs = args as KeyEventArgs;
			if (keyEventArgs == null)
				return false;

			switch (TriggerMode)
			{
				case TriggerMode.Activated:
					return args.RoutedEvent == UIElement.KeyDownEvent && keyEventArgs.Key == Key && keyEventArgs.Modifiers == Modifiers;
				case TriggerMode.Deactivated:
					return args.RoutedEvent == UIElement.KeyUpEvent && (keyEventArgs.Key == Key || keyEventArgs.Modifiers != Modifiers);
				default:
					Log.Die("Unknown trigger mode.");
					return false;
			}
		}
	}
}