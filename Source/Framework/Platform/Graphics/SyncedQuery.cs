using System;

namespace Pegasus.Framework.Platform.Graphics
{
	/// <summary>
	///   Represents a query that checks whether the GPU has reached a CPU/GPU synchronization marker.
	/// </summary>
	public class SyncedQuery : Query
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		public SyncedQuery(GraphicsDevice graphicsDevice)
			: base(graphicsDevice, QueryType.Synced)
		{
		}

		/// <summary>
		///   Places the CPU/GPU synchronization marker into the GPU's command stream.
		/// </summary>
		public void MarkSyncPoint()
		{
			EndQuery();
		}
	}
}