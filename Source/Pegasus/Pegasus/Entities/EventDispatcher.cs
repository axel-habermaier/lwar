namespace Pegasus.Entities
{
	using System;
	using System.Collections.Generic;
	using Utilities;

	/// <summary>
	///     Dispatches entity events.
	/// </summary>
	public sealed class EventDispatcher
	{
		/// <summary>
		///     The list of registered event handlers.
		/// </summary>
		private readonly List<IEventHandler> _eventHandlers = new List<IEventHandler>();

		/// <summary>
		///     Adds the given event handler to the dispatcher.
		/// </summary>
		/// <param name="eventHandler">The event handler that should be added.</param>
		public void AddEventHandler(IEventHandler eventHandler)
		{
			Assert.ArgumentNotNull(eventHandler);
			Assert.ArgumentSatisfies(!_eventHandlers.Contains(eventHandler), "The event handler has already been added.");

			_eventHandlers.Add(eventHandler);
		}

		/// <summary>
		///     Removes the given event handler from the dispatcher.
		/// </summary>
		/// <param name="eventHandler">The event handler that should be removed.</param>
		public void RemoveEventHandler(IEventHandler eventHandler)
		{
			Assert.ArgumentNotNull(eventHandler);
			Assert.ArgumentSatisfies(_eventHandlers.Contains(eventHandler), "Cannot remove unknown event handler.");

			_eventHandlers.Remove(eventHandler);
		}

		/// <summary>
		///     Dispatches the given event for the given entity.
		/// </summary>
		/// <typeparam name="T">The type of the event that should be dispatched.</typeparam>
		/// <param name="eventArgs">The event arguments that should be dispatched.</param>
		/// <param name="entity">The entity the event should be dispatched for.</param>
		public void DispatchEvent<T>(T eventArgs, Entity entity)
			where T : struct
		{
			foreach (var eventHandler in _eventHandlers)
			{
				var typedHandler = eventHandler as IEventHandler<T>;
				if (typedHandler != null)
					typedHandler.HandleEvent(eventArgs, entity);
			}
		}
	}
}