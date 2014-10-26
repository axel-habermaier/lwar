namespace Pegasus.Entities
{
	using System;

	/// <summary>
	///     Represents an event handler for an entity event of the given type.
	/// </summary>
	/// <typeparam name="TEvent">The type of the entity event that is handled by the entity event handler.</typeparam>
	public interface IEventHandler<in TEvent> : IEventHandler
		where TEvent : struct
	{
		/// <summary>
		///     Handles the event for the given entity.
		/// </summary>
		/// <param name="eventArgs">The event arguments.</param>
		/// <param name="entity">The entity the event was raised for.</param>
		void HandleEvent(TEvent eventArgs, Entity entity);
	}
}