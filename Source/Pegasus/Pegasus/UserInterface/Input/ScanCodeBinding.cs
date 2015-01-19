namespace Pegasus.UserInterface.Input
{
	using System;
	using Platform.Logging;
	using Utilities;

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
		private ScanCode _scanCode;

		/// <summary>
		///     Indicates whether the binding is also triggered when the OS reports a repeated key press.
		/// </summary>
		private bool _triggerOnRepeat = true;

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
		/// <param name="triggerOnRepeat">
		///     Indicates whether the binding should also be triggered when the OS reports a repeated key press.
		/// </param>
		public ScanCodeBinding(ScanCode scanCode, string method, KeyModifiers modifiers = KeyModifiers.None, bool triggerOnRepeat = true)
		{
			Assert.ArgumentNotNullOrWhitespace(method);

			ScanCode = scanCode;
			Modifiers = modifiers;
			Method = method;
			TriggerOnRepeat = triggerOnRepeat;
		}

		/// <summary>
		///     Gets or sets the scan code of the key that activates the binding.
		/// </summary>
		public ScanCode ScanCode
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
					var downEvent = Preview ? UIElement.PreviewKeyDownEvent : UIElement.KeyDownEvent;
					if (args.RoutedEvent == downEvent && keyEventArgs.ScanCode == _scanCode && keyEventArgs.Modifiers == Modifiers)
						return keyEventArgs.WentDown || (keyEventArgs.IsRepeated && _triggerOnRepeat);
					return false;
				case TriggerMode.Deactivated:
					var upEvent = Preview ? UIElement.PreviewKeyUpEvent : UIElement.KeyUpEvent;
					return args.RoutedEvent == upEvent && (keyEventArgs.ScanCode == _scanCode || keyEventArgs.Modifiers != Modifiers);
				default:
					Log.Die("Unknown trigger mode.");
					return false;
			}
		}
	}
}