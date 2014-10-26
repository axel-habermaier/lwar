namespace Pegasus.Utilities
{
	using System;
	using Platform;

	/// <summary>
	///     Allows CPU time measurements using C#'s using blocks.
	/// </summary>
	internal unsafe struct TimeMeasurement : IDisposable
	{
		/// <summary>
		///     A pointer to a variable that stores the measured CPU time in milliseconds.
		/// </summary>
		private double* _measuredTime;

		/// <summary>
		///     The starting time of the measurement in seconds.
		/// </summary>
		private double _startTime;

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Assert.NotNull(new IntPtr(_measuredTime));
			*_measuredTime = (Clock.SystemTime - _startTime) * 1000;
		}

		/// <summary>
		///     Measures the CPU time until the Dispose method is called.
		/// </summary>
		/// <param name="measuredTime">A pointer to a variable that should store the measured CPU time in milliseconds.</param>
		internal static TimeMeasurement Measure(double* measuredTime)
		{
			Assert.ArgumentNotNull(new IntPtr(measuredTime));
			return new TimeMeasurement { _startTime = Clock.SystemTime, _measuredTime = measuredTime };
		}
	}
}