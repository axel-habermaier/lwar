namespace Pegasus.UserInterface
{
	using System;

	/// <summary>
	///     Represents a strongly-typed routed event.
	/// </summary>
	/// <typeparam name="T">The type of the data associated with the routed event.</typeparam>
	public class RoutedEvent<T> : RoutedEvent
		where T : RoutedEventArgs
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="routingStrategy">The routing strategy that should be used by the event.</param>
		public RoutedEvent(RoutingStrategy routingStrategy)
			: base(routingStrategy)
		{
		}

		/// <summary>
		///     Raised when the routed event has been raised on any UI element.
		/// </summary>
		public event RoutedEventHandler<T> Raised;

		/// <summary>
		///     Invokes the class handlers of this routed event.
		/// </summary>
		/// <param name="sender">The object that raised the event.</param>
		/// <param name="args">The routed event arguments.</param>
		internal void InvokeClassHandlers(object sender, T args)
		{
			if (Raised != null)
				Raised(sender, args);
		}
	}
}