namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Runtime.InteropServices;
	using Utilities;

	/// <summary>
	///     Represents a query that records the frequency of the GPU timer that can be used to interpret the result of timestamp
	///     queries. It also indicates whether a GPU event invalidates all timestamp results.
	/// </summary>
	public class TimestampDisjointQuery : Query
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		public TimestampDisjointQuery(GraphicsDevice graphicsDevice)
			: base(graphicsDevice, QueryType.TimestampDisjoint)
		{
		}

		/// <summary>
		///     Tries to gets the queried GPU frequency if the queried value is valid.
		/// </summary>
		public unsafe bool TryGetFrequency(out double frequency)
		{
			Assert.NotDisposed(this);

			Result data;
			QueryObject.GetResult(&data);

			frequency = data.Frequency;
			return !data.Disjoint;
		}

		/// <summary>
		///     Begins the timestamp disjoint query.
		/// </summary>
		public void Begin()
		{
			Assert.NotDisposed(this);
			QueryObject.Begin();
		}

		/// <summary>
		///     Ends the timestamp disjoint query.
		/// </summary>
		public void End()
		{
			Assert.NotDisposed(this);
			QueryObject.End();
		}

		/// <summary>
		///     Represents the data returned by a timestamp disjoint query.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Size=16)]
		internal struct Result
		{
			/// <summary>
			///     The frequency of the GPU's internal timer.
			/// </summary>
			public ulong Frequency;

			/// <summary>
			///     Indicates whether the timestamp queries that have been executed while the timestamp disjoint query was active
			///     returned valid data or should be discarded.
			/// </summary>
			public bool Disjoint;
		}
	}
}