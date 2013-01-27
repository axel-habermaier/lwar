using System;

namespace Pegasus.Framework.Math
{
	using Math = System.Math;

	/// <summary>
	///   Provides math utility functions.
	/// </summary>
	public static class MathUtils
	{
		/// <summary>
		///   Epsilon value for float-point equality comparisons.
		/// </summary>
		public const float Epsilon = 10e-6f;

		/// <summary>
		///   Represents a 180 degree rotation or the ratio of the circumference of a circle to its diameter.
		/// </summary>
		public const float Pi = (float)Math.PI;

		/// <summary>
		///   Represents a 360 degree rotation.
		/// </summary>
		public const float TwoPi = (float)Math.PI * 2;

		/// <summary>
		///   Represents the value of Pi divided by two, i.e., a 90 dregree rotation.
		/// </summary>
		public const float PiOver2 = (float)Math.PI / 2;

		/// <summary>
		///   Checks whether two float values are equal. If the difference between the two floats is
		///   small enough, they are considered equal.
		/// </summary>
		/// <param name="left">The first value to compare.</param>
		/// <param name="right">The second value to compare.</param>
		public static bool FloatEquality(float left, float right)
		{
			return Math.Abs(left - right) < Epsilon;
		}

		/// <summary>
		///   Clamps the given value to be in the range [min, max].
		/// </summary>
		/// <param name="value">The value that should be clamped.</param>
		/// <param name="min">The lower bound of the clamped interval.</param>
		/// <param name="max">The upper bound of the clamped interval.</param>
		public static float Clamp(float value, float min, float max)
		{
			if (value < min)
				return min;

			if (value > max)
				return max;

			return value;
		}

		/// <summary>
		///   Clamps the given value to be in the range [min, max].
		/// </summary>
		/// <param name="value">The value that should be clamped.</param>
		/// <param name="min">The lower bound of the clamped interval.</param>
		/// <param name="max">The upper bound of the clamped interval.</param>
		public static int Clamp(int value, int min, int max)
		{
			if (value < min)
				return min;

			if (value > max)
				return max;

			return value;
		}

		/// <summary>
		///   Clamps the given value to be in the range [min, max].
		/// </summary>
		/// <param name="value">The value that should be clamped.</param>
		/// <param name="min">The lower bound of the clamped interval.</param>
		/// <param name="max">The upper bound of the clamped interval.</param>
		public static Fixed8 Clamp(Fixed8 value, Fixed8 min, Fixed8 max)
		{
			if (value < min)
				return min;

			if (value > max)
				return max;

			return value;
		}

		/// <summary>
		///   Clamps the given value to be in the range [min, max].
		/// </summary>
		/// <param name="value">The value that should be clamped.</param>
		/// <param name="min">The lower bound of the clamped interval.</param>
		/// <param name="max">The upper bound of the clamped interval.</param>
		public static Fixed16 Clamp(Fixed16 value, Fixed16 min, Fixed16 max)
		{
			if (value < min)
				return min;

			if (value > max)
				return max;

			return value;
		}
	}
}