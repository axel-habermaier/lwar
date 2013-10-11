using System;

namespace Pegasus.Framework.UserInterface
{
	/// <summary>
	///   Represents a handler for various routed events that do not require any specific event data except for the data common
	///   to all routed events.
	/// </summary>
	public delegate void RoutedEventHandler(object sender, IRoutedEventArgs e);
}