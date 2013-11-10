namespace Pegasus.Framework.UserInterface
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	///   Represents an untyped routed event.
	/// </summary>
	public abstract class RoutedEvent
	{
		/// <summary>
		///   The number of routed events that have been registered throughout the lifetime of the application.
		/// </summary>
		private static int _eventCount;

		/// <summary>
		///   The list of all routed events that have been created, sorted by routed event index.
		/// </summary>
		internal static readonly List<RoutedEvent> RoutedEvents = new List<RoutedEvent>();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="routingStrategy">The routing strategy that should be used by the event.</param>
		protected RoutedEvent(RoutingStrategy routingStrategy)
		{
			Assert.ArgumentInRange(routingStrategy);

			Index = _eventCount++;
			RoutedEvents.Add(this);

			RoutingStrategy = routingStrategy;
		}

		/// <summary>
		///   The index of the routed event that remains unchanged and unique throughout the lifetime of the application.
		/// </summary>
		internal int Index { get; private set; }

		/// <summary>
		///   Gets the routing strategy used by the event.
		/// </summary>
		public RoutingStrategy RoutingStrategy { get; private set; }
	}
}