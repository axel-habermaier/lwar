namespace Pegasus.Math
{
	using System;

	/// <summary>
	///     Configures a particle property of an emitter.
	/// </summary>
	/// <typeparam name="T">The type of the configured property.</typeparam>
	public struct Range<T>
	{
		/// <summary>
		///     The inclusive lower bound of the range. For multi-component values such as vectors or colors, this
		///     is the lower bound value per component.
		/// </summary>
		public readonly T LowerBound;

		/// <summary>
		///     The inclusive upper bound of the range. For multi-component values such as vectors or colors, this
		///     is the upper bound value per component.
		/// </summary>
		public readonly T UpperBound;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="lowerBoundValue">
		///     The inclusive lower bound of the range. For multi-component values such as vectors or colors, this
		///     is the lower bound value per component.
		/// </param>
		/// <param name="upperBoundValue">
		///     The inclusive upper bound of the range. For multi-component values such as vectors or colors, this
		///     is the upper bound value per component.
		/// </param>
		public Range(T lowerBoundValue, T upperBoundValue)
		{
			LowerBound = lowerBoundValue;
			UpperBound = upperBoundValue;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="bounds">
		///     The lower and upper bound of the range. For multi-component values such as vectors or colors, this
		///     is the lower and upper bound per component.
		/// </param>
		public Range(T bounds)
		{
			LowerBound = bounds;
			UpperBound = bounds;
		}
	}
}