namespace Pegasus.Framework.UserInterface.Input
{
	using System;
	using Platform.Logging;
	using Scripting;

	/// <summary>
	///     Represents a configurable input binding.
	/// </summary>
	public sealed class ConfigurableBinding : InputBinding
	{
		/// <summary>
		///     The configurable input cvar that triggers the binding.
		/// </summary>
		private Cvar<ConfigurableInput> _cvar;

		/// <summary>
		///     Gets or sets the configurable input cvar that triggers the binding.
		/// </summary>
		public Cvar<ConfigurableInput> Cvar
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
		///     Checks whether the given event triggers the input binding.
		/// </summary>
		/// <param name="args">The arguments of the event that should be checked.</param>
		protected override bool IsTriggered(RoutedEventArgs args)
		{
			if (Cvar.Value.Key != null)
			{
				var keyEventArgs = args as KeyEventArgs;
				if (keyEventArgs == null)
					return false;

				switch (TriggerMode)
				{
					case TriggerMode.Activated:
						var downEvent = Preview ? UIElement.PreviewKeyDownEvent : UIElement.KeyDownEvent;
						return args.RoutedEvent == downEvent && keyEventArgs.Key == Cvar.Value.Key &&
							   keyEventArgs.Modifiers == Cvar.Value.Modifiers;
					case TriggerMode.Deactivated:
						var upEvent = Preview ? UIElement.PreviewKeyUpEvent : UIElement.KeyUpEvent;
						return args.RoutedEvent == upEvent &&
							   (keyEventArgs.Key == Cvar.Value.Key || keyEventArgs.Modifiers != Cvar.Value.Modifiers);
					default:
						Log.Die("Unknown trigger mode.");
						return false;
				}
			}

			if (Cvar.Value.MouseButton != null)
			{
				var mouseEventArgs = args as MouseButtonEventArgs;
				if (mouseEventArgs == null)
					return false;

				switch (TriggerMode)
				{
					case TriggerMode.Activated:
						var downEvent = Preview ? UIElement.PreviewMouseDownEvent : UIElement.MouseDownEvent;
						return args.RoutedEvent == downEvent &&
							   (mouseEventArgs.Button == Cvar.Value.MouseButton && mouseEventArgs.Modifiers == Cvar.Value.Modifiers);
					case TriggerMode.Deactivated:
						var upEvent = Preview ? UIElement.PreviewMouseUpEvent : UIElement.MouseUpEvent;
						return args.RoutedEvent == upEvent &&
							   (mouseEventArgs.Button == Cvar.Value.MouseButton || mouseEventArgs.Modifiers != Cvar.Value.Modifiers);
					default:
						Log.Die("Unknown trigger mode.");
						return false;
				}
			}

			return false;
		}
	}
}