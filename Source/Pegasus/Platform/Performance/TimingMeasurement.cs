using System;

namespace Pegasus.Platform.Performance
{
	using System.Text;

	/// <summary>
	///   Represents a timing measurement that is averaged over a certain number of frames.
	/// </summary>
	public class TimingMeasurement : IMeasurement
	{
		/// <summary>
		///   The number of samples for the computation of the average.
		/// </summary>
		private const int AverageSamples = 32;

		/// <summary>
		///   The averaged value of the timing measurements.
		/// </summary>
		private readonly AveragedDouble _value = new AveragedDouble("ms", AverageSamples);

		/// <summary>
		///   The timestamp at the beginning of the measurement.
		/// </summary>
		private double _beginTime;

		/// <summary>
		///   Invoked when the measurement should begin.
		/// </summary>
		public void Begin()
		{
			_beginTime = Clock.SystemTime;
		}

		/// <summary>
		///   Invoked when the measurement should end.
		/// </summary>
		public void End()
		{
			_value.AddMeasurement(1000 * (Clock.SystemTime - _beginTime));
		}

		/// <summary>
		///   Writes the results of the measurement into the given string builder.
		/// </summary>
		/// <param name="builder">The string builder the results should be written to.</param>
		public void WriteResults(StringBuilder builder)
		{
			_value.WriteResults(builder);
		}
	}
}