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
		///     Raised when the button has been clicked.
		/// </summary>
		public static readonly RoutedEvent<RoutedEventArgs> ClickEvent = new RoutedEvent<RoutedEventArgs>(RoutingStrategy.Bubble);

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Button()
		{
			MouseUpEvent.Raised += OnMouseUp;
		}

		/// <summary>
		///     Raised when the button has been clicked.
		/// </summary>
		public event RoutedEventHandler<RoutedEventArgs> Click
		{
			add { AddHandler(ClickEvent, value); }
			remove { RemoveHandler(ClickEvent, value); }
		}

		/// <summary>
		///     Raises the Click event, setting the mouse event to handled. Additionally, sets the keyboard focus to the button.
		/// </summary>
		private static void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			var button = sender as Button;
			if (button == null || e.Button != MouseButton.Left)
				return;

			e.Handled = true;
			button.RaiseEvent(ClickEvent, RoutedEventArgs.Default);
			button.Focus();
		}
	}
}