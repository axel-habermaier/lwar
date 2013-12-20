namespace Pegasus.Framework.UserInterface
{
	using System;

	/// <summary>
	///   Represents a strongly-typed routed event.
	/// </summary>
	/// <typeparam name="T">The type of the data associated with the routed event.</typeparam>
	public class RoutedEvent<T> : RoutedEvent
		where T : class, IRoutedEventArgs
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="routingStrategy">The routing strategy that should be used by the event.</param>
		public RoutedEvent(RoutingStrategy routingStrategy)
			: base(routingStrategy)
		{
		}
	}
}