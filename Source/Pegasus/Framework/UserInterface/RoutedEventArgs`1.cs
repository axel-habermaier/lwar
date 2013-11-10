using System;

namespace Pegasus.Framework.UserInterface
{
	using Platform.Memory;

	/// <summary>
	///   Abstract base class that associates data to a routed event that has been raised.
	/// </summary>
	public abstract class RoutedEventArgs<T> : PooledObject<T>, IRoutedEventArgs
		where T : RoutedEventArgs<T>, new()
	{
		/// <summary>
		///   Gets or sets a value indicating whether the routed event has already been handled.
		/// </summary>
		public bool Handled { get; set; }

		/// <summary>
		///   Gets the object the event originated from.
		/// </summary>
		public object Source { get; internal set; }

		/// <summary>
		///   Gets the routed event that has been raised.
		/// </summary>
		public RoutedEvent RoutedEvent { get; internal set; }
	}
}