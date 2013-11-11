// ReSharper disable InconsistentNaming

using System;

namespace Pegasus.Math
{
	using System.Globalization;
	using System.Runtime.InteropServices;
	using Math = System.Math;

	/// <summary>
	///   Represents a two-component vector of 32-bit floating point values.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector2 : IEquatable<Vector2>
	{
		/// <summary>
		///   A vector with all components set to zero.
		/// </summary>
		public static readonly Vector2 Zero = new Vector2(0, 0);

		/// <summary>
		///   The X-component of the vector.
		/// </summary>
		public float X;

		/// <summary>
		///   The Y-component of the vector.
		/// </summary>
		public float Y;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="x">The X-component of the vector.</param>
		/// <param name="y">The Y-component of the vector.</param>
		public Vector2(float x, float y)
		{
			X = x;
			Y = y;
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
			get { return X * X + Y * Y; }
		}

		/// <summary>
		///   Constructs a new vector instance that is normalized to a length of 1, but still points into the same direction.
		/// </summary>
		public Vector2 Normalize()
		{
			var length = Length;
			if (length < MathUtils.Epsilon)
				length = MathUtils.Epsilon;

			return new Vector2(X / length, Y / length);
		}

		/// <summary>
		///   Determines whether the given vector is equal to this vector.
		/// </summary>
		/// <param name="other">The other vector to compare with this vector.</param>
		public bool Equals(Vector2 other)
		{
			return MathUtils.FloatEquality(X, other.X) && MathUtils.FloatEquality(Y, other.Y);
		}

		/// <summary>
		///   Determines whether the specified object is equal to this vector.
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
		///   Returns a hash code for this vector.
		/// </summary>
		public override int GetHashCode()
		{
			return (X.GetHashCode() * 397) ^ Y.GetHashCode();
		}

		/// <summary>
		///   Returns a string representation of this vector.
		/// </summary>
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "X: {0}, Y: {1}", X, Y);
		}

		/// <summary>
		///   Tests for equality between two vectors.
		/// </summary>
		/// <param name="left">The first vector to compare.</param>
		/// <param name="right">The second vector to compare.</param>
		public static bool operator ==(Vector2 left, Vector2 right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///   Tests for inequality between two vectors.
		/// </summary>
		/// <param name="left">The first vector to compare.</param>
		/// <param name="right">The second vector to compare.</param>
		public static bool operator !=(Vector2 left, Vector2 right)
		{
			return !(left == right);
		}

		/// <summary>
		///   Performs a vector addition.
		/// </summary>
		/// <param name="left">The first vector to add.</param>
		/// <param name="right">The second vector to add.</param>
		public static Vector2 operator +(Vector2 left, Vector2 right)
		{
			return new Vector2(left.X + right.X, left.Y + right.Y);
		}

		/// <summary>
		///   Negates the components of a vector.
		/// </summary>
		/// <param name="vector">The vector whose components should be negated.</param>
		public static Vector2 operator -(Vector2 vector)
		{
			return new Vector2(-vector.X, -vector.Y);
		}

		/// <summary>
		///   Performs a vector subtraction.
		/// </summary>
		/// <param name="left">The first vector.</param>
		/// <param name="right">The second vector.</param>
		public static Vector2 operator -(Vector2 left, Vector2 right)
		{
			return new Vector2(left.X - right.X, left.Y - right.Y);
		}

		/// <summary>
		///   Scales the vector by the given factor.
		/// </summary>
		/// <param name="vector">The vector that should be scaled.</param>
		/// <param name="factor">The factor that should be applied.</param>
		public static Vector2 operator *(Vector2 vector, float factor)
		{
			return new Vector2(vector.X * factor, vector.Y * factor);
		}

		/// <summary>
		///   Scales the vector by the given factor.
		/// </summary>
		/// <param name="factor">The factor that should be applied.</param>
		/// <param name="vector">The vector that should be scaled.</param>
		public static Vector2 operator *(float factor, Vector2 vector)
		{
			return new Vector2(factor * vector.X, factor * vector.Y);
		}

		/// <summary>
		///   Divides the vector by a scalar value.
		/// </summary>
		/// <param name="vector">The vector that should be divided.</param>
		/// <param name="factor">The scalar value the vector should be divided by.</param>
		public static Vector2 operator /(Vector2 vector, float factor)
		{
			return new Vector2(vector.X / factor, vector.Y / factor);
		}

		/// <summary>
		///   Computes the dot product of the two vectors.
		/// </summary>
		/// <param name="left">The first vector.</param>
		/// <param name="right">The second vector.</param>
		public static float Dot(Vector2 left, Vector2 right)
		{
			return left.X * right.X + left.Y * right.Y;
		}

		/// <summary>
		///   Applies the given transformation matrix to the vector.
		/// </summary>
		/// <param name="vector">The vector that should be transformed.</param>
		/// <param name="matrix">The transformation matrix that should be applied.</param>
		public static Vector2 Transform(ref Vector2 vector, ref Matrix matrix)
		{
			var vector4 = new Vector4(vector.X, vector.Y, 0);
			vector4 = Vector4.Transform(ref vector4, ref matrix);
			return new Vector2(vector4.X, vector4.Y);
		}
	}

	/// <summary>
	///   Represents a two-component vector of 64-bit floating point values.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector2d : IEquatable<Vector2d>
	{
		/// <summary>
		///   A vector with all components set to zero.
		/// </summary>
		public static readonly Vector2d Zero = new Vector2d(0, 0);

		/// <summary>
		///   The X-component of the vector.
		/// </summary>
		public double X;

		/// <summary>
		///   The Y-component of the vector.
		/// </summary>
		public double Y;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="x">The X-component of the vector.</param>
		/// <param name="y">The Y-component of the vector.</param>
		public Vector2d(double x, double y)
		{
			X = x;
			Y = y;
		}

		/// <summary>
		///   Gets the length of the vector.
		/// </summary>
		public double Length
		{
			get { return Math.Sqrt(SquaredLength); }
		}

		/// <summary>
		///   Gets the squared length of the vector.
		/// </summary>
		public double SquaredLength
		{
			get { return X * X + Y * Y; }
		}

		/// <summary>
		///   Constructs a new vector instance that is normalized to a length of 1, but still points into the same direction.
		/// </summary>
		public Vector2d Normalize()
		{
			var length = Length;
			if (length < MathUtils.Epsilon)
				length = MathUtils.Epsilon;

			return new Vector2d(X / length, Y / length);
		}

		/// <summary>
		///   Determines whether the given vector is equal to this vector.
		/// </summary>
		/// <param name="other">The other vector to compare with this vector.</param>
		public bool Equals(Vector2d other)
		{
			return MathUtils.DoubleEquality(X, other.X) && MathUtils.DoubleEquality(Y, other.Y);
		}

		/// <summary>
		///   Determines whether the specified object is equal to this vector.
		/// </summary>
		/// <param name="value">The object to compare with this vector.</param>
		public override bool Equals(object value)
		{
			if (value == null)
				return false;

			if (!ReferenceEquals(value.GetType(), typeof(Vector2d)))
				return false;

			return Equals((Vector2d)value);
		}

		/// <summary>
		///   Returns a hash code for this vector.
		/// </summary>
		public override int GetHashCode()
		{
			return (X.GetHashCode() * 397) ^ Y.GetHashCode();
		}

		/// <summary>
		///   Returns a string representation of this vector.
		/// </summary>
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "X: {0}, Y: {1}", X, Y);
		}

		/// <summary>
		///   Tests for equality between two vectors.
		/// </summary>
		/// <param name="left">The first vector to compare.</param>
		/// <param name="right">The second vector to compare.</param>
		public static bool operator ==(Vector2d left, Vector2d right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///   Tests for inequality between two vectors.
		/// </summary>
		/// <param name="left">The first vector to compare.</param>
		/// <param name="right">The second vector to compare.</param>
		public static bool operator !=(Vector2d left, Vector2d right)
		{
			return !(left == right);
		}

		/// <summary>
		///   Performs a vector addition.
		/// </summary>
		/// <param name="left">The first vector to add.</param>
		/// <param name="right">The second vector to add.</param>
		public static Vector2d operator +(Vector2d left, Vector2d right)
		{
			return new Vector2d(left.X + right.X, left.Y + right.Y);
		}

		/// <summary>
		///   Negates the components of a vector.
		/// </summary>
		/// <param name="vector">The vector whose components should be negated.</param>
		public static Vector2d operator -(Vector2d vector)
		{
			return new Vector2d(-vector.X, -vector.Y);
		}

		/// <summary>
		///   Performs a vector subtraction.
		/// </summary>
		/// <param name="left">The first vector.</param>
		/// <param name="right">The second vector.</param>
		public static Vector2d operator -(Vector2d left, Vector2d right)
		{
			return new Vector2d(left.X - right.X, left.Y - right.Y);
		}

		/// <summary>
		///   Scales the vector by the given factor.
		/// </summary>
		/// <param name="vector">The vector that should be scaled.</param>
		/// <param name="factor">The factor that should be applied.</param>
		public static Vector2d operator *(Vector2d vector, double factor)
		{
			return new Vector2d(vector.X * factor, vector.Y * factor);
		}

		/// <summary>
		///   Scales the vector by the given factor.
		/// </summary>
		/// <param name="factor">The factor that should be applied.</param>
		/// <param name="vector">The vector that should be scaled.</param>
		public static Vector2d operator *(double factor, Vector2d vector)
		{
			return new Vector2d(factor * vector.X, factor * vector.Y);
		}

		/// <summary>
		///   Divides the vector by a scalar value.
		/// </summary>
		/// <param name="vector">The vector that should be divided.</param>
		/// <param name="factor">The scalar value the vector should be divided by.</param>
		public static Vector2d operator /(Vector2d vector, double factor)
		{
			return new Vector2d(vector.X / factor, vector.Y / factor);
		}

		/// <summary>
		///   Computes the dot product of the two vectors.
		/// </summary>
		/// <param name="left">The first vector.</param>
		/// <param name="right">The second vector.</param>
		public static double Dot(Vector2d left, Vector2d right)
		{
			return left.X * right.X + left.Y * right.Y;
		}
	}

	/// <summary>
	///   Represents a two-component vector of 32-bit signed integer values.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector2i : IEquatable<Vector2i>
	{
		/// <summary>
		///   A vector with all components set to zero.
		/// </summary>
		public static readonly Vector2i Zero = new Vector2i(0, 0);

		/// <summary>
		///   The X-component of the vector.
		/// </summary>
		public int X;

		/// <summary>
		///   The Y-component of the vector.
		/// </summary>
		public int Y;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="x">The X-component of the vector.</param>
		/// <param name="y">The Y-component of the vector.</param>
		public Vector2i(int x, int y)
		{
			X = x;
			Y = y;
		}

		/// <summary>
		///   Gets the length of the vector.
		/// </summary>
		public Fixed16 Length
		{
			get { return Fixed16.Sqrt(SquaredLength); }
		}

		/// <summary>
		///   Gets the squared length of the vector.
		/// </summary>
		public int SquaredLength
		{
			get { return X * X + Y * Y; }
		}

		/// <summary>
		///   Constructs a new vector instance that is normalized to a length of 1, but still points into the same direction.
		/// </summary>
		public Vector2f16 Normalize()
		{
			var length = Length;
			if (length < Fixed16.Epsilon)
				length = Fixed16.Epsilon;

			return new Vector2f16(X / length, Y / length);
		}

		/// <summary>
		///   Determines whether the given vector is equal to this vector.
		/// </summary>
		/// <param name="other">The other vector to compare with this vector.</param>
		public bool Equals(Vector2i other)
		{
			return X == other.X && Y == other.Y;
		}

		/// <summary>
		///   Determines whether the specified object is equal to this vector.
		/// </summary>
		/// <param name="value">The object to compare with this vector.</param>
		public override bool Equals(object value)
		{
			if (value == null)
				return false;

			if (!ReferenceEquals(value.GetType(), typeof(Vector2i)))
				return false;

			return Equals((Vector2i)value);
		}

		/// <summary>
		///   Returns a hash code for this vector.
		/// </summary>
		public override int GetHashCode()
		{
			return (X.GetHashCode() * 397) ^ Y.GetHashCode();
		}

		/// <summary>
		///   Returns a string representation of this vector.
		/// </summary>
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "X: {0}, Y: {1}", X, Y);
		}

		/// <summary>
		///   Tests for equality between two vectors.
		/// </summary>
		/// <param name="left">The first vector to compare.</param>
		/// <param name="right">The second vector to compare.</param>
		public static bool operator ==(Vector2i left, Vector2i right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///   Tests for inequality between two vectors.
		/// </summary>
		/// <param name="left">The first vector to compare.</param>
		/// <param name="right">The second vector to compare.</param>
		public static bool operator !=(Vector2i left, Vector2i right)
		{
			return !(left == right);
		}

		/// <summary>
		///   Performs a vector addition.
		/// </summary>
		/// <param name="left">The first vector to add.</param>
		/// <param name="right">The second vector to add.</param>
		public static Vector2i operator +(Vector2i left, Vector2i right)
		{
			return new Vector2i(left.X + right.X, left.Y + right.Y);
		}

		/// <summary>
		///   Negates the components of a vector.
		/// </summary>
		/// <param name="vector">The vector whose components should be negated.</param>
		public static Vector2i operator -(Vector2i vector)
		{
			return new Vector2i(-vector.X, -vector.Y);
		}

		/// <summary>
		///   Performs a vector subtraction.
		/// </summary>
		/// <param name="left">The first vector.</param>
		/// <param name="right">The second vector.</param>
		public static Vector2i operator -(Vector2i left, Vector2i right)
		{
			return new Vector2i(left.X - right.X, left.Y - right.Y);
		}

		/// <summary>
		///   Scales the vector by the given factor.
		/// </summary>
		/// <param name="vector">The vector that should be scaled.</param>
		/// <param name="factor">The factor that should be applied.</param>
		public static Vector2i operator *(Vector2i vector, int factor)
		{
			return new Vector2i(vector.X * factor, vector.Y * factor);
		}

		/// <summary>
		///   Scales the vector by the given factor.
		/// </summary>
		/// <param name="factor">The factor that should be applied.</param>
		/// <param name="vector">The vector that should be scaled.</param>
		public static Vector2i operator *(int factor, Vector2i vector)
		{
			return new Vector2i(factor * vector.X, factor * vector.Y);
		}

		/// <summary>
		///   Divides the vector by a scalar value.
		/// </summary>
		/// <param name="vector">The vector that should be divided.</param>
		/// <param name="factor">The scalar value the vector should be divided by.</param>
		public static Vector2i operator /(Vector2i vector, int factor)
		{
			return new Vector2i(vector.X / factor, vector.Y / factor);
		}

		/// <summary>
		///   Computes the dot product of the two vectors.
		/// </summary>
		/// <param name="left">The first vector.</param>
		/// <param name="right">The second vector.</param>
		public static int Dot(Vector2i left, Vector2i right)
		{
			return left.X * right.X + left.Y * right.Y;
		}
	}

	/// <summary>
	///   Represents a two-component vector of 32-bit signed fixed-point (in 24.8 format) values.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector2f8 : IEquatable<Vector2f8>
	{
		/// <summary>
		///   A vector with all components set to zero.
		/// </summary>
		public static readonly Vector2f8 Zero = new Vector2f8(0, 0);

		/// <summary>
		///   The X-component of the vector.
		/// </summary>
		public Fixed8 X;

		/// <summary>
		///   The Y-component of the vector.
		/// </summary>
		public Fixed8 Y;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="x">The X-component of the vector.</param>
		/// <param name="y">The Y-component of the vector.</param>
		public Vector2f8(Fixed8 x, Fixed8 y)
		{
			X = x;
			Y = y;
		}

		/// <summary>
		///   Gets the length of the vector.
		/// </summary>
		public Fixed8 Length
		{
			get { return Fixed8.Sqrt(SquaredLength); }
		}

		/// <summary>
		///   Gets the squared length of the vector.
		/// </summary>
		public Fixed8 SquaredLength
		{
			get { return X * X + Y * Y; }
		}

		/// <summary>
		///   Constructs a new vector instance that is normalized to a length of 1, but still points into the same direction.
		/// </summary>
		public Vector2f8 Normalize()
		{
			var length = Length;
			if (length < Fixed8.Epsilon)
				length = Fixed8.Epsilon;

			return new Vector2f8(X / length, Y / length);
		}

		/// <summary>
		///   Determines whether the given vector is equal to this vector.
		/// </summary>
		/// <param name="other">The other vector to compare with this vector.</param>
		public bool Equals(Vector2f8 other)
		{
			return X == other.X && Y == other.Y;
		}

		/// <summary>
		///   Determines whether the specified object is equal to this vector.
		/// </summary>
		/// <param name="value">The object to compare with this vector.</param>
		public override bool Equals(object value)
		{
			if (value == null)
				return false;

			if (!ReferenceEquals(value.GetType(), typeof(Vector2f8)))
				return false;

			return Equals((Vector2f8)value);
		}

		/// <summary>
		///   Returns a hash code for this vector.
		/// </summary>
		public override int GetHashCode()
		{
			return (X.GetHashCode() * 397) ^ Y.GetHashCode();
		}

		/// <summary>
		///   Returns a string representation of this vector.
		/// </summary>
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "X: {0}, Y: {1}", X, Y);
		}

		/// <summary>
		///   Tests for equality between two vectors.
		/// </summary>
		/// <param name="left">The first vector to compare.</param>
		/// <param name="right">The second vector to compare.</param>
		public static bool operator ==(Vector2f8 left, Vector2f8 right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///   Tests for inequality between two vectors.
		/// </summary>
		/// <param name="left">The first vector to compare.</param>
		/// <param name="right">The second vector to compare.</param>
		public static bool operator !=(Vector2f8 left, Vector2f8 right)
		{
			return !(left == right);
		}

		/// <summary>
		///   Performs a vector addition.
		/// </summary>
		/// <param name="left">The first vector to add.</param>
		/// <param name="right">The second vector to add.</param>
		public static Vector2f8 operator +(Vector2f8 left, Vector2f8 right)
		{
			return new Vector2f8(left.X + right.X, left.Y + right.Y);
		}

		/// <summary>
		///   Negates the components of a vector.
		/// </summary>
		/// <param name="vector">The vector whose components should be negated.</param>
		public static Vector2f8 operator -(Vector2f8 vector)
		{
			return new Vector2f8(-vector.X, -vector.Y);
		}

		/// <summary>
		///   Performs a vector subtraction.
		/// </summary>
		/// <param name="left">The first vector.</param>
		/// <param name="right">The second vector.</param>
		public static Vector2f8 operator -(Vector2f8 left, Vector2f8 right)
		{
			return new Vector2f8(left.X - right.X, left.Y - right.Y);
		}

		/// <summary>
		///   Scales the vector by the given factor.
		/// </summary>
		/// <param name="vector">The vector that should be scaled.</param>
		/// <param name="factor">The factor that should be applied.</param>
		public static Vector2f8 operator *(Vector2f8 vector, Fixed8 factor)
		{
			return new Vector2f8(vector.X * factor, vector.Y * factor);
		}

		/// <summary>
		///   Scales the vector by the given factor.
		/// </summary>
		/// <param name="factor">The factor that should be applied.</param>
		/// <param name="vector">The vector that should be scaled.</param>
		public static Vector2f8 operator *(Fixed8 factor, Vector2f8 vector)
		{
			return new Vector2f8(factor * vector.X, factor * vector.Y);
		}

		/// <summary>
		///   Divides the vector by a scalar value.
		/// </summary>
		/// <param name="vector">The vector that should be divided.</param>
		/// <param name="factor">The scalar value the vector should be divided by.</param>
		public static Vector2f8 operator /(Vector2f8 vector, Fixed8 factor)
		{
			return new Vector2f8(vector.X / factor, vector.Y / factor);
		}

		/// <summary>
		///   Computes the dot product of the two vectors.
		/// </summary>
		/// <param name="left">The first vector.</param>
		/// <param name="right">The second vector.</param>
		public static Fixed8 Dot(Vector2f8 left, Vector2f8 right)
		{
			return left.X * right.X + left.Y * right.Y;
		}
	}

	/// <summary>
	///   Represents a two-component vector of 32-bit signed fixed-point (in 16.16 format) values.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector2f16 : IEquatable<Vector2f16>
	{
		/// <summary>
		///   A vector with all components set to zero.
		/// </summary>
		public static readonly Vector2f16 Zero = new Vector2f16(0, 0);

		/// <summary>
		///   The X-component of the vector.
		/// </summary>
		public Fixed16 X;

		/// <summary>
		///   The Y-component of the vector.
		/// </summary>
		public Fixed16 Y;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="x">The X-component of the vector.</param>
		/// <param name="y">The Y-component of the vector.</param>
		public Vector2f16(Fixed16 x, Fixed16 y)
		{
			X = x;
			Y = y;
		}

		/// <summary>
		///   Gets the length of the vector.
		/// </summary>
		public Fixed16 Length
		{
			get { return Fixed16.Sqrt(SquaredLength); }
		}

		/// <summary>
		///   Gets the squared length of the vector.
		/// </summary>
		public Fixed16 SquaredLength
		{
			get { return X * X + Y * Y; }
		}

		/// <summary>
		///   Constructs a new vector instance that is normalized to a length of 1, but still points into the same direction.
		/// </summary>
		public Vector2f16 Normalize()
		{
			var length = Length;
			if (length < Fixed16.Epsilon)
				length = Fixed16.Epsilon;

			return new Vector2f16(X / length, Y / length);
		}

		/// <summary>
		///   Determines whether the given vector is equal to this vector.
		/// </summary>
		/// <param name="other">The other vector to compare with this vector.</param>
		public bool Equals(Vector2f16 other)
		{
			return X == other.X && Y == other.Y;
		}

		/// <summary>
		///   Determines whether the specified object is equal to this vector.
		/// </summary>
		/// <param name="value">The object to compare with this vector.</param>
		public override bool Equals(object value)
		{
			if (value == null)
				return false;

			if (!ReferenceEquals(value.GetType(), typeof(Vector2f16)))
				return false;

			return Equals((Vector2f16)value);
		}

		/// <summary>
		///   Returns a hash code for this vector.
		/// </summary>
		public override int GetHashCode()
		{
			return (X.GetHashCode() * 397) ^ Y.GetHashCode();
		}

		/// <summary>
		///   Returns a string representation of this vector.
		/// </summary>
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "X: {0}, Y: {1}", X, Y);
		}

		/// <summary>
		///   Tests for equality between two vectors.
		/// </summary>
		/// <param name="left">The first vector to compare.</param>
		/// <param name="right">The second vector to compare.</param>
		public static bool operator ==(Vector2f16 left, Vector2f16 right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///   Tests for inequality between two vectors.
		/// </summary>
		/// <param name="left">The first vector to compare.</param>
		/// <param name="right">The second vector to compare.</param>
		public static bool operator !=(Vector2f16 left, Vector2f16 right)
		{
			return !(left == right);
		}

		/// <summary>
		///   Performs a vector addition.
		/// </summary>
		/// <param name="left">The first vector to add.</param>
		/// <param name="right">The second vector to add.</param>
		public static Vector2f16 operator +(Vector2f16 left, Vector2f16 right)
		{
			return new Vector2f16(left.X + right.X, left.Y + right.Y);
		}

		/// <summary>
		///   Negates the components of a vector.
		/// </summary>
		/// <param name="vector">The vector whose components should be negated.</param>
		public static Vector2f16 operator -(Vector2f16 vector)
		{
			return new Vector2f16(-vector.X, -vector.Y);
		}

		/// <summary>
		///   Performs a vector subtraction.
		/// </summary>
		/// <param name="left">The first vector.</param>
		/// <param name="right">The second vector.</param>
		public static Vector2f16 operator -(Vector2f16 left, Vector2f16 right)
		{
			return new Vector2f16(left.X - right.X, left.Y - right.Y);
		}

		/// <summary>
		///   Scales the vector by the given factor.
		/// </summary>
		/// <param name="vector">The vector that should be scaled.</param>
		/// <param name="factor">The factor that should be applied.</param>
		public static Vector2f16 operator *(Vector2f16 vector, Fixed16 factor)
		{
			return new Vector2f16(vector.X * factor, vector.Y * factor);
		}

		/// <summary>
		///   Scales the vector by the given factor.
		/// </summary>
		/// <param name="factor">The factor that should be applied.</param>
		/// <param name="vector">The vector that should be scaled.</param>
		public static Vector2f16 operator *(Fixed16 factor, Vector2f16 vector)
		{
			return new Vector2f16(factor * vector.X, factor * vector.Y);
		}

		/// <summary>
		///   Divides the vector by a scalar value.
		/// </summary>
		/// <param name="vector">The vector that should be divided.</param>
		/// <param name="factor">The scalar value the vector should be divided by.</param>
		public static Vector2f16 operator /(Vector2f16 vector, Fixed16 factor)
		{
			return new Vector2f16(vector.X / factor, vector.Y / factor);
		}

		/// <summary>
		///   Computes the dot product of the two vectors.
		/// </summary>
		/// <param name="left">The first vector.</param>
		/// <param name="right">The second vector.</param>
		public static Fixed16 Dot(Vector2f16 left, Vector2f16 right)
		{
			return left.X * right.X + left.Y * right.Y;
		}
	}
}

// ReSharper restore InconsistentNaming