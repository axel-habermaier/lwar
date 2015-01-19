namespace Pegasus.UserInterface.Converters
{
	using System;

	/// <summary>
	///     Converts a character value to a string.
	/// </summary>
	public class CharToStringConverter : ValueConverter<CharToStringConverter, char, string>
	{
		/// <summary>
		///     Converts the given value to a string.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override string ConvertToTarget(char value)
		{
			return value.ToString();
		}

		/// <summary>
		///     Converts the given value to the source type.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override char ConvertToSource(string value)
		{
			if (String.IsNullOrWhiteSpace(value))
				return default(char);

			return value[0];
		}
	}
}