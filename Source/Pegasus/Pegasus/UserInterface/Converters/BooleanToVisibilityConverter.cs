namespace Pegasus.UserInterface.Converters
{
	using System;

	/// <summary>
	///     Converts a frame time value to a string.
	/// </summary>
	public class BooleanToVisibilityConverter : ValueConverter<BooleanToVisibilityConverter, bool, Visibility>
	{
		/// <summary>
		///     Converts the given Boolean value to the corresponding Visibility literal.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override Visibility ConvertToTarget(bool value)
		{
			return value ? Visibility.Visible : Visibility.Collapsed;
		}

		/// <summary>
		///     Converts the given value to the source type.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public override bool ConvertToSource(Visibility value)
		{
			return value == Visibility.Visible;
		}
	}
}