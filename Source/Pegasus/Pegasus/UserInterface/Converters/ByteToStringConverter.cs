namespace Pegasus.UserInterface.Converters
{
	using System;
	using System.Globalization;

	/// <summary>
	///     Converts an unsigned byte value to a string.
	/// </summary>
	public class ByteToStringConverter : ValueConverter<ByteToStringConverter, byte, string>
	{
		/// <summary>
		///     Converts the given value to a string.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override string ConvertToTarget(byte value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		///     Converts the given value to the source type.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override byte ConvertToSource(string value)
		{
			if (String.IsNullOrWhiteSpace(value))
				return default(byte);

			return Byte.Parse(value, CultureInfo.InvariantCulture);
		}
	}
}