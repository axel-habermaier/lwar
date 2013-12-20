namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	///   Represents a query that records the frequency of the GPU timer that can be used to interpret the result of timestamp
	///   queries. It also indicates whether a GPU event invalidates all timestamp results.
	/// </summary>
	public class TimestampDisjointQuery : Query
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		public TimestampDisjointQuery(GraphicsDevice graphicsDevice)
			: base(graphicsDevice, QueryType.TimestampDisjoint)
		{
		}

		/// <summary>
		///   Gets the queried GPU timestamp.
		/// </summary>
		public unsafe QueryData Result
		{
			get
			{
				QueryData data;
				GetQueryData(&data, sizeof(QueryData));
				return data;
			}
		}

		/// <summary>
		///   Begins the timestamp disjoint query.
		/// </summary>
		public void Begin()
		{
			BeginQuery();
		}

		/// <summary>
		///   Ends the timestamp disjoint query.
		/// </summary>
		public void End()
		{
			EndQuery();
		}

		/// <summary>
		///   Represents the data returned by a timestamp disjoint query.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct QueryData
		{
			/// <summary>
			///   The frequency of the GPU's internal timer.
			/// </summary>
			public readonly ulong Frequency;

			/// <summary>
			///   Indicates whether the timestamp queries that have been executed while the timestamp disjoint query was active
			///   returned valid data or should be discarded.
			/// </summary>
			public readonly bool Valid;
		}
	}
}