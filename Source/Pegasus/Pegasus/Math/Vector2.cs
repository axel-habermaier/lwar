namespace Pegasus.Math
{
	using System;
	using System.Globalization;
	using System.Runtime.InteropServices;

	/// <summary>
	///     Represents a two-component vector.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector2 : IEquatable<Vector2>
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
			get { return (float)Math.Sqrt(SquaredLength); }
		}

		/// <summary>
		///     Gets the squared length of the vector.
		/// </summary>
		public float SquaredLength
		{
			get { return X * X + Y * Y; }
		}

		/// <summary>
		///     Constructs a new vector instance that is normalized to a length of 1, but still points into the same direction.
		/// </summary>
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
			return new Vector2(matrix.M11 * vector.X + matrix.M21 * vector.Y + matrix.M41,
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
			result = new Vector2(matrix.M11 * vector.X + matrix.M21 * vector.Y + matrix.M41,
				matrix.M12 * vector.X + matrix.M22 * vector.Y + matrix.M42);
		}
	}
}