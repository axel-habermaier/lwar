using System;

namespace Pegasus.Platform.Performance
{
	/// <summary>
	///   Represents a measurement that can be used with C#'s using keyword to define the scope of the measurement.
	/// </summary>
	public struct Measurement : IDisposable
	{
		/// <summary>
		///   The measurement for which a new measurement is being done.
		/// </summary>
		private readonly IMeasurement _measurement;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="measurement">The measurement for which a new measurement should be done.</param>
		public Measurement(IMeasurement measurement)
		{
			_measurement = measurement;
			_measurement.Begin();
		}

		/// <summary>
		///   Ends the measurement. The name Dispose is only required to make the type usable with C#'s
		///   using syntax. Calling Dispose ends the measurement, but does not actually dispose anything.
		/// </summary>
		public void Dispose()
		{
			_measurement.End();
		}
	}
}