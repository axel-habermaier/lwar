namespace Pegasus.UserInterface
{
	using System;
	using Utilities;

	/// <summary>
	///     Represents the base class for a dependency property value.
	/// </summary>
	internal abstract class RoutedEventStorage
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="routedEvent">The routed event whose handlers are stored.</param>
		protected RoutedEventStorage(RoutedEvent routedEvent)
		{
			Assert.ArgumentNotNull(routedEvent);
			Event = routedEvent;
		}

		/// <summary>
		///     Gets the routed event whose handlers are stored.
		/// </summary>
		public RoutedEvent Event { get; private set; }

		/// <summary>
		///     Updates the activation state of the event's binding, if any.
		/// </summary>
		/// <param name="activated">Indicates whether the binding should be activated.</param>
		public abstract void SetBindingActivationState(bool activated);
	}
}