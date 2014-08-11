namespace Pegasus.Framework.UserInterface.Input
{
	using System;
	using Scripting;

	/// <summary>
	///     Represents an input binding that is triggered by a key down event, with the specific key being configurable by a cvar.
	/// </summary>
	public sealed class CvarKeyBinding : InputBinding
	{
		/// <summary>
		///     The configurable key that activates the binding.
		/// </summary>
		private Cvar<Key> _cvar;

		/// <summary>
		///     The key modifiers that must be pressed for the binding to trigger.
		/// </summary>
		private KeyModifiers _modifiers;

		/// <summary>
		///     The type of the key event that triggers the binding.
		/// </summary>
		private KeyTriggerType _trigger = KeyTriggerType.Pressed;

		/// <summary>
		///     Gets or sets the configurable key that activates the binding.
		/// </summary>
		public Cvar<Key> Cvar
		{
			get { return _cvar; }
			set
			{
				Assert.ArgumentNotNull(value);
				Assert.NotSealed(this);

				_cvar = value;
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
			return IsTriggered(args, _cvar.Value, _modifiers, _trigger);
		}
	}
}