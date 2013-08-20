using System;

namespace Pegasus.Framework.Platform.Performance
{
	using System.Text;

	/// <summary>
	///   Represents a measurement that is averaged over a certain number of samples.
	/// </summary>
	internal class AveragedDouble
	{
		/// <summary>
		///   The last couple of values for a more stable average.
		/// </summary>
		private readonly double[] _average;

		/// <summary>
		///   The number of samples for the computation of the average.
		/// </summary>
		private readonly int _sampleCount;

		/// <summary>
		///   The unit of measure of the averaged values that is shown when printing the value.
		/// </summary>
		private readonly string _unitOfMeasure;

		/// <summary>
		///   The current write index in the average array (circular writes).
		/// </summary>
		private int _averageIndex;

		/// <summary>
		///   A value indicating whether the entire average array has been filled at least once.
		/// </summary>
		private bool _averageIsFilled;

		/// <summary>
		///   The last value that has been measured.
		/// </summary>
		private double _last;

		/// <summary>
		///   The maximum supported value.
		/// </summary>
		private double _max = Double.MinValue;

		/// <summary>
		///   The minimum supported value.
		/// </summary>
		private double _min = Double.MaxValue;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="unitOfMeasure">The unit of measure of the averaged values that should be shown when printing the value.</param>
		/// <param name="sampleCount">The number of samples for the computation of the average.</param>
		public AveragedDouble(string unitOfMeasure, int sampleCount)
		{
			Assert.ArgumentNotNull(unitOfMeasure);

			_unitOfMeasure = unitOfMeasure;
			_sampleCount = sampleCount;

			_average = new double[sampleCount];
		}

		/// <summary>
		///   Writes the results of the measurement into the given string builder.
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
			builder.Append(average.ToString("F2")).Append(_unitOfMeasure).Append(" (");
			builder.Append(_min.ToString("F2")).Append(_unitOfMeasure).Append("/");
			builder.Append(_last.ToString("F2")).Append(_unitOfMeasure).Append("/");
			builder.Append(_max.ToString("F2")).Append(_unitOfMeasure).Append(")");
		}

		/// <summary>
		///   Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			var builder = new StringBuilder();
			WriteResults(builder);
			return builder.ToString();
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
			_averageIndex = (_averageIndex + 1) % _sampleCount;

			if (_averageIndex == 0)
				_averageIsFilled = true;
		}
	}
}