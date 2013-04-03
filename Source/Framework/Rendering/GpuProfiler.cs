using System;

namespace Pegasus.Framework.Rendering
{
	using System.Text;
	using System.Threading;
	using Platform;
	using Platform.Graphics;

	/// <summary>
	///   Manages timestamp queries to profile the time the GPU spends on rendering each frame.
	/// </summary>
	internal class GpuProfiler : DisposableObject, IMeasurement
	{
		/// <summary>
		///   The number of queries that is used to compute the frame time. As the CPU and the GPU work asynchronously,
		///   the results of the queries are not immediately available. In order to avoid stalling the CPU, the results
		///   of the queries are checked BufferSize frames later.
		/// </summary>
		private const int BufferSize = 3;

		/// <summary>
		///   The timestamp queries that mark the beginning of a frame.
		/// </summary>
		private readonly TimestampQuery[] _beginQueries = new TimestampQuery[BufferSize];

		/// <summary>
		///   The timestamp disjoint queries that are used to check whether the timestamps are valid and that allow the
		///   correct interpretation of the timestamp values.
		/// </summary>
		private readonly TimestampDisjointQuery[] _disjointQueries = new TimestampDisjointQuery[BufferSize];

		/// <summary>
		///   The timestamp queries that mark the end of a frame.
		/// </summary>
		private readonly TimestampQuery[] _endQueries = new TimestampQuery[BufferSize];

		/// <summary>
		///   Gets the statistical render time information.
		/// </summary>
		private readonly AveragedValue _renderTime;

		/// <summary>
		///   The index denoting the queries whose results that are checked next.
		/// </summary>
		private int _index;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be profiled.</param>
		public GpuProfiler(GraphicsDevice graphicsDevice)
		{
			_renderTime = new AveragedValue();

			for (var i = 0; i < BufferSize; ++i)
			{
				_beginQueries[i] = new TimestampQuery(graphicsDevice);
				_beginQueries[i].SetName("GpuProfiler.BeginQuery" + i);
			}
				

			for (var i = 0; i < BufferSize; ++i)
			{
				_endQueries[i] = new TimestampQuery(graphicsDevice);
				_endQueries[i].SetName("GpuProfiler.EndQuery" + i);
			}

			for (var i = 0; i < BufferSize; ++i)
			{
				_disjointQueries[i] = new TimestampDisjointQuery(graphicsDevice);
				_disjointQueries[i].SetName("GpuProfiler.DisjointQuery" + i);
			}
		}

		/// <summary>
		///   Gets the index of the queries that should be issued during the current frame.
		/// </summary>
		private int StartIndex
		{
			get { return (_index + (BufferSize - 1)) % BufferSize; }
		}

		/// <summary>
		///   Gets the index of the queries whose results should be checked during the current frame.
		/// </summary>
		private int ResultIndex
		{
			get { return _index % BufferSize; }
		}

		/// <summary>
		///   Marks the beginning of a new frame.
		/// </summary>
		public void Begin()
		{
			_disjointQueries[StartIndex].Begin();
			_beginQueries[StartIndex].Query();
		}

		/// <summary>
		///   Marks the end of the current frame.
		/// </summary>
		public void End()
		{
			_endQueries[StartIndex].Query();
			_disjointQueries[StartIndex].End();

			// Skip the first couple of frames for which we do not have any queued queries
			if (_index >= BufferSize - 1)
			{
				// If the queries are not yet available, we have to stall the CPU
				while (!_disjointQueries[ResultIndex].DataAvailable || !_endQueries[ResultIndex].DataAvailable)
					Thread.Sleep(0);

				// The timestamps might be invalid if the GPU changed its clockrate, for instance
				var result = _disjointQueries[ResultIndex].Result;
				if (!result.Valid)
					return;

				var value = (_endQueries[ResultIndex].Timestamp - _beginQueries[ResultIndex].Timestamp) / (double)result.Frequency * 1000;
				_renderTime.AddMeasurement(value);
			}

			++_index;
		}

		/// <summary>
		///   Writes the results of the measurement into the given string builder.
		/// </summary>
		/// <param name="builder">The string builder the results should be written to.</param>
		public void WriteResults(StringBuilder builder)
		{
			_renderTime.WriteResults(builder);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			for (var i = 0; i < BufferSize; ++i)
				_beginQueries[i].SafeDispose();

			for (var i = 0; i < BufferSize; ++i)
				_endQueries[i].SafeDispose();

			for (var i = 0; i < BufferSize; ++i)
				_disjointQueries[i].SafeDispose();
		}
	}
}