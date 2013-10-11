namespace Pegasus.Framework.UserInterface
{
	using System;

	/// <summary>
	///   A sparse storage for routed event handlers.
	/// </summary>
	internal struct RoutedEventStore
	{
		/// <summary>
		///   The values that are currently stored.
		/// </summary>
		private SparseObjectStorage<RoutedEventStorage> _values;

		/// <summary>
		///   Adds the given handler to the routed event.
		/// </summary>
		/// <typeparam name="T">The type of the data associated with the routed event.</typeparam>
		/// <param name="routedEvent">The routed event the handler should be added to.</param>
		/// <param name="handler">The handler that should be invoked when the routed event is raised.</param>
		public void AddHandler<T>(RoutedEvent<T> routedEvent, RoutedEventHandler<T> handler)
			where T : class, IRoutedEventArgs
		{
			Assert.ArgumentNotNull(routedEvent);
			Assert.ArgumentNotNull(handler);

			var storage = _values.Get(routedEvent.Index) as RoutedEventStorage<T>;
			if (storage == null)
				_values.Add(storage = new RoutedEventStorage<T>(routedEvent));

			storage.AddHandler(handler);
		}

		/// <summary>
		///   Removes the given handler from the routed event.
		/// </summary>
		/// <typeparam name="T">The type of the data associated with the routed event.</typeparam>
		/// <param name="routedEvent">The routed event the handler should be removed from.</param>
		/// <param name="handler">The handler that should no longer be invoked when the routed event is raised.</param>
		public void RemoveHandler<T>(RoutedEvent<T> routedEvent, RoutedEventHandler<T> handler)
			where T : class, IRoutedEventArgs
		{
			Assert.ArgumentNotNull(routedEvent);
			Assert.ArgumentNotNull(handler);

			var storage = _values.Get(routedEvent.Index) as RoutedEventStorage<T>;
			if (storage != null)
				storage.RemoveHandler(handler);
		}
	}
}