namespace Pegasus.Framework.UserInterface.Converters
{
	using System;
	using System.Globalization;

	/// <summary>
	///     Converts a frame time value to a string.
	/// </summary>
	public class FrameTimeToStringConverter : ValueConverter<FrameTimeToStringConverter, double, string>
	{
		/// <summary>
		///     Converts the given value to a string.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override string ConvertToTarget(double value)
		{
			return value.ToString("F2", CultureInfo.InvariantCulture);
		}

		/// <summary>
		///     Converts the given value to the source type.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override double ConvertToSource(string value)
		{
			return Double.Parse(value, CultureInfo.InvariantCulture);
		}
	}
}