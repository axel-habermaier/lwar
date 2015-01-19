namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///     Describes the type of a query.
	/// </summary>
	public enum QueryType
	{
		/// <summary>
		///     Indicates that a query represents a GPU/CPU synchronization marker.
		/// </summary>
		Synced,

		/// <summary>
		///     Indicates that a query is an occlusion query.
		/// </summary>
		Occlusion,

		/// <summary>
		///     Indicates that a query is a timestamp query.
		/// </summary>
		Timestamp,

		/// <summary>
		///     Indicates that a query is a timestamp disjoint query.
		/// </summary>
		TimestampDisjoint
	}
}