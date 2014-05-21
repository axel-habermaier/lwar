namespace Pegasus.Framework.UserInterface.Converters
{
	using System;
	using System.Globalization;

	/// <summary>
	///     Converts a signed 64 bit integer value to a string.
	/// </summary>
	public class Int64ToStringConverter : IValueConverter<long, string>
	{
		/// <summary>
		///     Converts the given value to a string.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public string Convert(long value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}
	}
}