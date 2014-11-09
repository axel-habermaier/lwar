namespace Pegasus.UserInterface.Converters
{
	using System;

	/// <summary>
	///     Converts values from the given source type to the given target type.
	/// </summary>
	/// <typeparam name="T">The type of the target value.</typeparam>
	public interface IValueConverter<out T>
	{
		/// <summary>
		///     Converts the given value to the target type.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		T ConvertToTarget(object value);
	}
}