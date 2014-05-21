namespace Pegasus.Framework.UserInterface.Converters
{
	using System;
	using System.Globalization;

	/// <summary>
	///     Converts an unsigned 64 bit integer value to a string.
	/// </summary>
	public class UInt64ToStringConverter : ValueConverter<UInt64ToStringConverter, ulong, string>
	{
		/// <summary>
		///     Converts the given value to a string.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override string ConvertToTarget(ulong value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		///     Converts the given value to the source type.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override ulong ConvertToSource(string value)
		{
			return UInt64.Parse(value, CultureInfo.InvariantCulture);
		}
	}
}