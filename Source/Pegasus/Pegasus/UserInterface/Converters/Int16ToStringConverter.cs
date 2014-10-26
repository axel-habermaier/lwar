namespace Pegasus.UserInterface.Converters
{
	using System;
	using System.Globalization;

	/// <summary>
	///     Converts a signed 16 bit integer value to a string.
	/// </summary>
	public class Int16ToStringConverter : ValueConverter<Int16ToStringConverter, short, string>
	{
		/// <summary>
		///     Converts the given value to a string.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override string ConvertToTarget(short value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		///     Converts the given value to the source type.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override short ConvertToSource(string value)
		{
			if (String.IsNullOrWhiteSpace(value))
				return default(short);

			return Int16.Parse(value, CultureInfo.InvariantCulture);
		}
	}
}