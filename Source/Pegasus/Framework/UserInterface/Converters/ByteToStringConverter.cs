namespace Pegasus.Framework.UserInterface.Converters
{
	using System;
	using System.Globalization;

	/// <summary>
	///     Converts an unsigned byte value to a string.
	/// </summary>
	public class ByteToStringConverter : IValueConverter<byte, string>
	{
		/// <summary>
		///     Converts the given value to a string.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public string Convert(byte value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}
	}
}