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
		///     The default template that defines the visual appearance of a button.
		/// </summary>
		private static readonly ControlTemplate DefaultTemplate = control =>
		{
			var border = new Border();
			border.CreateTemplateBinding(control, BorderBrushProperty, Border.BorderBrushProperty);
			border.CreateTemplateBinding(control, BorderThicknessProperty, Border.BorderThicknessProperty);
			border.CreateTemplateBinding(control, PaddingProperty, Border.PaddingProperty);

			var presenter = new ContentPresenter();
			presenter.CreateTemplateBinding(control, ContentProperty, ContentPresenter.ContentProperty);
			presenter.CreateTemplateBinding(control, HorizontalContentAlignmentProperty, HorizontalAlignmentProperty);
			presenter.CreateTemplateBinding(control, VerticalContentAlignmentProperty, VerticalAlignmentProperty);

			border.Child = presenter;
			return border;
		};

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
		///     Initializes a new instance.
		/// </summary>
		public Button()
		{
			SetStyleValue(TemplateProperty, DefaultTemplate);
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