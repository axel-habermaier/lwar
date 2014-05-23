namespace Pegasus.Framework.UserInterface.Converters
{
	using System;
	using System.Globalization;

	/// <summary>
	///     Converts a signed byte value to a string.
	/// </summary>
	public class SByteToStringConverter : ValueConverter<SByteToStringConverter, sbyte, string>
	{
		/// <summary>
		///     Converts the given value to a string.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override string ConvertToTarget(sbyte value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		///     Converts the given value to the source type.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override sbyte ConvertToSource(string value)
		{
			if (String.IsNullOrWhiteSpace(value))
				return default(sbyte);

			return SByte.Parse(value, CultureInfo.InvariantCulture);
		}
	}
}