namespace Pegasus.Framework.UserInterface
{
	using System;

	/// <summary>
	///     Indicates the routing strategy of a routed event.
	/// </summary>
	public enum RoutingStrategy : byte
	{
		/// <summary>
		///     Indicates that the routed event is raised on the source element only and does not traverse the element tree.
		/// </summary>
		Direct,

		/// <summary>
		///     Indicates that the routed event routes upwards through the object tree; from the event source to the tree root.
		/// </summary>
		Bubble,

		/// <summary>
		///     Indicates that the routed event routes downwards through the object tree; from the tree root to the event source.
		/// </summary>
		Tunnel
	}
}