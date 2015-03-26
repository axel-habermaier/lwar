﻿namespace Pegasus.Platform.Graphics
{
	using System;
	using Utilities;

	/// <summary>
	///     Represents a query that checks whether the GPU has reached a CPU/GPU synchronization marker.
	/// </summary>
	public unsafe class SyncedQuery : Query
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		public SyncedQuery(GraphicsDevice graphicsDevice)
			: base(graphicsDevice, QueryType.Synced)
		{
		}

		/// <summary>
		///     Places the CPU/GPU synchronization marker into the GPU's command stream.
		/// </summary>
		public void MarkSyncPoint()
		{
			Assert.NotDisposed(this);
			DeviceInterface->EndQuery(NativeObject);
		}
	}
}