namespace Pegasus.Framework.UserInterface.Converters
{
	using System;
	using System.Globalization;

	/// <summary>
	///     Converts a 32 bit floating point value to a string.
	/// </summary>
	public class SingleToStringConverter : ValueConverter<SingleToStringConverter, float, string>
	{
		/// <summary>
		///     Converts the given value to a string.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override string ConvertToTarget(float value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		///     Converts the given value to the source type.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override float ConvertToSource(string value)
		{
			if (String.IsNullOrWhiteSpace(value))
				return default(float);

			return Single.Parse(value, CultureInfo.InvariantCulture);
		}
	}
}