namespace Pegasus.Framework.UserInterface
{
	using System;

	/// <summary>
	///   Represents a typed routed event.
	/// </summary>
	/// <typeparam name="T">The type of the event handler delegate.</typeparam>
	public class RoutedEvent<T> : RoutedEvent
		where T : class
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="routingStrategy">The routing strategy that should be used by the event.</param>
		public RoutedEvent(RoutingStrategy routingStrategy)
			: base(routingStrategy)
		{
			Assert.That(typeof(RoutedEventHandler).IsAssignableFrom(typeof(T)), "Expected 'RoutedEventHandler' delegate (sub-)type.");
		}
	}
}