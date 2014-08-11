namespace Pegasus.Framework.UserInterface.Input
{
	using System;

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
		///     The type of the key event that triggers the binding.
		/// </summary>
		private KeyTriggerType _trigger = KeyTriggerType.Pressed;

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
		///     Gets or sets the type of the key event that triggers the binding.
		/// </summary>
		public KeyTriggerType Trigger
		{
			get { return _trigger; }
			set
			{
				Assert.ArgumentInRange(value);
				Assert.NotSealed(this);

				_trigger = value;
			}
		}

		/// <summary>
		///     Checks whether the given event triggers the input binding.
		/// </summary>
		/// <param name="args">The arguments of the event that should be checked.</param>
		protected override bool IsTriggered(RoutedEventArgs args)
		{
			return IsTriggered(args, _key, _modifiers, _trigger);
		}
	}
}