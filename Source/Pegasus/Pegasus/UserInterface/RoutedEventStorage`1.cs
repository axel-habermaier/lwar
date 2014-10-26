namespace Pegasus.UserInterface
{
	using System;
	using Utilities;

	/// <summary>
	///     Represents the base class for a dependency property value.
	/// </summary>
	/// <typeparam name="T">The type of the data associated with the routed event.</typeparam>
	internal class RoutedEventStorage<T> : RoutedEventStorage
		where T : RoutedEventArgs
	{
		/// <summary>
		///     The binding that is set for the routed event.
		/// </summary>
		private RoutedEventBinding<T> _binding;

		/// <summary>
		///     The handlers that must be invoked when the routed event is raised.
		/// </summary>
		private RoutedEventHandler<T> _handlers;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="routedEvent">The routed event whose handlers are stored.</param>
		public RoutedEventStorage(RoutedEvent routedEvent)
			: base(routedEvent)
		{
		}

		/// <summary>
		///     Adds the given handler to the routed event.
		/// </summary>
		/// <param name="handler">The handler that should be invoked when the routed event is raised.</param>
		public void AddHandler(RoutedEventHandler<T> handler)
		{
			Assert.ArgumentNotNull(handler);
			_handlers += handler;
		}

		/// <summary>
		///     Removes the given handler from the routed event.
		/// </summary>
		/// <param name="handler">The handler that should no longer be invoked when the routed event is raised.</param>
		public void RemoveHandler(RoutedEventHandler<T> handler)
		{
			Assert.ArgumentNotNull(handler);
			_handlers -= handler;
		}

		/// <summary>
		///     Invokes the registered handlers.
		/// </summary>
		/// <param name="sender">The object that raises the event.</param>
		/// <param name="arguments">The arguments that should be passed to the event handlers.</param>
		public void InvokeHandlers(object sender, T arguments)
		{
			Assert.ArgumentNotNull(arguments);

			if (_handlers != null)
				_handlers(sender, arguments);
		}

		/// <summary>
		///     Sets the given binding for the routed event.
		/// </summary>
		/// <param name="binding">The binding that should be set.</param>
		public void SetBinding(RoutedEventBinding<T> binding)
		{
			_binding = binding;
		}

		/// <summary>
		///     Updates the activation state of the event's binding, if any.
		/// </summary>
		/// <param name="activated">Indicates whether the binding should be activated.</param>
		public override void SetBindingActivationState(bool activated)
		{
			if (_binding != null)
				_binding.Active = activated;
		}
	}
}