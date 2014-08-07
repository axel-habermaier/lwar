namespace Pegasus.Framework.UserInterface
{
	using System;

	/// <summary>
	///     Associates common data to a routed event that has been raised.
	/// </summary>
	public class RoutedEventArgs
	{
		/// <summary>
		///     Gets the object the event originated from.
		/// </summary>
		public object Source { get; internal set; }

		/// <summary>
		///     Gets the routed event that has been raised.
		/// </summary>
		public RoutedEvent RoutedEvent { get; internal set; }

		/// <summary>
		///     Gets or sets a value indicating whether the routed event has already been handled.
		/// </summary>
		public bool Handled { get; set; }
	}
}