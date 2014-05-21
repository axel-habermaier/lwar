namespace Pegasus.Framework.UserInterface.Converters
{
	using System;
	using System.Globalization;

	/// <summary>
	///     Converts a signed 32 bit integer value to a string.
	/// </summary>
	public class Int32ToStringConverter : ValueConverter<Int32ToStringConverter, int, string>
	{
		/// <summary>
		///     Converts the given value to a string.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override string Convert(int value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}
	}
}