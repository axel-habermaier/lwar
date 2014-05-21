namespace Pegasus.Framework.UserInterface.Converters
{
	using System;
	using System.Globalization;

	/// <summary>
	///     Converts an unsigned 16 bit integer value to a string.
	/// </summary>
	public class UInt16ToStringConverter : IValueConverter<ushort, string>
	{
		/// <summary>
		///     Converts the given value to a string.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public string Convert(ushort value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}
	}
}