using System;

namespace Pegasus.Framework.Platform.Graphics
{
	/// <summary>
	///   Represents a query that records the current GPU timestamp. A timestamp query can only be issued when a timestamp
	///   disjoint query is currently active.
	/// </summary>
	public class TimestampQuery : Query
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		public TimestampQuery(GraphicsDevice graphicsDevice)
			: base(graphicsDevice, QueryType.Timestamp)
		{
		}

		/// <summary>
		///   Gets the queried GPU timestamp.
		/// </summary>
		public unsafe ulong Timestamp
		{
			get
			{
				ulong data;
				GetQueryData(&data, sizeof(ulong));
				return data;
			}
		}

		/// <summary>
		///   Queries the current GPU timestamp.
		/// </summary>
		public void Query()
		{
			EndQuery();
		}
	}
}