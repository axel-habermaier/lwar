namespace Pegasus.Framework.UserInterface.Converters
{
	using System;
	using System.Globalization;

	/// <summary>
	///     Converts a signed byte value to a string.
	/// </summary>
	public class SByteToStringConverter : IValueConverter<sbyte, string>
	{
		/// <summary>
		///     Converts the given value to a string.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public string Convert(sbyte value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}
	}
}