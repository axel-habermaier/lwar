﻿namespace Pegasus.UserInterface
{
	using System;
	using Utilities;

	/// <summary>
	///     A sparse storage for routed event handlers.
	/// </summary>
	internal struct RoutedEventStore
	{
		/// <summary>
		///     The values that are currently stored.
		/// </summary>
		private SparseObjectStorage<RoutedEventStorage> _values;

		/// <summary>
		///     Adds the given handler to the routed event.
		/// </summary>
		/// <typeparam name="T">The type of the data associated with the routed event.</typeparam>
		/// <param name="routedEvent">The routed event the handler should be added to.</param>
		/// <param name="handler">The handler that should be invoked when the routed event is raised.</param>
		public void AddHandler<T>(RoutedEvent<T> routedEvent, RoutedEventHandler<T> handler)
			where T : RoutedEventArgs
		{
			Assert.ArgumentNotNull(routedEvent);
			Assert.ArgumentNotNull(handler);

			var storage = _values.Get(routedEvent.Index) as RoutedEventStorage<T>;
			if (storage == null)
				_values.Add(storage = new RoutedEventStorage<T>(routedEvent), routedEvent.Index);

			storage.AddHandler(handler);
		}

		/// <summary>
		///     Removes the given handler from the routed event.
		/// </summary>
		/// <typeparam name="T">The type of the data associated with the routed event.</typeparam>
		/// <param name="routedEvent">The routed event the handler should be removed from.</param>
		/// <param name="handler">The handler that should no longer be invoked when the routed event is raised.</param>
		public void RemoveHandler<T>(RoutedEvent<T> routedEvent, RoutedEventHandler<T> handler)
			where T : RoutedEventArgs
		{
			Assert.ArgumentNotNull(routedEvent);
			Assert.ArgumentNotNull(handler);

			var storage = _values.Get(routedEvent.Index) as RoutedEventStorage<T>;
			if (storage != null)
				storage.RemoveHandler(handler);
		}

		/// <summary>
		///     Invokes the handlers registered on the routed event.
		/// </summary>
		/// <typeparam name="T">The type of the data associated with the routed event.</typeparam>
		/// <param name="routedEvent">The routed event that should be raised.</param>
		/// <param name="sender">The object that raises the event.</param>
		/// <param name="arguments">The arguments that should be passed to the event handlers.</param>
		public void InvokeHandlers<T>(RoutedEvent<T> routedEvent, object sender, T arguments)
			where T : RoutedEventArgs
		{
			Assert.ArgumentNotNull(routedEvent);
			Assert.ArgumentNotNull(arguments);

			var storage = _values.Get(routedEvent.Index) as RoutedEventStorage<T>;
			if (storage != null)
				storage.InvokeHandlers(sender, arguments);
		}

		/// <summary>
		///     Changes the activation state of all bindings that are set on any dependency properties.
		/// </summary>
		/// <param name="activated">Indicates whether the bindings should be activated.</param>
		public void SetBindingsActivationState(bool activated)
		{
			foreach (var value in _values)
				value.SetBindingActivationState(activated);
		}

		/// <summary>
		///     Sets the binding for the routed event.
		/// </summary>
		/// <typeparam name="T">The type of the data associated with the routed event.</typeparam>
		/// <param name="routedEvent">The routed event the binding should be set for.</param>
		/// <param name="binding">The binding that should be set.</param>
		public void SetBinding<T>(RoutedEvent<T> routedEvent, RoutedEventBinding<T> binding)
			where T : RoutedEventArgs
		{
			Assert.ArgumentNotNull(routedEvent);
			Assert.ArgumentNotNull(binding);

			var storage = _values.Get(routedEvent.Index) as RoutedEventStorage<T>;
			if (storage == null)
				_values.Add(storage = new RoutedEventStorage<T>(routedEvent), routedEvent.Index);

			storage.SetBinding(binding);
		}
	}
}