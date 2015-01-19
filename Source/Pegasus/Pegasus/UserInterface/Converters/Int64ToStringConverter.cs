namespace Pegasus.UserInterface.Converters
{
	using System;
	using System.Globalization;

	/// <summary>
	///     Converts a signed 64 bit integer value to a string.
	/// </summary>
	public class Int64ToStringConverter : ValueConverter<Int64ToStringConverter, long, string>
	{
		/// <summary>
		///     Converts the given value to a string.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override string ConvertToTarget(long value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		///     Converts the given value to the source type.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override long ConvertToSource(string value)
		{
			if (String.IsNullOrWhiteSpace(value))
				return default(long);

			return Int64.Parse(value, CultureInfo.InvariantCulture);
		}
	}
}