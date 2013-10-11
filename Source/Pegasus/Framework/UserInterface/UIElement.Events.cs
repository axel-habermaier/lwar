namespace Pegasus.Framework.UserInterface
{
	using System;

	public partial class UIElement
	{
		/// <summary>
		///   Occurs when a key is pressed while the UI element is focused.
		/// </summary>
		public static readonly RoutedEvent<RoutedEventHandler> KeyDownEvent =
			new RoutedEvent<RoutedEventHandler>(RoutingStrategy.Bubble);

		/// <summary>
		///   Occurs when a key is pressed while the UI element is focused.
		/// </summary>
		public event RoutedEventHandler KeyDown
		{
			add { AddHandler(KeyDownEvent, value); }
			remove { RemoveHandler(KeyDownEvent, value); }
		}

		/// <summary>
		///   Raised when a change to a resource dictionary in this UI element or one of its ancestors has occurred.
		/// </summary>
		internal event Action ResourcesInvalidated;

		/// <summary>
		///   Adds the given handler to the given routed event. If handledEventsToo is true, the handler is invoked even if the the
		///   routed event has already been marked as handled along the event route.
		/// </summary>
		/// <typeparam name="T">The type of the event handler delegate.</typeparam>
		/// <param name="routedEvent">The routed event that should be handled.</param>
		/// <param name="handler">The handler that should be invoked when the routed event has been raised.</param>
		/// <param name="handledEventsToo">
		///   Indicates whether the handler should be invoked even if the the
		///   routed event has already been marked as handled along the event route.
		/// </param>
		public void AddHandler<T>(RoutedEvent<T> routedEvent, T handler, bool handledEventsToo = false)
			where T : class
		{
			Assert.ArgumentNotNull(routedEvent);
			Assert.ArgumentNotNull(handler);
		}

		/// <summary>
		///   Removes the given handler from the given routed event.
		/// </summary>
		/// <typeparam name="T">The type of the event handler delegate.</typeparam>
		/// <param name="routedEvent">The routed event that should be handled.</param>
		/// <param name="handler">The handler that should be invoked when the routed event has been raised.</param>
		public void RemoveHandler<T>(RoutedEvent<T> routedEvent, T handler)
			where T : class
		{
			Assert.ArgumentNotNull(routedEvent);
			Assert.ArgumentNotNull(handler);
		}
	}
}