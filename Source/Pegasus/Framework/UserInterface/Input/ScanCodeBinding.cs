namespace Pegasus.Framework.UserInterface.Input
{
	using System;
	using Platform.Logging;

	/// <summary>
	///     Represents an input binding that is triggered by a key event, identifying the key by its scan code.
	/// </summary>
	public sealed class ScanCodeBinding : InputBinding
	{
		/// <summary>
		///     The key modifiers that must be pressed for the binding to trigger.
		/// </summary>
		private KeyModifiers _modifiers;

		/// <summary>
		///     The scan code of the key that activates the binding.
		/// </summary>
		private int _scanCode;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public ScanCodeBinding()
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="scanCode">The scan code of the key that should activate the binding.</param>
		/// <param name="method">The name of the method that should be invoked when the binding is triggered.</param>
		/// <param name="modifiers">The key modifiers that must be pressed for the binding to trigger.</param>
		public ScanCodeBinding(int scanCode, string method, KeyModifiers modifiers = KeyModifiers.None)
		{
			Assert.ArgumentNotNullOrWhitespace(method);

			ScanCode = scanCode;
			Modifiers = modifiers;
			Method = method;
		}

		/// <summary>
		///     Gets or sets the scan code of the key that activates the binding.
		/// </summary>
		public int ScanCode
		{
			get { return _scanCode; }
			set
			{
				Assert.NotSealed(this);
				_scanCode = value;
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
					return args.RoutedEvent == UIElement.KeyDownEvent && keyEventArgs.ScanCode == _scanCode && keyEventArgs.Modifiers == Modifiers;
				case TriggerMode.Deactivated:
					return args.RoutedEvent == UIElement.KeyUpEvent && (keyEventArgs.ScanCode == _scanCode || keyEventArgs.Modifiers != Modifiers);
				default:
					Log.Die("Unknown trigger mode.");
					return false;
			}
		}
	}
}