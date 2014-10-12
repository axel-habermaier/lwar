namespace Pegasus.Rendering.Particles
{
	using System;

	/// <summary>
	///     A random number generator that uses the FastRand algorithm to generate random values.
	/// </summary>
	internal static class RandomValues
	{
		/// <summary>
		///     The internal state that is used to determine the next random value.
		/// </summary>
		private static int _state = 1;

		/// <summary>
		///     Gets the next random integer value.
		/// </summary>
		public static int NextInteger()
		{
			_state = 214013 * _state + 2531011;
			return (_state >> 16) & 0x7FFF;
		}

		/// <summary>
		///     Gets the next random integer value which is greater than zero and less than or equal to
		///     the specified maxmimum value.
		/// </summary>
		/// <param name="max">The maximum random integer value to return.</param>
		public static int NextInteger(int max)
		{
			return (int)(max * NextSingle());
		}

		/// <summary>
		///     Gets the next random integer between the specified minimum and maximum values.
		/// </summary>
		/// <param name="min">The inclusive minimum value.</param>
		/// <param name="max">The inclusive maximum value.</param>
		public static int NextInteger(int min, int max)
		{
			return (int)((max - min) * NextSingle()) + min;
		}

		/// <summary>
		///     Gets the next random integer between the specified minimum and maximum values.
		/// </summary>
		/// <param name="min">The inclusive minimum value.</param>
		/// <param name="max">The inclusive maximum value.</param>
		public static byte NextByte(byte min, byte max)
		{
			return (byte)((max - min) * NextSingle() + min);
		}

		/// <summary>
		///     Gets the next random single value in the range [0,1].
		/// </summary>
		public static float NextSingle()
		{
			return NextInteger() / (float)Int16.MaxValue;
		}

		/// <summary>
		///     Gets the next random single value which is greater than zero and less than or equal to
		///     the specified maxmimum value.
		/// </summary>
		/// <param name="max">The maximum random single value to return.</param>
		public static float NextSingle(float max)
		{
			return max * NextSingle();
		}

		/// <summary>
		///     Gets the next random single value between the specified minimum and maximum values.
		/// </summary>
		/// <param name="min">The inclusive minimum value.</param>
		/// <param name="max">The inclusive maximum value.</param>
		public static float NextSingle(float min, float max)
		{
			return ((max - min) * NextSingle()) + min;
		}
	}
}