namespace Pegasus.Math
{
	using System;
	using System.Globalization;

	/// <summary>
	///     Represents a 32-bit signed fixed-point value, with 8 bits being used for the fractional part of the value.
	/// </summary>
	public struct Fixed8 : IEquatable<Fixed8>, IComparable<Fixed8>, IComparable
	{
		/// <summary>
		///     The number of bits that are used to store the fractional part of the value.
		/// </summary>
		private const int FractionalBits = 8;

		/// <summary>
		///     Represents the largest possible value of the integer part of a fixed-point value.
		/// </summary>
		public const int MaxValue = Int32.MaxValue >> FractionalBits;

		/// <summary>
		///     Represents the smallest possible value of the integer part of a fixed-point value.
		/// </summary>
		public const int MinValue = Int32.MinValue >> FractionalBits;

		/// <summary>
		///     Epsilon value for fixed-point equality comparisons.
		/// </summary>
		public static readonly Fixed8 Epsilon = MathUtils.Epsilon;

		/// <summary>
		///     Represents a 180 degree rotation or the ratio of the circumference of a circle to its diameter.
		/// </summary>
		public static readonly Fixed8 Pi = Math.PI;

		/// <summary>
		///     Represents a 360 degree rotation.
		/// </summary>
		public static readonly Fixed8 TwoPi = Math.PI * 2;

		/// <summary>
		///     Represents the value of Pi divided by two, i.e., a 90 dregree rotation.
		/// </summary>
		public static readonly Fixed8 PiOver2 = Math.PI / 2;

