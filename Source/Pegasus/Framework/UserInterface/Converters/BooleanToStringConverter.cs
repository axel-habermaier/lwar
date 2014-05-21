namespace Pegasus.Framework.UserInterface.Converters
{
	using System;

	/// <summary>
	///     Converts a Boolean value to a string.
	/// </summary>
	public class BooleanToStringConverter : ValueConverter<BooleanToStringConverter, bool, string>
	{
		/// <summary>
		///     Converts the given value to a string.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override string ConvertToTarget(bool value)
		{
			return value ? "true" : "false";
		}

		/// <summary>
		///     Converts the given value to the source type.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override bool ConvertToSource(string value)
		{
			return value == "true" ? true : false;
		}
	}
}