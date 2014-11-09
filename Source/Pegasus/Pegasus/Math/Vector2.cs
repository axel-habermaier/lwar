namespace Pegasus.Math
{
	using System;
	using System.Globalization;
	using System.Runtime.InteropServices;
	using Utilities;

	/// <summary>
	///     Represents a two-component vector.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector2 : IEquatable<Vector2>, IFormattable
	{
		/// <summary>
		///     A vector with all components set to zero.
		/// </summary>
		public static readonly Vector2 Zero = new Vector2(0, 0);

		/// <summary>
		///     The X-component of the vector.
		/// </summary>
		public float X;

		/// <summary>
		///     The Y-component of the vector.
		/// </summary>
		public float Y;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="value">The value of both the X- and Y-components of the vector.</param>
		public Vector2(float value)
		{
			X = value;
			Y = value;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="x">The X-component of the vector.</param>
		/// <param name="y">The Y-component of the vector.</param>
		public Vector2(float x, float y)
		{
			X = x;
			Y = y;
		}

		/// <summary>
		///     Gets the X-component of the vector rounded to the nearest integral value.
		/// </summary>
		public int IntegralX
		{
			get { return MathUtils.RoundIntegral(X); }
		}

		/// <summary>
		///     Gets the Y-component of the vector rounded to the nearest integral value.
		/// </summary>
		public int IntegralY
		{
			get { return MathUtils.RoundIntegral(Y); }
		}

		/// <summary>
		///     Gets the length of the vector.
		/// </summary>
		public float Length
		{
			get { return (float)Math.Sqrt(LengthSquared); }
		}

		/// <summary>
		///     Gets the squared length of the vector.
		/// </summary>
		public float LengthSquared
		{
			get { return X * X + Y * Y; }
		}

		/// <summary>
		///     Constructs a new vector instance that is normalized to a length of 1, but still points into the same direction.
		/// </summary>
		[Pure]
		public Vector2 Normalize()
		{
			var length = Length;
			if (length < MathUtils.Epsilon)
				length = MathUtils.Epsilon;

			return new Vector2(X / length, Y / length);
		}

		/// <summary>
		///     Determines whether the given vector is equal to this vector.
		/// </summary>
		/// <param name="other">The other vector to compare with this vector.</param>
		public bool Equals(Vector2 other)
		{
			return MathUtils.Equals(X, other.X) && MathUtils.Equals(Y, other.Y);
		}

		/// <summary>
		///     Determines whether the specified object is equal to this vector.
		/// </summary>
		/// <param name="value">The object to compare with this vector.</param>
		public override bool Equals(object value)
		{
			if (value == null)
				return false;

			if (!ReferenceEquals(value.GetType(), typeof(Vector2)))
				return false;

			return Equals((Vector2)value);
		}

		/// <summary>
		///     Returns a hash code for this vector.
		/// </summary>
		public override int GetHashCode()
		{
			return (X.GetHashCode() * 397) ^ Y.GetHashCode();
		}

		/// <summary>
		///     Formats the value of the current instance using the specified format.
		/// </summary>
		/// <param name="format">The format to use.</param>
		/// <param name="formatProvider">The provider to use to format the value</param>
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return String.Format(formatProvider, String.Format("X: {{0:{0}}}, Y: {{1:{0}}}", format), X, Y);
		}

		/// <summary>
		///     Returns a string representation of this vector.
		/// </summary>
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "X: {0}, Y: {1}", X, Y);
		}

		/// <summary>
		///     Tests for equality between two vectors.
		/// </summary>
		/// <param name="left">The first vector to compare.</param>
		/// <param name="right">The second vector to compare.</param>
		public static bool operator ==(Vector2 left, Vector2 right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///     Tests for inequality between two vectors.
		/// </summary>
		/// <param name="left">The first vector to compare.</param>
		/// <param name="right">The second vector to compare.</param>
		public static bool operator !=(Vector2 left, Vector2 right)
		{
			return !(left == right);
		}

		/// <summary>
		///     Performs a vector addition.
		/// </summary>
		/// <param name="left">The first vector to add.</param>
		/// <param name="right">The second vector to add.</param>
		public static Vector2 operator +(Vector2 left, Vector2 right)
		{
			return new Vector2(left.X + right.X, left.Y + right.Y);
		}

		/// <summary>
		///     Negates the components of a vector.
		/// </summary>
		/// <param name="vector">The vector whose components should be negated.</param>
		public static Vector2 operator -(Vector2 vector)
		{
			return new Vector2(-vector.X, -vector.Y);
		}

		/// <summary>
		///     Performs a vector subtraction.
		/// </summary>
		/// <param name="left">The first vector.</param>
		/// <param name="right">The second vector.</param>
		public static Vector2 operator -(Vector2 left, Vector2 right)
		{
			return new Vector2(left.X - right.X, left.Y - right.Y);
		}

		/// <summary>
		///     Scales the vector by the given factor.
		/// </summary>
		/// <param name="vector">The vector that should be scaled.</param>
		/// <param name="factor">The factor that should be applied.</param>
		public static Vector2 operator *(Vector2 vector, float factor)
		{
			return new Vector2(vector.X * factor, vector.Y * factor);
		}

		/// <summary>
		///     Scales the vector by the given factor.
		/// </summary>
		/// <param name="factor">The factor that should be applied.</param>
		/// <param name="vector">The vector that should be scaled.</param>
		public static Vector2 operator *(float factor, Vector2 vector)
		{
			return new Vector2(factor * vector.X, factor * vector.Y);
		}

		/// <summary>
		///     Divides the vector by a scalar value.
		/// </summary>
		/// <param name="vector">The vector that should be divided.</param>
		/// <param name="factor">The scalar value the vector should be divided by.</param>
		public static Vector2 operator /(Vector2 vector, float factor)
		{
			return new Vector2(vector.X / factor, vector.Y / factor);
		}

		/// <summary>
		///     Computes the dot product of the two vectors.
		/// </summary>
		/// <param name="left">The first vector.</param>
		/// <param name="right">The second vector.</param>
		public static float Dot(Vector2 left, Vector2 right)
		{
			return left.X * right.X + left.Y * right.Y;
		}

		/// <summary>
		///     Applies the given transformation matrix to the vector.
		/// </summary>
		/// <param name="vector">The vector that should be transformed.</param>
		/// <param name="matrix">The transformation matrix that should be applied.</param>
		public static Vector2 Transform(ref Vector2 vector, ref Matrix matrix)
		{
			return new Vector2(
				matrix.M11 * vector.X + matrix.M21 * vector.Y + matrix.M41,
				matrix.M12 * vector.X + matrix.M22 * vector.Y + matrix.M42);
		}

		/// <summary>
		///     Applies the given transformation matrix to the vector.
		/// </summary>
		/// <param name="vector">The vector that should be transformed.</param>
		/// <param name="matrix">The transformation matrix that should be applied.</param>
		/// <param name="result">The vector that stores the result of the transformation.</param>
		public static void Transform(ref Vector2 vector, ref Matrix matrix, out Vector2 result)
		{
			result = new Vector2(
				matrix.M11 * vector.X + matrix.M21 * vector.Y + matrix.M41,
				matrix.M12 * vector.X + matrix.M22 * vector.Y + matrix.M42);
		}

		/// <summary>
		///     Clamps the given vector to be in the range [min, max] for each component of the vector.
		/// </summary>
		/// <param name="value">The value that should be clamped.</param>
		/// <param name="min">The lower bound of the clamped interval.</param>
		/// <param name="max">The upper bound of the clamped interval.</param>
		public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max)
		{
			return new Vector2(MathUtils.Clamp(value.X, min.X, max.X), MathUtils.Clamp(value.Y, min.Y, max.Y));
		}

		/// <summary>
		///     Rotates the given vector by the given angle.
		/// </summary>
		/// <param name="vector">The vector that should be rotated.</param>
		/// <param name="angle">The rotation angle in radians.</param>
		public static Vector2 Rotate(Vector2 vector, float angle)
		{
			var cos = MathUtils.Cos(angle);
			var sin = -MathUtils.Sin(angle);
			return new Vector2(vector.X * cos - vector.Y * sin, vector.X * sin + vector.Y * cos);
		}

		/// <summary>
		///     Scales the length of the vector to the given length.
		/// </summary>
		/// <param name="vector">The vector that should be scaled.</param>
		/// <param name="length">The length of the scaled vector.</param>
		public static Vector2 ScaleTo(Vector2 vector, float length)
		{
			return vector * (length / vector.Length);
		}

		/// <summary>
		///     Scales the length of the vector to the given length.
		/// </summary>
		/// <param name="length">The length of the scaled vector.</param>
		public Vector2 ScaleTo(float length)
		{
			return this * (length / Length);
		}

		/// <summary>
		///     Gets the squared distance between two vectors.
		/// </summary>
		/// <param name="vector1">The first vector the squared distance should be computed for.</param>
		/// <param name="vector2">The second vector the squared distance should be computed for.</param>
		public static float DistanceSquared(Vector2 vector1, Vector2 vector2)
		{
			return (vector1 - vector2).LengthSquared;
		}

		/// <summary>
		///     Gets the distance between two vectors.
		/// </summary>
		/// <param name="vector1">The first vector the distance should be computed for.</param>
		/// <param name="vector2">The second vector the distance should be computed for.</param>
		public static float Distance(Vector2 vector1, Vector2 vector2)
		{
			return (vector1 - vector2).Length;
		}

		/// <summary>
		///     Computes the angle in radians from the given vector.
		/// </summary>
		/// <param name="vector">The vector the angle should be computed from.</param>
		public static float ToAngle(Vector2 vector)
		{
			return MathUtils.Atan2(-vector.Y, vector.X);
		}

		/// <summary>
		///     Computes the vector from the given angle.
		/// </summary>
		/// <param name="angle">The angle in radians the vector should be computed from.</param>
		public static Vector2 FromAngle(float angle)
		{
			return new Vector2(MathUtils.Cos(angle), -MathUtils.Sin(angle));
		}
	}
}