using System;

namespace Pegasus.Framework
{
	using System.Diagnostics;

	/// <summary>
	///   Keeps track of the game time.
	/// </summary>
	public sealed class GameTime
	{
		/// <summary>
		///   The stopwatch that is used for time measurements.
		/// </summary>
		private readonly Stopwatch _stopwatch = new Stopwatch();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		internal GameTime()
		{
			_stopwatch.Start();
		}

		/// <summary>
		///   Gets the elapsed real-time in milliseconds.
		/// </summary>
		internal double RealTime { get; private set; }

		/// <summary>
		///   Gets the elapsed real-time in milliseconds since the last update.
		/// </summary>
		internal double RealElapsedTime { get; private set; }

		/// <summary>
		///   Gets the elapsed time in milliseconds.
		/// </summary>
		public double Time { get; internal set; }

		/// <summary>
		///   Gets the elapsed time in milliseconds since the last update.
		/// </summary>
		public double ElapsedTime { get; internal set; }

		/// <summary>
		///   Updates the internal state.
		/// </summary>
		internal void Update()
		{
			var now = RealTime + _stopwatch.Elapsed.TotalMilliseconds;
			RealElapsedTime = now - RealTime;
			RealTime = now;

			_stopwatch.Reset();
			_stopwatch.Start();
		}
	}
}