using System;

namespace Pegasus.Framework.Platform
{
	using System.Text;

	/// <summary>
	///   Represents a measurement that is averaged over a certain number of frames.
	/// </summary>
	public class AveragedValue : IMeasurement
	{
		/// <summary>
		///   The number of samples for the computation of the average.
		/// </summary>
		private const int AverageCount = 32;

		/// <summary>
		///   The last couple of values for a more stable average.
		/// </summary>
		private readonly double[] _average = new double[AverageCount];

		/// <summary>
		///   The current write index in the average array (circular writes).
		/// </summary>
		private int _averageIndex;

		/// <summary>
		///   A value indicating whether the entire average array has been filled at least once.
		/// </summary>
		private bool _averageIsFilled;

		/// <summary>
		///   The timestamp at the beginning of the measurement.
		/// </summary>
		private double _beginTime;

		/// <summary>
		///   The last value.
		/// </summary>
		private double _last;

		/// <summary>
		///   The maximum value.
		/// </summary>
		private double _max = -1000;

		/// <summary>
		///   The minimum value.
		/// </summary>
		private double _min = 1000;

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
			AddMeasurement(1000 * (Clock.SystemTime - _beginTime));
		}

		/// <summary>
		///   Writes the results of the measurement into the given string builder.
		/// </summary>
		/// <param name="builder">The string builder the results should be written to.</param>
		public void WriteResults(StringBuilder builder)
		{
			double average = 0;
			var count = _averageIsFilled ? AverageCount : _averageIndex;

			for (var i = 0; i < count; ++i)
				average += _average[i];

			average /= count;

			// Using Append is more efficient than using AppendFormat as profiling shows...
			builder.Append(average.ToString("F2")).Append("ms (");
			builder.Append(_min.ToString("F2")).Append("ms/");
			builder.Append(_last.ToString("F2")).Append("ms/");
			builder.Append(_max.ToString("F2")).Append("ms)");
		}

		/// <summary>
		///   Adds the given measured value to the statistics.
		/// </summary>
		/// <param name="value">The value that should be added.</param>
		internal void AddMeasurement(double value)
		{
			_last = value;

			if (_last > _max)
				_max = _last;
			if (_last < _min)
				_min = _last;

			_average[_averageIndex] = _last;
			_averageIndex = (_averageIndex + 1) % AverageCount;

			if (_averageIndex == 0)
				_averageIsFilled = true;
		}
	}
}