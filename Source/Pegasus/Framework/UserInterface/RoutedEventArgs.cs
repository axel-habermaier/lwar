using System;

namespace Pegasus.Framework.UserInterface
{
	/// <summary>
	///   Associates only the data common to all routed events to a routed event that has been raised.
	/// </summary>
	public class RoutedEventArgs : RoutedEventArgs<RoutedEventArgs>
	{
	}
}