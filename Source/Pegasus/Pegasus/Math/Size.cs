using System;

namespace Pegasus.Math
{
	using System.Globalization;
	using System.Runtime.InteropServices;

	/// <summary>
	///     Represents a size value with the width and height stored as 32-bit signed integer values.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Size : IEquatable<Size>
	{
		/// <summary>
		///     The width.
		/// </summary>
		public int Width;

		/// <summary>
		///     The height.
		/// </summary>
		public int Height;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		public Size(int width, int height)
			: this()
		{
			Width = width;
			Height = height;
		}

		/// <summary>
		///     Determines whether the specified object is equal to this size instance.
		/// </summary>
		/// <param name="obj">The object to compare with this instance.</param>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (obj.GetType() != typeof(Size))
				return false;
			return Equals((Size)obj);
		}

		/// <summary>
		///     Determines whether the specified size instance is equal to this size instance.
		/// </summary>
		/// <param name="other">The size instance to compare with this instance.</param>
		public bool Equals(Size other)
		{
			return Width == other.Width && Height == other.Height;
		}

		/// <summary>
		///     Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
		/// </returns>
		public override int GetHashCode()
		{
			var result = Width.GetHashCode();
			result = (result * 397) ^ Height.GetHashCode();
			return result;
		}

		/// <summary>
		///     Tests for equality between two sizes.
		/// </summary>
		/// <param name="left">The first size to compare.</param>
		/// <param name="right">The second size to compare.</param>
		public static bool operator ==(Size left, Size right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///     Tests for inequality between two sizes.
		/// </summary>
		/// <param name="left">The first size to compare.</param>
		/// <param name="right">The second size to compare.</param>
		public static bool operator !=(Size left, Size right)
		{
			return !(left == right);
		}

		/// <summary>
		///     Scales the size by the given factor.
		/// </summary>
		/// <param name="size">The size that should be scaled.</param>
		/// <param name="factor">The factor that should be applied.</param>
		public static Size operator *(Size size, int factor)
		{
			return new Size(size.Width * factor, size.Height * factor);
		}

		/// <summary>
		///     Scales the size by the given factor.
		/// </summary>
		/// <param name="factor">The factor that should be applied.</param>
		/// <param name="size">The size that should be scaled.</param>
		public static Size operator *(int factor, Size size)
		{
			return new Size(factor * size.Width, factor * size.Height);
		}

		/// <summary>
		///     Divides the size by the given factor.
		/// </summary>
		/// <param name="size">The size that should be divided.</param>
		/// <param name="factor">The scalar value the vector should be divided by.</param>
		public static Size operator /(Size size, int factor)
		{
			return new Size(size.Width / factor, size.Height / factor);
		}

		/// <summary>
		///     Implicitly converts a size to a vector.
		/// </summary>
		/// <param name="size">The size that should be converted.</param>
		public static implicit operator Vector2i(Size size)
		{
			return new Vector2i(size.Width, size.Height);
		}

		/// <summary>
		///     Returns a string representation of this size instance.
		/// </summary>
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "Width: {0}, Height: {1}", Width, Height);
		}
	}

	/// <summary>
	///     Represents a size value with the width and height stored as 32-bit floating point values.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct SizeF : IEquatable<SizeF>
	{
		/// <summary>
		///     The width.
		/// </summary>
		public float Width;

		/// <summary>
		///     The height.
		/// </summary>
		public float Height;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		public SizeF(float width, float height)
			: this()
		{
			Width = width;
			Height = height;
		}

		/// <summary>
		///     Determines whether the specified object is equal to this size instance.
		/// </summary>
		/// <param name="obj">The object to compare with this instance.</param>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (obj.GetType() != typeof(SizeF))
				return false;
			return Equals((SizeF)obj);
		}

		/// <summary>
		///     Determines whether the specified size instance is equal to this size instance.
		/// </summary>
		/// <param name="other">The size instance to compare with this instance.</param>
		public bool Equals(SizeF other)
		{
			return MathUtils.Equals(Width, other.Width) && MathUtils.Equals(Height, other.Height);
		}

		/// <summary>
		///     Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
		/// </returns>
		public override int GetHashCode()
		{
			var result = Width.GetHashCode();
			result = (result * 397) ^ Height.GetHashCode();
			return result;
		}

		/// <summary>
		///     Tests for equality between two sizes.
		/// </summary>
		/// <param name="left">The first size to compare.</param>
		/// <param name="right">The second size to compare.</param>
		public static bool operator ==(SizeF left, SizeF right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///     Tests for inequality between two sizes.
		/// </summary>
		/// <param name="left">The first size to compare.</param>
		/// <param name="right">The second size to compare.</param>
		public static bool operator !=(SizeF left, SizeF right)
		{
			return !(left == right);
		}

		/// <summary>
		///     Scales the size by the given factor.
		/// </summary>
		/// <param name="size">The size that should be scaled.</param>
		/// <param name="factor">The factor that should be applied.</param>
		public static SizeF operator *(SizeF size, float factor)
		{
			return new SizeF(size.Width * factor, size.Height * factor);
		}

		/// <summary>
		///     Scales the size by the given factor.
		/// </summary>
		/// <param name="factor">The factor that should be applied.</param>
		/// <param name="size">The size that should be scaled.</param>
		public static SizeF operator *(float factor, SizeF size)
		{
			return new SizeF(factor * size.Width, factor * size.Height);
		}

		/// <summary>
		///     Divides the size by the given factor.
		/// </summary>
		/// <param name="size">The size that should be divided.</param>
		/// <param name="factor">The scalar value the vector should be divided by.</param>
		public static SizeF operator /(SizeF size, float factor)
		{
			return new SizeF(size.Width / factor, size.Height / factor);
		}

		/// <summary>
		///     Implicitly converts a size to a vector.
		/// </summary>
		/// <param name="size">The size that should be converted.</param>
		public static implicit operator Vector2(SizeF size)
		{
			return new Vector2(size.Width, size.Height);
		}

		/// <summary>
		///     Returns a string representation of this size instance.
		/// </summary>
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "Width: {0}, Height: {1}", Width, Height);
		}
	}

	/// <summary>
	///     Represents a size value with the width and height stored as 32-bit signed fixed-point (in 24.8 format) values.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct SizeF8 : IEquatable<SizeF8>
	{
		/// <summary>
		///     The width.
		/// </summary>
		public Fixed8 Width;

		/// <summary>
		///     The height.
		/// </summary>
		public Fixed8 Height;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		public SizeF8(Fixed8 width, Fixed8 height)
			: this()
		{
			Width = width;
			Height = height;
		}

		/// <summary>
		///     Determines whether the specified object is equal to this size instance.
		/// </summary>
		/// <param name="obj">The object to compare with this instance.</param>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (obj.GetType() != typeof(SizeF8))
				return false;
			return Equals((SizeF8)obj);
		}

		/// <summary>
		///     Determines whether the specified size instance is equal to this size instance.
		/// </summary>
		/// <param name="other">The size instance to compare with this instance.</param>
		public bool Equals(SizeF8 other)
		{
			return Width == other.Width && Height == other.Height;
		}

		/// <summary>
		///     Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
		/// </returns>
		public override int GetHashCode()
		{
			var result = Width.GetHashCode();
			result = (result * 397) ^ Height.GetHashCode();
			return result;
		}

		/// <summary>
		///     Tests for equality between two sizes.
		/// </summary>
		/// <param name="left">The first size to compare.</param>
		/// <param name="right">The second size to compare.</param>
		public static bool operator ==(SizeF8 left, SizeF8 right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///     Tests for inequality between two sizes.
		/// </summary>
		/// <param name="left">The first size to compare.</param>
		/// <param name="right">The second size to compare.</param>
		public static bool operator !=(SizeF8 left, SizeF8 right)
		{
			return !(left == right);
		}

		/// <summary>
		///     Scales the size by the given factor.
		/// </summary>
		/// <param name="size">The size that should be scaled.</param>
		/// <param name="factor">The factor that should be applied.</param>
		public static SizeF8 operator *(SizeF8 size, Fixed8 factor)
		{
			return new SizeF8(size.Width * factor, size.Height * factor);
		}

		/// <summary>
		///     Scales the size by the given factor.
		/// </summary>
		/// <param name="factor">The factor that should be applied.</param>
		/// <param name="size">The size that should be scaled.</param>
		public static SizeF8 operator *(Fixed8 factor, SizeF8 size)
		{
			return new SizeF8(factor * size.Width, factor * size.Height);
		}

		/// <summary>
		///     Divides the size by the given factor.
		/// </summary>
		/// <param name="size">The size that should be divided.</param>
		/// <param name="factor">The scalar value the vector should be divided by.</param>
		public static SizeF8 operator /(SizeF8 size, Fixed8 factor)
		{
			return new SizeF8(size.Width / factor, size.Height / factor);
		}

		/// <summary>
		///     Implicitly converts a size to a vector.
		/// </summary>
		/// <param name="size">The size that should be converted.</param>
		public static implicit operator Vector2f8(SizeF8 size)
		{
			return new Vector2f8(size.Width, size.Height);
		}

		/// <summary>
		///     Returns a string representation of this size instance.
		/// </summary>
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "Width: {0}, Height: {1}", Width, Height);
		}
	}

	/// <summary>
	///     Represents a size value with the width and height stored as 32-bit signed fixed-point (in 16.16 format) values.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct SizeF16 : IEquatable<SizeF16>
	{
		/// <summary>
		///     The width.
		/// </summary>
		public Fixed16 Width;

		/// <summary>
		///     The height.
		/// </summary>
		public Fixed16 Height;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		public SizeF16(Fixed16 width, Fixed16 height)
			: this()
		{
			Width = width;
			Height = height;
		}

		/// <summary>
		///     Determines whether the specified object is equal to this size instance.
		/// </summary>
		/// <param name="obj">The object to compare with this instance.</param>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (obj.GetType() != typeof(SizeF16))
				return false;
			return Equals((SizeF16)obj);
		}

		/// <summary>
		///     Determines whether the specified size instance is equal to this size instance.
		/// </summary>
		/// <param name="other">The size instance to compare with this instance.</param>
		public bool Equals(SizeF16 other)
		{
			return Width == other.Width && Height == other.Height;
		}

		/// <summary>
		///     Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
		/// </returns>
		public override int GetHashCode()
		{
			var result = Width.GetHashCode();
			result = (result * 397) ^ Height.GetHashCode();
			return result;
		}

		/// <summary>
		///     Tests for equality between two sizes.
		/// </summary>
		/// <param name="left">The first size to compare.</param>
		/// <param name="right">The second size to compare.</param>
		public static bool operator ==(SizeF16 left, SizeF16 right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///     Tests for inequality between two sizes.
		/// </summary>
		/// <param name="left">The first size to compare.</param>
		/// <param name="right">The second size to compare.</param>
		public static bool operator !=(SizeF16 left, SizeF16 right)
		{
			return !(left == right);
		}

		/// <summary>
		///     Scales the size by the given factor.
		/// </summary>
		/// <param name="size">The size that should be scaled.</param>
		/// <param name="factor">The factor that should be applied.</param>
		public static SizeF16 operator *(SizeF16 size, Fixed16 factor)
		{
			return new SizeF16(size.Width * factor, size.Height * factor);
		}

		/// <summary>
		///     Scales the size by the given factor.
		/// </summary>
		/// <param name="factor">The factor that should be applied.</param>
		/// <param name="size">The size that should be scaled.</param>
		public static SizeF16 operator *(Fixed16 factor, SizeF16 size)
		{
			return new SizeF16(factor * size.Width, factor * size.Height);
		}

		/// <summary>
		///     Divides the size by the given factor.
		/// </summary>
		/// <param name="size">The size that should be divided.</param>
		/// <param name="factor">The scalar value the vector should be divided by.</param>
		public static SizeF16 operator /(SizeF16 size, Fixed16 factor)
		{
			return new SizeF16(size.Width / factor, size.Height / factor);
		}

		/// <summary>
		///     Implicitly converts a size to a vector.
		/// </summary>
		/// <param name="size">The size that should be converted.</param>
		public static implicit operator Vector2f16(SizeF16 size)
		{
			return new Vector2f16(size.Width, size.Height);
		}

		/// <summary>
		///     Returns a string representation of this size instance.
		/// </summary>
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "Width: {0}, Height: {1}", Width, Height);
		}
	}

	/// <summary>
	///     Represents a size value with the width and height stored as 64-bit floating point values.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct SizeD : IEquatable<SizeD>
	{
		/// <summary>
		///     The width.
		/// </summary>
		public double Width;

		/// <summary>
		///     The height.
		/// </summary>
		public double Height;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		public SizeD(double width, double height)
			: this()
		{
			Width = width;
			Height = height;
		}

		/// <summary>
		///     Determines whether the specified object is equal to this size instance.
		/// </summary>
		/// <param name="obj">The object to compare with this instance.</param>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (obj.GetType() != typeof(SizeD))
				return false;
			return Equals((SizeD)obj);
		}

		/// <summary>
		///     Determines whether the specified size instance is equal to this size instance.
		/// </summary>
		/// <param name="other">The size instance to compare with this instance.</param>
		public bool Equals(SizeD other)
		{
			return MathUtils.Equals(Width, other.Width) && MathUtils.Equals(Height, other.Height);
		}

		/// <summary>
		///     Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
		/// </returns>
		public override int GetHashCode()
		{
			var result = Width.GetHashCode();
			result = (result * 397) ^ Height.GetHashCode();
			return result;
		}

		/// <summary>
		///     Tests for equality between two sizes.
		/// </summary>
		/// <param name="left">The first size to compare.</param>
		/// <param name="right">The second size to compare.</param>
		public static bool operator ==(SizeD left, SizeD right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///     Tests for inequality between two sizes.
		/// </summary>
		/// <param name="left">The first size to compare.</param>
		/// <param name="right">The second size to compare.</param>
		public static bool operator !=(SizeD left, SizeD right)
		{
			return !(left == right);
		}

		/// <summary>
		///     Scales the size by the given factor.
		/// </summary>
		/// <param name="size">The size that should be scaled.</param>
		/// <param name="factor">The factor that should be applied.</param>
		public static SizeD operator *(SizeD size, double factor)
		{
			return new SizeD(size.Width * factor, size.Height * factor);
		}

		/// <summary>
		///     Scales the size by the given factor.
		/// </summary>
		/// <param name="factor">The factor that should be applied.</param>
		/// <param name="size">The size that should be scaled.</param>
		public static SizeD operator *(double factor, SizeD size)
		{
			return new SizeD(factor * size.Width, factor * size.Height);
		}

		/// <summary>
		///     Divides the size by the given factor.
		/// </summary>
		/// <param name="size">The size that should be divided.</param>
		/// <param name="factor">The scalar value the vector should be divided by.</param>
		public static SizeD operator /(SizeD size, double factor)
		{
			return new SizeD(size.Width / factor, size.Height / factor);
		}

		/// <summary>
		///     Implicitly converts a size to a vector.
		/// </summary>
		/// <param name="size">The size that should be converted.</param>
		public static implicit operator Vector2d(SizeD size)
		{
			return new Vector2d(size.Width, size.Height);
		}

		/// <summary>
		///     Returns a string representation of this size instance.
		/// </summary>
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "Width: {0}, Height: {1}", Width, Height);
		}
	}
}

