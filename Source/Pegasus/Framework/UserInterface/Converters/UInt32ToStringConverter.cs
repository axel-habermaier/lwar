namespace Pegasus.Framework.UserInterface.Converters
{
	using System;
	using System.Globalization;

	/// <summary>
	///     Converts an unsigned 32 bit integer value to a string.
	/// </summary>
	public class UInt32ToStringConverter : ValueConverter<UInt32ToStringConverter, uint, string>
	{
		/// <summary>
		///     Converts the given value to a string.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override string ConvertToTarget(uint value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		///     Converts the given value to the source type.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override uint ConvertToSource(string value)
		{
			return UInt32.Parse(value, CultureInfo.InvariantCulture);
		}
	}
}