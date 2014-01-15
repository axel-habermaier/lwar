namespace Pegasus.Platform.Performance
{
	using System;
	using System.Text;

	/// <summary>
	///     Represents a measurement that is averaged over a certain number of samples.
	/// </summary>
	internal class AveragedInteger
	{
		/// <summary>
		///     The last couple of values for a more stable average.
		/// </summary>
		private readonly int[] _average;

		/// <summary>
		///     The number of samples for the computation of the average.
		/// </summary>
		private readonly int _sampleCount;

		/// <summary>
		///     The current write index in the average array (circular writes).
		/// </summary>
		private int _averageIndex;

		/// <summary>
		///     A value indicating whether the entire average array has been filled at least once.
		/// </summary>
		private bool _averageIsFilled;

		/// <summary>
		///     The last value that has been measured.
		/// </summary>
		private int _last;

		/// <summary>
		///     The maximum supported value.
		/// </summary>
		private int _max = Int32.MinValue;

		/// <summary>
		///     The minimum supported value.
		/// </summary>
		private int _min = Int32.MaxValue;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="sampleCount">The number of samples for the computation of the average.</param>
		public AveragedInteger(int sampleCount)
		{
			_sampleCount = sampleCount;
			_average = new int[sampleCount];
		}

		/// <summary>
		///     Writes the results of the measurement into the given string builder.
		/// </summary>
		/// <param name="builder">The string builder the results should be written to.</param>
		internal void WriteResults(StringBuilder builder)
		{
			double average = 0;
			var count = _averageIsFilled ? _sampleCount : _averageIndex;

			for (var i = 0; i < count; ++i)
				average += _average[i];

			average /= count;

			if (count == 0)
			{
				builder.Append("-");
				return;
			}

			// Using Append is more efficient than using AppendFormat
			builder.Append((int)average).Append(" (");
			builder.Append(_min).Append("/");
			builder.Append(_last).Append("/");
			builder.Append(_max).Append(")");
		}

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			var builder = new StringBuilder();
			WriteResults(builder);
			return builder.ToString();
		}

		/// <summary>
		///     Adds the given measured value to the statistics.
		/// </summary>
		/// <param name="value">The value that should be added.</param>
		internal void AddMeasurement(int value)
		{
			_last = value;

			if (_last > _max)
				_max = _last;
			if (_last < _min)
				_min = _last;

			_average[_averageIndex] = _last;
			_averageIndex = (_averageIndex + 1) % _sampleCount;

			if (_averageIndex == 0)
				_averageIsFilled = true;
		}
	}
}