namespace Pegasus.Framework.UserInterface
{
	using System;

	/// <summary>
	///   Represents the base class for a dependency property value.
	/// </summary>
	/// <typeparam name="T">The type of the data associated with the routed event.</typeparam>
	internal class RoutedEventStorage<T> : RoutedEventStorage
		where T : class, IRoutedEventArgs
	{
		/// <summary>
		///   The handlers that must be invoked when the routed event is raised.
		/// </summary>
		private RoutedEventHandler<T> _handlers;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="routedEvent">The routed event whose handlers are stored.</param>
		public RoutedEventStorage(RoutedEvent routedEvent)
			: base(routedEvent)
		{
		}

		/// <summary>
		///   Adds the given handler to the routed event.
		/// </summary>
		/// <param name="handler">The handler that should be invoked when the routed event is raised.</param>
		public void AddHandler(RoutedEventHandler<T> handler)
		{
			Assert.ArgumentNotNull(handler);
			_handlers += handler;
		}

		/// <summary>
		///   Removes the given handler from the routed event.
		/// </summary>
		/// <param name="handler">The handler that should no longer be invoked when the routed event is raised.</param>
		public void RemoveHandler(RoutedEventHandler<T> handler)
		{
			Assert.ArgumentNotNull(handler);
			_handlers -= handler;
		}
	}
}