		/// <summary>
		///     The raw value stored as a 32-bit signed integer.
		/// </summary>
		private int _rawValue;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="value">The integer value that should be converted.</param>
		public Fixed8(int value)
		{
			Assert.ArgumentInRange(value, MinValue, MaxValue);
			_rawValue = value << FractionalBits;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="value">The floating-point value that should be converted.</param>
		public Fixed8(float value)
		{
			Assert.ArgumentInRange(value, MinValue, MaxValue);
			_rawValue = (int)Math.Round(value * (1 << FractionalBits));
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="value">The floating-point value that should be converted.</param>
		public Fixed8(double value)
		{
			Assert.ArgumentInRange(value, MinValue, MaxValue);
			_rawValue = (int)Math.Round(value * (1 << FractionalBits));
		}

		/// <summary>
		///     Gets or sets the raw value stored as a 32-bit signed integer.
		/// </summary>
		public int RawValue
		{
			get { return _rawValue; }
			set { _rawValue = value; }
		}

		/// <summary>
		///     Returns the fully qualified type name of this instance.
		/// </summary>
		public override string ToString()
		{
			return ((float)this).ToString(CultureInfo.InvariantCulture);
		}

		#region Cast operators

		/// <summary>
		///     Implicitely casts an integer value to its fixed-point representation.
		/// </summary>
		/// <param name="value">The integer value that should be converted.</param>
		public static implicit operator Fixed8(int value)
		{
			return new Fixed8(value);
		}

		/// <summary>
		///     Implicitely casts a fixed-point value to its integer representation.
		/// </summary>
		/// <param name="value">The fixed-point value that should be converted.</param>
		public static explicit operator int(Fixed8 value)
		{
			return value._rawValue >> FractionalBits;
		}

		/// <summary>
		///     Implicitely casts an floating-point value to its fixed-point representation.
		/// </summary>
		/// <param name="value">The floating-point value that should be converted.</param>
		public static implicit operator Fixed8(float value)
		{
			return new Fixed8(value);
		}

		/// <summary>
		///     Implicitely casts a fixed-point value to its floating-point representation.
		/// </summary>
		/// <param name="value">The fixed-point value that should be converted.</param>
		public static explicit operator float(Fixed8 value)
		{
			return value._rawValue / (float)(1 << FractionalBits);
		}

		/// <summary>
		///     Implicitely casts an floating-point value to its fixed-point representation.
		/// </summary>
		/// <param name="value">The floating-point value that should be converted.</param>
		public static implicit operator Fixed8(double value)
		{
			return new Fixed8(value);
		}

		/// <summary>
		///     Implicitely casts a fixed-point value to its floating-point representation.
		/// </summary>
		/// <param name="value">The fixed-point value that should be converted.</param>
		public static explicit operator double(Fixed8 value)
		{
			return value._rawValue / (double)(1 << FractionalBits);
		}

		#endregion

		#region Equality operators and comparison

		/// <summary>
		///     Compares the current value with the given one.
		/// </summary>
		/// <param name="other">The value this instance should be compared with.</param>
		public int CompareTo(object other)
		{
			if (other == null)
				return 1;

			Assert.ArgumentSatisfies(other is Fixed8, "The given object is not of type 'Fixed8'.");
			return CompareTo((Fixed8)other);
		}

		/// <summary>
		///     Compares the current value with the given one.
		/// </summary>
		/// <param name="other">The value this instance should be compared with.</param>
		public int CompareTo(Fixed8 other)
		{
			return _rawValue.CompareTo(other._rawValue);
		}

		/// <summary>
		///     Indicates whether the current fixed-point value is equal to another fixed-point value.
		/// </summary>
		/// <param name="other">The value to compare with this value.</param>
		public bool Equals(Fixed8 other)
		{
			return _rawValue == other._rawValue;
		}

		/// <summary>
		///     Indicates whether the current fixed-point value is equal to another object.
		/// </summary>
		/// <param name="obj">An object to compare with this value.</param>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			return obj is Fixed8 && Equals((Fixed8)obj);
		}

		/// <summary>
		///     Returns the hash code for this instance.
		/// </summary>
		public override int GetHashCode()
		{
			return _rawValue;
		}

		/// <summary>
		///     Tests for equality between two fixed-point values.
		/// </summary>
		/// <param name="left">The first fixed-point value to compare.</param>
		/// <param name="right">The second fixed-point value to compare.</param>
		public static bool operator ==(Fixed8 left, Fixed8 right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///     Tests for inequality between two fixed-point values.
		/// </summary>
		/// <param name="left">The first fixed-point value to compare.</param>
		/// <param name="right">The second fixed-point value to compare.</param>
		public static bool operator !=(Fixed8 left, Fixed8 right)
		{
			return !left.Equals(right);
		}

		#endregion

		#region Arithmetic operators

		/// <summary>
		///     Adds the two fixed-point values and returns the result.
		/// </summary>
		/// <param name="left">The first fixed-point operand.</param>
		/// <param name="right">The second fixed-point operand.</param>
		public static Fixed8 operator +(Fixed8 left, Fixed8 right)
		{
			return new Fixed8 { _rawValue = left._rawValue + right._rawValue };
		}

		/// <summary>
		///     Subtracts the two fixed-point values and returns the result.
		/// </summary>
		/// <param name="left">The first fixed-point operand.</param>
		/// <param name="right">The second fixed-point operand.</param>
		public static Fixed8 operator -(Fixed8 left, Fixed8 right)
		{
			return new Fixed8 { _rawValue = left._rawValue - right._rawValue };
		}

		/// <summary>
		///     Negates the fixed-point value.
		/// </summary>
		/// <param name="value">The value that should be negated.</param>
		public static Fixed8 operator -(Fixed8 value)
		{
			return new Fixed8 { _rawValue = -value._rawValue };
		}

		/// <summary>
		///     Multiplies the two fixed-point values and returns the result.
		/// </summary>
		/// <param name="left">The first fixed-point operand.</param>
		/// <param name="right">The second fixed-point operand.</param>
		public static Fixed8 operator *(Fixed8 left, Fixed8 right)
		{
			long leftValue = left._rawValue;
			long rightValue = right._rawValue;
			var value = (leftValue * rightValue) >> FractionalBits;

			Assert.InRange(value, ((long)MinValue) << FractionalBits, ((long)MaxValue) << FractionalBits);
			return new Fixed8 { _rawValue = (int)value };
		}

		/// <summary>
		///     Multiplies the fixed-point value and the integer value and returns the result.
		/// </summary>
		/// <param name="left">The fixed-point operand.</param>
		/// <param name="right">The integer operand.</param>
		public static Fixed8 operator *(Fixed8 left, int right)
		{
			return new Fixed8 { _rawValue = left._rawValue * right };
		}

		/// <summary>
		///     Multiplies the fixed-point value and the integer value and returns the result.
		/// </summary>
		/// <param name="left">The integer operand.</param>
		/// <param name="right">The fixed-point operand.</param>
		public static Fixed8 operator *(int left, Fixed8 right)
		{
			return new Fixed8 { _rawValue = left * right._rawValue };
		}

		/// <summary>
		///     Divides the two fixed-point values and returns the result.
		/// </summary>
		/// <param name="left">The first fixed-point operand.</param>
		/// <param name="right">The second fixed-point operand.</param>
		public static Fixed8 operator /(Fixed8 left, Fixed8 right)
		{
			var leftValue = ((long)left._rawValue) << FractionalBits;
			var value = (leftValue / right._rawValue);

			return new Fixed8 { _rawValue = (int)value };
		}

		#endregion

		#region Relational operators

		/// <summary>
		///     Indicates whether the first fixed-point value is smaller than the second one.
		/// </summary>
		/// <param name="left">The first fixed-point operand.</param>
		/// <param name="right">The second fixed-point operand.</param>
		public static bool operator <(Fixed8 left, Fixed8 right)
		{
			return left._rawValue < right._rawValue;
		}

		/// <summary>
		///     Indicates whether the first fixed-point value is smaller than the second one.
		/// </summary>
		/// <param name="left">The first fixed-point operand.</param>
		/// <param name="right">The second fixed-point operand.</param>
		public static bool operator >(Fixed8 left, Fixed8 right)
		{
			return left._rawValue > right._rawValue;
		}

		/// <summary>
		///     Indicates whether the first fixed-point value is smaller than or equal to the second one.
		/// </summary>
		/// <param name="left">The first fixed-point operand.</param>
		/// <param name="right">The second fixed-point operand.</param>
		public static bool operator <=(Fixed8 left, Fixed8 right)
		{
			return left._rawValue <= right._rawValue;
		}

		/// <summary>
		///     Indicates whether the first fixed-point value is smaller than or equal to the second one.
		/// </summary>
		/// <param name="left">The first fixed-point operand.</param>
		/// <param name="right">The second fixed-point operand.</param>
		public static bool operator >=(Fixed8 left, Fixed8 right)
		{
			return left._rawValue >= right._rawValue;
		}

		#endregion

		#region Functions

		/// <summary>
		///     Returns the absolute value of a fixed-point value.
		/// </summary>
		/// <param name="value">The value whose absolute should be returned.</param>
		public static Fixed8 Abs(Fixed8 value)
		{
			if (value < 0)
				return value * -1;

			return value;
		}

		/// <summary>
		///     Computes the square root of the given fixed-point value.
		/// </summary>
		/// <param name="value">The value for which the square root should be computed.</param>
		public static Fixed8 Sqrt(Fixed8 value)
		{
			Assert.ArgumentSatisfies(value._rawValue >= 0, "Value must be greater than 0.");

			if (value == 0)
				return 0;

			// Normalize to range 1 < n <= 4
			int normalizationCount = 0;
			while (true)
			{
				if (value < 1)
				{
					--normalizationCount;
					value *= 4;
				}
				else if (value > 4)
				{
					++normalizationCount;
					value /= 4;
				}
				else
					break;
			}

			// Initial approximation
			var x = (value + 1) / 2;

			// Heron's recurrence equation
			x = (x + value / x) / 2;
			x = (x + value / x) / 2;
			x = (x + value / x) / 2;
			x = (x + value / x) / 2;
			x = (x + value / x) / 2;
			x = (x + value / x) / 2;

			// Denormalize again
			for (var i = 0; i < normalizationCount; ++i)
				x *= 2;

			for (var i = 0; i < -normalizationCount; ++i)
				x /= 2;

			return x;
		}

		#endregion
	}

	/// <summary>
	///     Represents a 32-bit signed fixed-point value, with 16 bits being used for the fractional part of the value.
	/// </summary>
	public struct Fixed16 : IEquatable<Fixed16>, IComparable<Fixed16>, IComparable
	{
		/// <summary>
		///     The number of bits that are used to store the fractional part of the value.
		/// </summary>
		private const int FractionalBits = 16;

		/// <summary>
		///     Represents the largest possible value of the integer part of a fixed-point value.
		/// </summary>
		public const int MaxValue = Int32.MaxValue >> FractionalBits;

		/// <summary>
		///     Represents the smallest possible value of the integer part of a fixed-point value.
		/// </summary>
		public const int MinValue = Int32.MinValue >> FractionalBits;

		/// <summary>
		///     Epsilon value for fixed-point equality comparisons.
		/// </summary>
		public static readonly Fixed16 Epsilon = MathUtils.Epsilon;

		/// <summary>
		///     Represents a 180 degree rotation or the ratio of the circumference of a circle to its diameter.
		/// </summary>
		public static readonly Fixed16 Pi = Math.PI;

		/// <summary>
		///     Represents a 360 degree rotation.
		/// </summary>
		public static readonly Fixed16 TwoPi = Math.PI * 2;

		/// <summary>
		///     Represents the value of Pi divided by two, i.e., a 90 dregree rotation.
		/// </summary>
		public static readonly Fixed16 PiOver2 = Math.PI / 2;

		/// <summary>
		///     The raw value stored as a 32-bit signed integer.
		/// </summary>
		private int _rawValue;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="value">The integer value that should be converted.</param>
		public Fixed16(int value)
		{
			Assert.ArgumentInRange(value, MinValue, MaxValue);
			_rawValue = value << FractionalBits;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="value">The floating-point value that should be converted.</param>
		public Fixed16(float value)
		{
			Assert.ArgumentInRange(value, MinValue, MaxValue);
			_rawValue = (int)Math.Round(value * (1 << FractionalBits));
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="value">The floating-point value that should be converted.</param>
		public Fixed16(double value)
		{
			Assert.ArgumentInRange(value, MinValue, MaxValue);
			_rawValue = (int)Math.Round(value * (1 << FractionalBits));
		}

		/// <summary>
		///     Gets or sets the raw value stored as a 32-bit signed integer.
		/// </summary>
		public int RawValue
		{
			get { return _rawValue; }
			set { _rawValue = value; }
		}

		/// <summary>
		///     Returns the fully qualified type name of this instance.
		/// </summary>
		public override string ToString()
		{
			return ((float)this).ToString(CultureInfo.InvariantCulture);
		}

		#region Cast operators

		/// <summary>
		///     Implicitely casts an integer value to its fixed-point representation.
		/// </summary>
		/// <param name="value">The integer value that should be converted.</param>
		public static implicit operator Fixed16(int value)
		{
			return new Fixed16(value);
		}

		/// <summary>
		///     Implicitely casts a fixed-point value to its integer representation.
		/// </summary>
		/// <param name="value">The fixed-point value that should be converted.</param>
		public static explicit operator int(Fixed16 value)
		{
			return value._rawValue >> FractionalBits;
		}

		/// <summary>
		///     Implicitely casts an floating-point value to its fixed-point representation.
		/// </summary>
		/// <param name="value">The floating-point value that should be converted.</param>
		public static implicit operator Fixed16(float value)
		{
			return new Fixed16(value);
		}

		/// <summary>
		///     Implicitely casts a fixed-point value to its floating-point representation.
		/// </summary>
		/// <param name="value">The fixed-point value that should be converted.</param>
		public static explicit operator float(Fixed16 value)
		{
			return value._rawValue / (float)(1 << FractionalBits);
		}

		/// <summary>
		///     Implicitely casts an floating-point value to its fixed-point representation.
		/// </summary>
		/// <param name="value">The floating-point value that should be converted.</param>
		public static implicit operator Fixed16(double value)
		{
			return new Fixed16(value);
		}

		/// <summary>
		///     Implicitely casts a fixed-point value to its floating-point representation.
		/// </summary>
		/// <param name="value">The fixed-point value that should be converted.</param>
		public static explicit operator double(Fixed16 value)
		{
			return value._rawValue / (double)(1 << FractionalBits);
		}

		#endregion

		#region Equality operators and comparison

		/// <summary>
		///     Compares the current value with the given one.
		/// </summary>
		/// <param name="other">The value this instance should be compared with.</param>
		public int CompareTo(object other)
		{
			if (other == null)
				return 1;

			Assert.ArgumentSatisfies(other is Fixed16, "The given object is not of type 'Fixed16'.");
			return CompareTo((Fixed16)other);
		}

		/// <summary>
		///     Compares the current value with the given one.
		/// </summary>
		/// <param name="other">The value this instance should be compared with.</param>
		public int CompareTo(Fixed16 other)
		{
			return _rawValue.CompareTo(other._rawValue);
		}

		/// <summary>
		///     Indicates whether the current fixed-point value is equal to another fixed-point value.
		/// </summary>
		/// <param name="other">The value to compare with this value.</param>
		public bool Equals(Fixed16 other)
		{
			return _rawValue == other._rawValue;
		}

		/// <summary>
		///     Indicates whether the current fixed-point value is equal to another object.
		/// </summary>
		/// <param name="obj">An object to compare with this value.</param>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			return obj is Fixed16 && Equals((Fixed16)obj);
		}

		/// <summary>
		///     Returns the hash code for this instance.
		/// </summary>
		public override int GetHashCode()
		{
			return _rawValue;
		}

		/// <summary>
		///     Tests for equality between two fixed-point values.
		/// </summary>
		/// <param name="left">The first fixed-point value to compare.</param>
		/// <param name="right">The second fixed-point value to compare.</param>
		public static bool operator ==(Fixed16 left, Fixed16 right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///     Tests for inequality between two fixed-point values.
		/// </summary>
		/// <param name="left">The first fixed-point value to compare.</param>
		/// <param name="right">The second fixed-point value to compare.</param>
		public static bool operator !=(Fixed16 left, Fixed16 right)
		{
			return !left.Equals(right);
		}

		#endregion

		#region Arithmetic operators

		/// <summary>
		///     Adds the two fixed-point values and returns the result.
		/// </summary>
		/// <param name="left">The first fixed-point operand.</param>
		/// <param name="right">The second fixed-point operand.</param>
		public static Fixed16 operator +(Fixed16 left, Fixed16 right)
		{
			return new Fixed16 { _rawValue = left._rawValue + right._rawValue };
		}

		/// <summary>
		///     Subtracts the two fixed-point values and returns the result.
		/// </summary>
		/// <param name="left">The first fixed-point operand.</param>
		/// <param name="right">The second fixed-point operand.</param>
		public static Fixed16 operator -(Fixed16 left, Fixed16 right)
		{
			return new Fixed16 { _rawValue = left._rawValue - right._rawValue };
		}

		/// <summary>
		///     Negates the fixed-point value.
		/// </summary>
		/// <param name="value">The value that should be negated.</param>
		public static Fixed16 operator -(Fixed16 value)
		{
			return new Fixed16 { _rawValue = -value._rawValue };
		}

		/// <summary>
		///     Multiplies the two fixed-point values and returns the result.
		/// </summary>
		/// <param name="left">The first fixed-point operand.</param>
		/// <param name="right">The second fixed-point operand.</param>
		public static Fixed16 operator *(Fixed16 left, Fixed16 right)
		{
			long leftValue = left._rawValue;
			long rightValue = right._rawValue;
			var value = (leftValue * rightValue) >> FractionalBits;

			Assert.InRange(value, ((long)MinValue) << FractionalBits, ((long)MaxValue) << FractionalBits);
			return new Fixed16 { _rawValue = (int)value };
		}

		/// <summary>
		///     Multiplies the fixed-point value and the integer value and returns the result.
		/// </summary>
		/// <param name="left">The fixed-point operand.</param>
		/// <param name="right">The integer operand.</param>
		public static Fixed16 operator *(Fixed16 left, int right)
		{
			return new Fixed16 { _rawValue = left._rawValue * right };
		}

		/// <summary>
		///     Multiplies the fixed-point value and the integer value and returns the result.
		/// </summary>
		/// <param name="left">The integer operand.</param>
		/// <param name="right">The fixed-point operand.</param>
		public static Fixed16 operator *(int left, Fixed16 right)
		{
			return new Fixed16 { _rawValue = left * right._rawValue };
		}

		/// <summary>
		///     Divides the two fixed-point values and returns the result.
		/// </summary>
		/// <param name="left">The first fixed-point operand.</param>
		/// <param name="right">The second fixed-point operand.</param>
		public static Fixed16 operator /(Fixed16 left, Fixed16 right)
		{
			var leftValue = ((long)left._rawValue) << FractionalBits;
			var value = (leftValue / right._rawValue);

			return new Fixed16 { _rawValue = (int)value };
		}

		#endregion

		#region Relational operators

		/// <summary>
		///     Indicates whether the first fixed-point value is smaller than the second one.
		/// </summary>
		/// <param name="left">The first fixed-point operand.</param>
		/// <param name="right">The second fixed-point operand.</param>
		public static bool operator <(Fixed16 left, Fixed16 right)
		{
			return left._rawValue < right._rawValue;
		}

		/// <summary>
		///     Indicates whether the first fixed-point value is smaller than the second one.
		/// </summary>
		/// <param name="left">The first fixed-point operand.</param>
		/// <param name="right">The second fixed-point operand.</param>
		public static bool operator >(Fixed16 left, Fixed16 right)
		{
			return left._rawValue > right._rawValue;
		}

		/// <summary>
		///     Indicates whether the first fixed-point value is smaller than or equal to the second one.
		/// </summary>
		/// <param name="left">The first fixed-point operand.</param>
		/// <param name="right">The second fixed-point operand.</param>
		public static bool operator <=(Fixed16 left, Fixed16 right)
		{
			return left._rawValue <= right._rawValue;
		}

		/// <summary>
		///     Indicates whether the first fixed-point value is smaller than or equal to the second one.
		/// </summary>
		/// <param name="left">The first fixed-point operand.</param>
		/// <param name="right">The second fixed-point operand.</param>
		public static bool operator >=(Fixed16 left, Fixed16 right)
		{
			return left._rawValue >= right._rawValue;
		}

		#endregion

		#region Functions

		/// <summary>
		///     Returns the absolute value of a fixed-point value.
		/// </summary>
		/// <param name="value">The value whose absolute should be returned.</param>
		public static Fixed16 Abs(Fixed16 value)
		{
			if (value < 0)
				return value * -1;

			return value;
		}

		/// <summary>
		///     Computes the square root of the given fixed-point value.
		/// </summary>
		/// <param name="value">The value for which the square root should be computed.</param>
		public static Fixed16 Sqrt(Fixed16 value)
		{
			Assert.ArgumentSatisfies(value._rawValue >= 0, "Value must be greater than 0.");

			if (value == 0)
				return 0;

			// Normalize to range 1 < n <= 4
			int normalizationCount = 0;
			while (true)
			{
				if (value < 1)
				{
					--normalizationCount;
					value *= 4;
				}
				else if (value > 4)
				{
					++normalizationCount;
					value /= 4;
				}
				else
					break;
			}

			// Initial approximation
			var x = (value + 1) / 2;

			// Heron's recurrence equation
			x = (x + value / x) / 2;
			x = (x + value / x) / 2;
			x = (x + value / x) / 2;
			x = (x + value / x) / 2;
			x = (x + value / x) / 2;
			x = (x + value / x) / 2;

			// Denormalize again
			for (var i = 0; i < normalizationCount; ++i)
				x *= 2;

			for (var i = 0; i < -normalizationCount; ++i)
				x /= 2;

			return x;
		}

		#endregion
	}
}