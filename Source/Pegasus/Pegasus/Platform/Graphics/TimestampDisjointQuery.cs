namespace Pegasus.Platform.Graphics
{
	using System;
	using Utilities;

	/// <summary>
	///     Represents a query that records the frequency of the GPU timer that can be used to interpret the result of timestamp
	///     queries. It also indicates whether a GPU event invalidates all timestamp results.
	/// </summary>
	public unsafe class TimestampDisjointQuery : Query
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

			TimestampDisjointQueryResult data;
			DeviceInterface->GetQueryData(NativeObject, &data);

			frequency = data.Frequency;
			return !data.Disjoint;
		}

		/// <summary>
		///     Begins the timestamp disjoint query.
		/// </summary>
		public void Begin()
		{
			Assert.NotDisposed(this);
			DeviceInterface->BeginQuery(NativeObject);
		}

		/// <summary>
		///     Ends the timestamp disjoint query.
		/// </summary>
		public void End()
		{
			Assert.NotDisposed(this);
			DeviceInterface->EndQuery(NativeObject);
		}
	}
}