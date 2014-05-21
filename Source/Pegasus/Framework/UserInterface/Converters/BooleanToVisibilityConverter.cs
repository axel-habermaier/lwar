namespace Pegasus.Framework.UserInterface.Converters
{
	using System;

	/// <summary>
	///     Converts a frame time value to a string.
	/// </summary>
	public class BooleanToVisibilityConverter : IValueConverter<bool, Visibility>
	{
		/// <summary>
		///     Converts the given Boolean value to the corresponding Visibility literal.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public Visibility Convert(bool value)
		{
			return value ? Visibility.Visible : Visibility.Collapsed;
		}
	}
}