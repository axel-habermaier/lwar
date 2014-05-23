namespace Pegasus.Framework.UserInterface.Converters
{
	using System;
	using System.Globalization;

	/// <summary>
	///     Converts an unsigned 16 bit integer value to a string.
	/// </summary>
	public class UInt16ToStringConverter : ValueConverter<UInt16ToStringConverter, ushort, string>
	{
		/// <summary>
		///     Converts the given value to a string.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override string ConvertToTarget(ushort value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		///     Converts the given value to the source type.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override ushort ConvertToSource(string value)
		{
			if (String.IsNullOrWhiteSpace(value))
				return default(ushort);

			return UInt16.Parse(value, CultureInfo.InvariantCulture);
		}
	}
}