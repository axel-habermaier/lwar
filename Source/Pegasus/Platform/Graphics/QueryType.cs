namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///     Describes the type of a query.
	/// </summary>
	public enum QueryType
	{
		/// <summary>
		///     Indicates that a query is a timestamp query.
		/// </summary>
		Timestamp = 2801,

		/// <summary>
		///     Indicates that a query is a timestamp disjoint query.
		/// </summary>
		TimestampDisjoint = 2802,

		/// <summary>
		///     Indicates that a query is an occlusion query.
		/// </summary>
		Occlusion = 2803,

		/// <summary>
		///     Indicates that a query represents a GPU/CPU synchronization marker.
		/// </summary>
		Synced = 2804
	}
}