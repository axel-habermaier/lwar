namespace Pegasus.Math
{
	using System;
	using System.Globalization;
	using System.Runtime.InteropServices;
	using Utilities;

	/// <summary>
	///     Represents a three-component vector.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector3 : IEquatable<Vector3>, IFormattable
	{
		/// <summary>
		///     A vector with all components set to zero.
		/// </summary>
		public static readonly Vector3 Zero = new Vector3(0.0f, 0.0f, 0.0f);

		/// <summary>
		///     The X-component of the vector.
		/// </summary>
		public float X;

		/// <summary>
		///     The Y-component of the vector.
		/// </summary>
		public float Y;

		/// <summary>
		///     The Z-component of the vector.
		/// </summary>
		public float Z;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="x">The X-component of the vector.</param>
		/// <param name="y">The Y-component of the vector.</param>
		/// <param name="z">The Z-component of the vector.</param>
		public Vector3(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="value">The value of the X, Y, and Z components of the vector.</param>
		public Vector3(float value)
		{
			X = value;
			Y = value;
			Z = value;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="v">The X and Y components of the vector.</param>
		/// <param name="z">The Z-component of the vector.</param>
		public Vector3(Vector2 v, float z)
		{
			X = v.X;
			Y = v.Y;
			Z = z;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="v">The X, Y, and Z components of the vector. The W component is discarded.</param>
		public Vector3(Vector4 v)
		{
			X = v.X;
			Y = v.Y;
			Z = v.Z;
		}

		/// <summary>
		///     Determines whether the given vector is equal to this vector.
		/// </summary>
		/// <param name="other">The other vector to compare with this vector.</param>
		public bool Equals(Vector3 other)
		{
			return MathUtils.Equals(X, other.X) &&
				   MathUtils.Equals(Y, other.Y) &&
				   MathUtils.Equals(Z, other.Z);
		}

		/// <summary>
		///     Determines whether the specified object is equal to this vector.
		/// </summary>
		/// <param name="value">The object to compare with this vector.</param>
		public override bool Equals(object value)
		{
			if (value == null)
				return false;

			if (!ReferenceEquals(value.GetType(), typeof(Vector3)))
				return false;

			return Equals((Vector3)value);
		}

		/// <summary>
		///     Returns a hash code for this vector.
		/// </summary>
		public override int GetHashCode()
		{
			return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
		}

		/// <summary>
		///     Formats the value of the current instance using the specified format.
		/// </summary>
		/// <param name="format">The format to use.</param>
		/// <param name="formatProvider">The provider to use to format the value</param>
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return String.Format(formatProvider, String.Format("X: {{0:{0}}}, Y: {{1:{0}}}, Z: {{2:{0}}}", format), X, Y, Z);
		}

		/// <summary>
		///     Returns a string representation of this vector.
		/// </summary>
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "X: {0}, Y: {1}, Z: {2}", X, Y, Z);
		}

		/// <summary>
		///     Tests for equality between two vectors.
		/// </summary>
		/// <param name="left">The first vector to compare.</param>
		/// <param name="right">The second vector to compare.</param>
		public static bool operator ==(Vector3 left, Vector3 right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///     Tests for inequality between two vectors.
		/// </summary>
		/// <param name="left">The first vector to compare.</param>
		/// <param name="right">The second vector to compare.</param>
		public static bool operator !=(Vector3 left, Vector3 right)
		{
			return !(left == right);
		}

		/// <summary>
		///     Performs a vector addition.
		/// </summary>
		/// <param name="left">The first vector to add.</param>
		/// <param name="right">The second vector to add.</param>
		public static Vector3 operator +(Vector3 left, Vector3 right)
		{
			return new Vector3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
		}

		/// <summary>
		///     Performs a vector subtraction.
		/// </summary>
		/// <param name="left">The first vector.</param>
		/// <param name="right">The second vector.</param>
		public static Vector3 operator -(Vector3 left, Vector3 right)
		{
			return new Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
		}

		/// <summary>
		///     Negates the components of a vector.
		/// </summary>
		/// <param name="vector">The vector whose components should be negated.</param>
		public static Vector3 operator -(Vector3 vector)
		{
			return new Vector3(-vector.X, -vector.Y, -vector.Z);
		}

		/// <summary>
		///     Multiplies a vector with a scalar value.
		/// </summary>
		/// <param name="vector">The vector that should be multiplied.</param>
		/// <param name="factor">The scalar value the vector should be multiplied with.</param>
		public static Vector3 operator *(Vector3 vector, float factor)
		{
			return new Vector3(vector.X * factor, vector.Y * factor, vector.Z * factor);
		}

		/// <summary>
		///     Multiplies a vector with a scalar value.
		/// </summary>
		/// <param name="factor">The scalar value the vector should be multiplied with.</param>
		/// <param name="vector">The vector that should be multiplied.</param>
		public static Vector3 operator *(float factor, Vector3 vector)
		{
			return vector * factor;
		}

		/// <summary>
		///     Divides a vector by a scalar value.
		/// </summary>
		/// <param name="vector">The vector that should be divided.</param>
		/// <param name="factor">The scalar value the vector should be divided by.</param>
		public static Vector3 operator /(Vector3 vector, float factor)
		{
			return new Vector3(vector.X / factor, vector.Y / factor, vector.Z / factor);
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
			get { return X * X + Y * Y + Z * Z; }
		}

		/// <summary>
		///     Constructs a new vector instance that is normalized to a length of 1, but still points into the same direction.
		/// </summary>
		[Pure]
		public Vector3 Normalize()
		{
			var length = Length;
			if (length < MathUtils.Epsilon)
				length = MathUtils.Epsilon;

			return new Vector3(X / length, Y / length, Z / length);
		}

		/// <summary>
		///     Calculates the cross product between the two vectors.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		public static Vector3 Cross(Vector3 v1, Vector3 v2)
		{
			return new Vector3
			{
				X = v1.Y * v2.Z - v1.Z * v2.Y,
				Y = v1.Z * v2.X - v1.X * v2.Z,
				Z = v1.X * v2.Y - v1.Y * v2.X
			};
		}

		/// <summary>
		///     Calculates the dot product between the two vectors.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		public static float Dot(Vector3 v1, Vector3 v2)
		{
			return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
		}

		/// <summary>
		///     Applies the given transformation matrix to the vector.
		/// </summary>
		/// <param name="vector">The vector that should be transformed.</param>
		/// <param name="matrix">The transformation matrix that should be applied.</param>
		public static Vector3 Transform(ref Vector3 vector, ref Matrix matrix)
		{
			return new Vector3(
				matrix.M11 * vector.X + matrix.M21 * vector.Y + matrix.M31 * vector.Z + matrix.M41,
				matrix.M12 * vector.X + matrix.M22 * vector.Y + matrix.M32 * vector.Z + matrix.M42,
				matrix.M13 * vector.X + matrix.M23 * vector.Y + matrix.M33 * vector.Z + matrix.M43);
		}

		/// <summary>
		///     Applies the given transformation matrix to the vector.
		/// </summary>
		/// <param name="vector">The vector that should be transformed.</param>
		/// <param name="matrix">The transformation matrix that should be applied.</param>
		/// <param name="result">The vector that stores the result of the transformation.</param>
		public static void Transform(ref Vector3 vector, ref Matrix matrix, out Vector3 result)
		{
			result = new Vector3(
				matrix.M11 * vector.X + matrix.M21 * vector.Y + matrix.M31 * vector.Z + matrix.M41,
				matrix.M12 * vector.X + matrix.M22 * vector.Y + matrix.M32 * vector.Z + matrix.M42,
				matrix.M13 * vector.X + matrix.M23 * vector.Y + matrix.M33 * vector.Z + matrix.M43);
		}
	}
}