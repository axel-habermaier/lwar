namespace Pegasus.UserInterface
{
	using System;
	using Input;

	public partial class UIElement
	{
		/// <summary>
		///     Raised when a key is pressed while the UI element is focused.
		/// </summary>
		public static readonly RoutedEvent<KeyEventArgs> KeyDownEvent =
			new RoutedEvent<KeyEventArgs>(RoutingStrategy.Bubble);

		/// <summary>
		///     Raised when a key is pressed while the UI element is focused.
		/// </summary>
		public static readonly RoutedEvent<KeyEventArgs> PreviewKeyDownEvent =
			new RoutedEvent<KeyEventArgs>(RoutingStrategy.Tunnel);

		/// <summary>
		///     Raised when a key is released while the UI element is focused.
		/// </summary>
		public static readonly RoutedEvent<KeyEventArgs> KeyUpEvent =
			new RoutedEvent<KeyEventArgs>(RoutingStrategy.Bubble);

		/// <summary>
		///     Raised when a key is released while the UI element is focused.
		/// </summary>
		public static readonly RoutedEvent<KeyEventArgs> PreviewKeyUpEvent =
			new RoutedEvent<KeyEventArgs>(RoutingStrategy.Tunnel);

		/// <summary>
		///     Raised when the UI element gets text input.
		/// </summary>
		public static readonly RoutedEvent<TextInputEventArgs> TextInputEvent =
			new RoutedEvent<TextInputEventArgs>(RoutingStrategy.Bubble);

		/// <summary>
		///     Raised when the UI element gets text input.
		/// </summary>
		public static readonly RoutedEvent<TextInputEventArgs> PreviewTextInputEvent =
			new RoutedEvent<TextInputEventArgs>(RoutingStrategy.Tunnel);

		/// <summary>
		///     Raised when a mouse button is pressed while the mouse is over the UI element.
		/// </summary>
		public static readonly RoutedEvent<MouseButtonEventArgs> MouseDownEvent =
			new RoutedEvent<MouseButtonEventArgs>(RoutingStrategy.Bubble);

		/// <summary>
		///     Raised when a mouse button is pressed while the mouse is over the UI element.
		/// </summary>
		public static readonly RoutedEvent<MouseButtonEventArgs> PreviewMouseDownEvent =
			new RoutedEvent<MouseButtonEventArgs>(RoutingStrategy.Tunnel);

		/// <summary>
		///     Raised when a mouse button is released while the mouse is over the UI element.
		/// </summary>
		public static readonly RoutedEvent<MouseButtonEventArgs> MouseUpEvent =
			new RoutedEvent<MouseButtonEventArgs>(RoutingStrategy.Bubble);

		/// <summary>
		///     Raised when a mouse button is released while the mouse is over the UI element.
		/// </summary>
		public static readonly RoutedEvent<MouseButtonEventArgs> PreviewMouseUpEvent =
			new RoutedEvent<MouseButtonEventArgs>(RoutingStrategy.Tunnel);

		/// <summary>
		///     Raised when the mouse wheel has changed while the mouse is over the UI element.
		/// </summary>
		public static readonly RoutedEvent<MouseWheelEventArgs> MouseWheelEvent =
			new RoutedEvent<MouseWheelEventArgs>(RoutingStrategy.Bubble);

		/// <summary>
		///     Raised when the mouse wheel has changed while the mouse is over the UI element.
		/// </summary>
		public static readonly RoutedEvent<MouseWheelEventArgs> PreviewMouseWheelEvent =
			new RoutedEvent<MouseWheelEventArgs>(RoutingStrategy.Tunnel);

		/// <summary>
		///     Raised when the mouse has moved while the mouse is over the UI element.
		/// </summary>
		public static readonly RoutedEvent<MouseEventArgs> MouseMoveEvent =
			new RoutedEvent<MouseEventArgs>(RoutingStrategy.Bubble);

		/// <summary>
		///     Raised when the mouse has moved while the mouse is over the UI element.
		/// </summary>
		public static readonly RoutedEvent<MouseEventArgs> PreviewMouseMoveEvent =
			new RoutedEvent<MouseEventArgs>(RoutingStrategy.Tunnel);

		/// <summary>
		///     Raised when the mouse has entered the bounds of the UI element.
		/// </summary>
		public static readonly RoutedEvent<MouseEventArgs> MouseEnterEvent =
			new RoutedEvent<MouseEventArgs>(RoutingStrategy.Direct);

		/// <summary>
		///     Raised when the mouse has left the bounds of the UI element.
		/// </summary>
		public static readonly RoutedEvent<MouseEventArgs> MouseLeaveEvent =
			new RoutedEvent<MouseEventArgs>(RoutingStrategy.Direct);

		/// <summary>
		///     Raised when a key is pressed while the UI element is focused.
		/// </summary>
		public event RoutedEventHandler<KeyEventArgs> KeyDown
		{
			add { AddHandler(KeyDownEvent, value); }
			remove { RemoveHandler(KeyDownEvent, value); }
		}

		/// <summary>
		///     Raised when a key is pressed while the UI element is focused.
		/// </summary>
		public event RoutedEventHandler<KeyEventArgs> PreviewKeyDown
		{
			add { AddHandler(PreviewKeyDownEvent, value); }
			remove { RemoveHandler(PreviewKeyDownEvent, value); }
		}

		/// <summary>
		///     Raised when a key is released while the UI element is focused.
		/// </summary>
		public event RoutedEventHandler<KeyEventArgs> KeyUp
		{
			add { AddHandler(KeyUpEvent, value); }
			remove { RemoveHandler(KeyUpEvent, value); }
		}

		/// <summary>
		///     Raised when a key is released while the UI element is focused.
		/// </summary>
		public event RoutedEventHandler<KeyEventArgs> PreviewKeyUp
		{
			add { AddHandler(PreviewKeyUpEvent, value); }
			remove { RemoveHandler(PreviewKeyUpEvent, value); }
		}

		/// <summary>
		///     Raised when the UI element gets text input.
		/// </summary>
		public event RoutedEventHandler<TextInputEventArgs> TextInput
		{
			add { AddHandler(TextInputEvent, value); }
			remove { RemoveHandler(TextInputEvent, value); }
		}

		/// <summary>
		///     Raised when the UI element gets text input.
		/// </summary>
		public event RoutedEventHandler<TextInputEventArgs> PreviewTextInput
		{
			add { AddHandler(PreviewTextInputEvent, value); }
			remove { RemoveHandler(PreviewTextInputEvent, value); }
		}

		/// <summary>
		///     Raised when a mouse button is pressed while the mouse is over the UI element.
		/// </summary>
		public event RoutedEventHandler<MouseButtonEventArgs> MouseDown
		{
			add { AddHandler(MouseDownEvent, value); }
			remove { RemoveHandler(MouseDownEvent, value); }
		}

		/// <summary>
		///     Raised when a mouse button is pressed while the mouse is over the UI element.
		/// </summary>
		public event RoutedEventHandler<MouseButtonEventArgs> PreviewMouseDown
		{
			add { AddHandler(PreviewMouseDownEvent, value); }
			remove { RemoveHandler(PreviewMouseDownEvent, value); }
		}

		/// <summary>
		///     Raised when a mouse button is released while the mouse is over the UI element.
		/// </summary>
		public event RoutedEventHandler<MouseButtonEventArgs> MouseUp
		{
			add { AddHandler(MouseUpEvent, value); }
			remove { RemoveHandler(MouseUpEvent, value); }
		}

		/// <summary>
		///     Raised when a mouse button is released while the mouse is over the UI element.
		/// </summary>
		public event RoutedEventHandler<MouseButtonEventArgs> PreviewMouseUp
		{
			add { AddHandler(PreviewMouseUpEvent, value); }
			remove { RemoveHandler(PreviewMouseUpEvent, value); }
		}

		/// <summary>
		///     Raised when the mouse wheel has changed while the mouse is over the UI element.
		/// </summary>
		public event RoutedEventHandler<MouseWheelEventArgs> MouseWheel
		{
			add { AddHandler(MouseWheelEvent, value); }
			remove { RemoveHandler(MouseWheelEvent, value); }
		}

		/// <summary>
		///     Raised when the mouse wheel has changed while the mouse is over the UI element.
		/// </summary>
		public event RoutedEventHandler<MouseWheelEventArgs> PreviewMouseWheel
		{
			add { AddHandler(PreviewMouseWheelEvent, value); }
			remove { RemoveHandler(PreviewMouseWheelEvent, value); }
		}

		/// <summary>
		///     Raised when the mouse has moved while the mouse is over the UI element.
		/// </summary>
		public event RoutedEventHandler<MouseEventArgs> MouseMove
		{
			add { AddHandler(MouseMoveEvent, value); }
			remove { RemoveHandler(MouseMoveEvent, value); }
		}

		/// <summary>
		///     Raised when the mouse has moved while the mouse is over the UI element.
		/// </summary>
		public event RoutedEventHandler<MouseEventArgs> PreviewMouseMove
		{
			add { AddHandler(PreviewMouseMoveEvent, value); }
			remove { RemoveHandler(PreviewMouseMoveEvent, value); }
		}

		/// <summary>
		///     Raised when the mouse has entered the bounds of the UI element.
		/// </summary>
		public event RoutedEventHandler<MouseEventArgs> MouseEnter
		{
			add { AddHandler(MouseEnterEvent, value); }
			remove { RemoveHandler(MouseEnterEvent, value); }
		}

		/// <summary>
		///     Raised when the mouse has left the bounds of the UI element.
		/// </summary>
		public event RoutedEventHandler<MouseEventArgs> MouseLeave
		{
			add { AddHandler(MouseLeaveEvent, value); }
			remove { RemoveHandler(MouseLeaveEvent, value); }
		}

		/// <summary>
		///     Raised when a change to a resource dictionary in this UI element or one of its ancestors has occurred.
		/// </summary>
		internal event Action ResourcesInvalidated;

		/// <summary>
		///     Raised when the value of the UI element's IsVisible property changed.
		/// </summary>
		public event DependencyPropertyChangedHandler<bool> IsVisibleChanged;
	}
}