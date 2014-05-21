namespace Pegasus.Framework.UserInterface.Converters
{
	using System;
	using System.Globalization;

	/// <summary>
	///     Converts an unsigned 64 bit integer value to a string.
	/// </summary>
	public class UInt64ToStringConverter : IValueConverter<ulong, string>
	{
		/// <summary>
		///     Converts the given value to a string.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public string Convert(ulong value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}
	}
}