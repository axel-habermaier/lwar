namespace Pegasus.Framework.UserInterface.Converters
{
	using System;

	/// <summary>
	///     Converts values from the given source type to the given target type.
	/// </summary>
	/// <typeparam name="TSource">The source type of the conversion.</typeparam>
	/// <typeparam name="TTarget">The target type of the conversion.</typeparam>
	public interface IValueConverter<TSource, TTarget> : IValueConverter
	{
		/// <summary>
		///     Converts the given value to the target type.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		TTarget ConvertToTarget(TSource value);

		/// <summary>
		///     Converts the given value to the source type.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		TSource ConvertToSource(TTarget value);
	}
}