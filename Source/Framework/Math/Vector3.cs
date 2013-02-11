using System;

namespace Pegasus.Framework.Math
{
	using System.Globalization;
	using System.Runtime.InteropServices;
	using Math = System.Math;

	/// <summary>
	///   Represents a three-component vector.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector3 : IEquatable<Vector3>
	{
		/// <summary>
		///   A vector with all components set to zero.
		/// </summary>
		public static readonly Vector3 Zero = new Vector3(0.0f, 0.0f, 0.0f);

		/// <summary>
		///   The X-component of the vector.
		/// </summary>
		public float X;

		/// <summary>
		///   The Y-component of the vector.
		/// </summary>
		public float Y;

		/// <summary>
		///   The Z-component of the vector.
		/// </summary>
		public float Z;

		/// <summary>
		///   Initializes a new instance.
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
		///   Determines whether the given vector is equal to this vector.
		/// </summary>
		/// <param name="other">The other vector to compare with this vector.</param>
		public bool Equals(Vector3 other)
		{
			return MathUtils.FloatEquality(X, other.X) &&
				   MathUtils.FloatEquality(Y, other.Y) &&
				   MathUtils.FloatEquality(Z, other.Z);
		}

		/// <summary>
		///   Determines whether the specified object is equal to this vector.
		/// </summary>
		/// <param name="value">The object to compare with this vector.</param>
		public override bool Equals(object value)
		{
			if (value == null)
				return false;

			if (!ReferenceEquals(value.GetType(), typeof(Vector4)))
				return false;

			return Equals((Vector4)value);
		}

		/// <summary>
		///   Returns a hash code for this vector.
		/// </summary>
		public override int GetHashCode()
		{
			return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
			;
		}

		/// <summary>
		///   Returns a string representation of this vector.
		/// </summary>
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "X: {0}, Y: {1}, Z: {2}", X, Y, Z);
		}

		/// <summary>
		///   Tests for equality between two vectors.
		/// </summary>
		/// <param name="left">The first vector to compare.</param>
		/// <param name="right">The second vector to compare.</param>
		public static bool operator ==(Vector3 left, Vector3 right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///   Tests for inequality between two vectors.
		/// </summary>
		/// <param name="left">The first vector to compare.</param>
		/// <param name="right">The second vector to compare.</param>
		public static bool operator !=(Vector3 left, Vector3 right)
		{
			return !(left == right);
		}

		/// <summary>
		///   Performs a vector addition.
		/// </summary>
		/// <param name="left">The first vector to add.</param>
		/// <param name="right">The second vector to add.</param>
		public static Vector3 operator +(Vector3 left, Vector3 right)
		{
			return new Vector3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
		}

		/// <summary>
		///   Performs a vector subtraction.
		/// </summary>
		/// <param name="left">The first vector.</param>
		/// <param name="right">The second vector.</param>
		public static Vector3 operator -(Vector3 left, Vector3 right)
		{
			return new Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
		}

		/// <summary>
		///   Multiplies a vector with a scalar value.
		/// </summary>
		/// <param name="vector">The vector that should be multiplied.</param>
		/// <param name="factor">The scalar value the vector should be multiplied with.</param>
		public static Vector3 operator *(Vector3 vector, float factor)
		{
			return new Vector3(vector.X * factor, vector.Y * factor, vector.Z * factor);
		}

		/// <summary>
		///   Multiplies a vector with a scalar value.
		/// </summary>
		/// <param name="factor">The scalar value the vector should be multiplied with.</param>
		/// <param name="vector">The vector that should be multiplied.</param>
		public static Vector3 operator *(float factor, Vector3 vector)
		{
			return vector * factor;
		}

		/// <summary>
		///   Gets the length of the vector.
		/// </summary>
		public float Length
		{
			get { return (float)Math.Sqrt(SquaredLength); }
		}

		/// <summary>
		///   Gets the squared length of the vector.
		/// </summary>
		public float SquaredLength
		{
			get { return X * X + Y * Y + Z * Z; }
		}

		/// <summary>
		///   Constructs a new vector instance that is normalized to a length of 1, but still points into the same direction.
		/// </summary>
		public Vector3 Normalize()
		{
			var length = Length;
			if (length < MathUtils.Epsilon)
				length = MathUtils.Epsilon;

			return new Vector3(X / length, Y / length, Z / length);
		}

		/// <summary>
		///   Calculates the cross product between the two vectors.
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
		///   Calculates the dot product between the two vectors.
		/// </summary>
		/// <param name="v1">The first vector.</param>
		/// <param name="v2">The second vector.</param>
		public static float Dot(Vector3 v1, Vector3 v2)
		{
			return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
		}

		/// <summary>
		///   Applies the given transformation matrix to the vector.
		/// </summary>
		/// <param name="vector">The vector that should be transformed.</param>
		/// <param name="matrix">The transformation matrix that should be applied.</param>
		public static Vector4 Transform(ref Vector3 vector, ref Matrix matrix)
		{
			var v = new Vector4(vector);
			return Vector4.Transform(ref v, ref matrix);
		}
	}
}