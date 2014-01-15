namespace Pegasus.Framework.UserInterface
{
	using System;

	/// <summary>
	///     Represents a handler for various routed events that do not require any specific event data except for the data common
	///     to all routed events.
	/// </summary>
	/// <typeparam name="T">The type of the data passed to the event handler.</typeparam>
	public delegate void RoutedEventHandler<in T>(object sender, T e)
		where T : IRoutedEventArgs;
}