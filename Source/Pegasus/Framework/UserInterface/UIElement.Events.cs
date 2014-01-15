namespace Pegasus.Framework.UserInterface
{
	using System;

	public partial class UIElement
	{
		/// <summary>
		///     Occurs when a key is pressed while the UI element is focused.
		/// </summary>
		public static readonly RoutedEvent<RoutedEventArgs> KeyDownEvent =
			new RoutedEvent<RoutedEventArgs>(RoutingStrategy.Bubble);

		/// <summary>
		///     Occurs when a key is pressed while the UI element is focused.
		/// </summary>
		public event RoutedEventHandler<RoutedEventArgs> KeyDown
		{
			add { AddHandler(KeyDownEvent, value); }
			remove { RemoveHandler(KeyDownEvent, value); }
		}

		/// <summary>
		///     Raised when a change to a resource dictionary in this UI element or one of its ancestors has occurred.
		/// </summary>
		internal event Action ResourcesInvalidated;
	}
}