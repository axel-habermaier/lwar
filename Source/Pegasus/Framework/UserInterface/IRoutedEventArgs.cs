using System;

namespace Pegasus.Framework.UserInterface
{
	/// <summary>
	///   Contains data associated with a routed event that has been raised.
	/// </summary>
	public interface IRoutedEventArgs
	{
		/// <summary>
		///   Gets or sets a value indicating whether the routed event has already been handled.
		/// </summary>
		bool Handled { get; set; }

		/// <summary>
		///   Gets the object the event originated from.
		/// </summary>
		object Source { get; }

		/// <summary>
		///   Gets the routed event that has been raised.
		/// </summary>
		RoutedEvent RoutedEvent { get; }
	}
}