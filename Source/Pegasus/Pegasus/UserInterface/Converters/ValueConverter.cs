namespace Pegasus.UserInterface.Converters
{
	using System;

	/// <summary>
	///     Converts values from the given source type to the given target type.
	/// </summary>
	/// <typeparam name="TConverter">The actual type of the converter.</typeparam>
	/// <typeparam name="TSource">The source type of the conversion.</typeparam>
	/// <typeparam name="TTarget">The target type of the conversion.</typeparam>
	public abstract class ValueConverter<TConverter, TSource, TTarget> : IValueConverter<TSource, TTarget>
		where TConverter : ValueConverter<TConverter, TSource, TTarget>, new()
	{
		/// <summary>
		///     The singleton instance of the converter.
		/// </summary>
		private static TConverter _converter;

		/// <summary>
		///     Gets the singleton instance of the converter.
		/// </summary>
		/// <remarks>The implementation is not thread-safe as UI elements must always be accessed on the main thread anyway.</remarks>
		public static TConverter Instance
		{
			get { return _converter ?? (_converter = new TConverter()); }
		}

		/// <summary>
		///     Converts the given value to the target type.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public abstract TTarget ConvertToTarget(TSource value);

		/// <summary>
		///     Converts the given value to the source type.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public abstract TSource ConvertToSource(TTarget value);
	}
}