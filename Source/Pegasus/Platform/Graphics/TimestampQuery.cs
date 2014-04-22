namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///     Represents a query that records the current GPU timestamp. A timestamp query can only be issued when a timestamp
	///     disjoint query is currently active.
	/// </summary>
	public class TimestampQuery : Query
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public TimestampQuery()
			: base(QueryType.Timestamp)
		{
		}

		/// <summary>
		///     Gets the queried GPU timestamp.
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
		///     Queries the current GPU timestamp.
		/// </summary>
		public void Query()
		{
			EndQuery();
		}
	}
}