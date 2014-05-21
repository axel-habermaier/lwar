namespace Pegasus.Framework.UserInterface.Converters
{
	using System;
	using System.Globalization;

	/// <summary>
	///     Converts a character value to a string.
	/// </summary>
	public class CharToStringConverter : IValueConverter<char, string>
	{
		/// <summary>
		///     Converts the given value to a string.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public string Convert(char value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}
	}
}