namespace Pegasus.Framework.UserInterface.Converters
{
	using System;
	using System.Globalization;

	/// <summary>
	///     Converts a 64 bit floating point value to a string.
	/// </summary>
	public class DoubleToStringConverter : ValueConverter<DoubleToStringConverter, double, string>
	{
		/// <summary>
		///     Converts the given value to a string.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override string Convert(double value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}
	}
}