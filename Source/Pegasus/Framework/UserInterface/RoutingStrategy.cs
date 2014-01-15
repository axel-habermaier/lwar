namespace Pegasus.Framework.UserInterface
{
	using System;

	/// <summary>
	///     Indicates the routing strategy of a routed event.
	/// </summary>
	public enum RoutingStrategy : byte
	{
		/// <summary>
		///     Indicates that the routed event routes upwards through the object tree; from event source to the tree root.
		/// </summary>
		Bubble,

		/// <summary>
		///     Indicates that the routed event routes downwards through the object tree; from tree root to event source.
		/// </summary>
		Tunnel
	}
}