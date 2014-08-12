namespace Pegasus.Framework.UserInterface.Input
{
	using System;
	using Platform.Logging;

	/// <summary>
	///     Represents an input binding that is triggered by a key event.
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
		///     Indicates whether the binding is also triggered when the OS reports a repeated key press.
		/// </summary>
		private bool _triggerOnRepeat = true;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public KeyBinding()
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="key">The key that should activate the binding.</param>
		/// <param name="method">The name of the method that should be invoked when the binding is triggered.</param>
		/// <param name="modifiers">The key modifiers that must be pressed for the binding to trigger.</param>
		public KeyBinding(Key key, string method, KeyModifiers modifiers = KeyModifiers.None)
		{
			Assert.ArgumentNotNullOrWhitespace(method);

			Key = key;
			Modifiers = modifiers;
			Method = method;
		}

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
		///     Gets or sets a value indicating whether the binding is also be triggered when the OS reports a repeated key press.
		/// </summary>
		public bool TriggerOnRepeat
		{
			get { return _triggerOnRepeat; }
			set
			{
				Assert.NotSealed(this);
				_triggerOnRepeat = value;
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
					if (args.RoutedEvent == UIElement.KeyDownEvent && keyEventArgs.Key == Key && keyEventArgs.Modifiers == Modifiers)
						return keyEventArgs.WentDown || (keyEventArgs.IsRepeated && _triggerOnRepeat);
					return false;
				case TriggerMode.Deactivated:
					return args.RoutedEvent == UIElement.KeyUpEvent && (keyEventArgs.Key == Key || keyEventArgs.Modifiers != Modifiers);
				default:
					Log.Die("Unknown trigger mode.");
					return false;
			}
		}
	}
}