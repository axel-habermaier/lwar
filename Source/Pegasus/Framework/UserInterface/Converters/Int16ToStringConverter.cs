namespace Pegasus.Framework.UserInterface.Converters
{
	using System;
	using System.Globalization;

	/// <summary>
	///     Converts a signed 16 bit integer value to a string.
	/// </summary>
	public class Int16ToStringConverter : IValueConverter<short, string>
	{
		/// <summary>
		///     Converts the given value to a string.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public string Convert(short value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}
	}
}