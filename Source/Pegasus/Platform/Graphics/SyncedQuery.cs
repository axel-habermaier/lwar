namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///     Represents a query that checks whether the GPU has reached a CPU/GPU synchronization marker.
	/// </summary>
	public class SyncedQuery : Query
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public SyncedQuery()
			: base(QueryType.Synced)
		{
		}

		/// <summary>
		///     Places the CPU/GPU synchronization marker into the GPU's command stream.
		/// </summary>
		public void MarkSyncPoint()
		{
			EndQuery();
		}
	}
}