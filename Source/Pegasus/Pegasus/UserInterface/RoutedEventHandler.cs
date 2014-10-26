namespace Pegasus.UserInterface
{
	using System;

	/// <summary>
	///     Represents a handler for routed events that require specific event data.
	/// </summary>
	/// <typeparam name="T">The type of the data passed to the event handler.</typeparam>
	public delegate void RoutedEventHandler<in T>(object sender, T e)
		where T : RoutedEventArgs;
}