namespace Pegasus.Framework.UserInterface
{
	using System;

	/// <summary>
	///     Represents the base class for a dependency property value.
	/// </summary>
	internal abstract class RoutedEventStorage : SparseObjectStorage<RoutedEventStorage>.IStorageLocation
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
		///     Gets the storage location of the routed event handlers.
		/// </summary>
		public int Location
		{
			get { return Event.Index; }
		}
	}
}