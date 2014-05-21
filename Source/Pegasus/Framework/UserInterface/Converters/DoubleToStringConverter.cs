namespace Pegasus.Framework.UserInterface.Converters
{
	using System;
	using System.Globalization;

	/// <summary>
	///     Converts a 64 bit floating point value to a string.
	/// </summary>
	public class DoubleToStringConverter : IValueConverter<double, string>
	{
		/// <summary>
		///     Gets or sets the format of the conversion to a string.
		/// </summary>
		public string Format { get; set; }

		/// <summary>
		///     Converts the given value to a string.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public string Convert(double value)
		{
			if (String.IsNullOrWhiteSpace(Format))
				return value.ToString(CultureInfo.InvariantCulture);

			return value.ToString(Format, CultureInfo.InvariantCulture);
		}
	}
}