namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Input;

	/// <summary>
	///     Represents a button control.
	/// </summary>
	public class Button : ContentControl
	{
		/// <summary>
		///     Raised when the button has been clicked or the enter key has been pressed while the button has the keyboard focus.
		/// </summary>
		public static readonly RoutedEvent<RoutedEventArgs> ClickEvent = new RoutedEvent<RoutedEventArgs>(RoutingStrategy.Bubble);

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Button()
		{
			KeyDownEvent.Raised += OnKeyDown;
			MouseLeftButtonUpEvent.Raised += OnLeftMouseButtonUp;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public Button()
		{
			Focusable = true;
		}

		/// <summary>
		///     Raised when the button has been clicked or the enter key has been pressed while the button has the keyboard focus.
		/// </summary>
		public event RoutedEventHandler<RoutedEventArgs> Click
		{
			add { AddHandler(ClickEvent, value); }
			remove { RemoveHandler(ClickEvent, value); }
		}

		/// <summary>
		///     Checks whether the enter key has been pressed and if so, raises the Click event and sets the mouse event to handled.
		/// </summary>
		private static void OnKeyDown(object sender, KeyEventArgs e)
		{
			var button = sender as Button;
			if (button == null)
				return;

			if (e.Key != Key.Return && e.Key != Key.NumpadEnter)
				return;

			e.Handled = true;
			button.RaiseEvent(ClickEvent, RoutedEventArgs.Default);
		}

		/// <summary>
		///     Raises the Click event, setting the mouse event to handled. Additionally, sets the keyboard focus to the button.
		/// </summary>
		private static void OnLeftMouseButtonUp(object sender, MouseButtonEventArgs e)
		{
			var button = sender as Button;
			if (button == null)
				return;

			e.Handled = true;
			button.RaiseEvent(ClickEvent, RoutedEventArgs.Default);
			button.Focus();
		}
	}
}