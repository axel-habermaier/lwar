﻿
using System;

namespace Pegasus.Framework.Math
{
	using System.Globalization;
	using System.Runtime.InteropServices;

	/// <summary>
	///   Represents a rectangle with the left, top, width, and height stored as 32-bit signed integer values.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Rectangle : IEquatable<Rectangle>
	{
		/// <summary>
		///   Represents a rectangle at the origin with an area of 0.
		/// </summary>
		public static readonly Rectangle Empty;

		/// <summary>
		///   The X-coordinate of the left edge of the rectangle.
		/// </summary>
		public int Left;

		/// <summary>
		///   The X-coordinate of the top edge of the rectangle.
		/// </summary>
		public int Top;

		/// <summary>
		///   The width of the rectangle.
		/// </summary>
		public int Width;

		/// <summary>
		///   The width of the rectangle.
		/// </summary>
		public int Height;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="left">The X-coordinate of the left edge of the rectangle.</param>
		/// <param name="top">The Y-coordinate of the left edge of the rectangle.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		public Rectangle(int left, int top, int width, int height)
		{
			Left = left;
			Top = top;
			Width = width;
			Height = height;
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="position">The position of the rectangle's top left corner.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		public Rectangle(Vector2i position, int width, int height)
		{
			Left = position.X;
			Top = position.Y;
			Width = width;
			Height = height;
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="position">The position of the rectangle's top left corner.</param>
		/// <param name="size">The size of the rectangle.</param>
		public Rectangle(Vector2i position, Size size)
		{
			Left = position.X;
			Top = position.Y;
			Width = size.Width;
			Height = size.Height;
		}

		/// <summary>
		///   Gets the X-coordinate of the right edge of the rectangle.
		/// </summary>
		public int Right
		{
			get { return Left + Width; }
		}

		/// <summary>
		///   Gets the Y-coordinate of the bottom edge of the rectangle.
		/// </summary>
		public int Bottom
		{
			get { return Top + Height; }
		}

		/// <summary>
		///   Gets the position of the rectangle's top left corner.
		/// </summary>
		public Vector2i Position
		{
			get { return new Vector2i(Left, Top); }
		}

		/// <summary>
		///   Gets the size of the rectangle.
		/// </summary>
		public Size Size
		{
			get { return new Size(Width, Height); }
		}

		/// <summary>
		///   Gets the position of the rectangle's top left corner.
		/// </summary>
		public Vector2i TopLeft
		{
			get { return Position; }
		}

		/// <summary>
		///   Gets the position of the rectangle's top right corner.
		/// </summary>
		public Vector2i TopRight
		{
			get { return Position + new Vector2i(Width, 0); }
		}

		/// <summary>
		///   Gets the position of the rectangle's bottom left corner.
		/// </summary>
		public Vector2i BottomLeft
		{
			get { return Position + new Vector2i(0, Height); }
		}

		/// <summary>
		///   Gets the position of the rectangle's bottom right corner.
		/// </summary>
		public Vector2i BottomRight
		{
			get { return Position + new Vector2i(Width, Height); }
		}

		/// <summary>
		///   Returns a copy of the rectangle with the given offsets added to the position of the returned rectangle.
		/// </summary>
		/// <param name="x">The offset that should be applied to the rectangle's left edge.</param>
		/// <param name="y">The offset that should be applied to the rectangle's top edge.</param>
		public Rectangle Offset(int x, int y)
		{
			return new Rectangle(Left + x, Top + y, Width, Height);
		}

		/// <summary>
		///   Returns a copy of the rectangle with the given offsets added to the position of the returned rectangle.
		/// </summary>
		/// <param name="offset">The offset that should be applied to the rectangle's position.</param>
		public Rectangle Offset(Vector2i offset)
		{
			return Offset(offset.X, offset.Y);
		}

		/// <summary>
		///   Determines whether the given rectangle is equal to this rectangle.
		/// </summary>
		/// <param name="other">The other rectangle to compare with this rectangle.</param>
		public bool Equals(Rectangle other)
		{
			return Left == other.Left && Top == other.Top && 
				Width == other.Width && Height == other.Height;
		}

		/// <summary>
		///   Determines whether the specified object is equal to this rectangle.
		/// </summary>
		/// <param name="value">The object to compare with this rectangle.</param>
		public override bool Equals(object value)
		{
			if (ReferenceEquals(null, value))
				return false;

			if (value.GetType() != typeof(Rectangle))
				return false;

			return Equals((Rectangle)value);
		}

		/// <summary>
		///   Returns a hash code for this rectangle.
		/// </summary>
		public override int GetHashCode()
		{
			var result = Left.GetHashCode();
			result = (result * 397) ^ Top.GetHashCode();
			result = (result * 397) ^ Width.GetHashCode();
			result = (result * 397) ^ Height.GetHashCode();
			return result;
		}

		/// <summary>
		///   Tests for equality between two rectangles.
		/// </summary>
		/// <param name="left">The first rectangle to compare.</param>
		/// <param name="right">The second rectangle to compare.</param>
		public static bool operator ==(Rectangle left, Rectangle right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///   Tests for inequality between two rectangles.
		/// </summary>
		/// <param name="left">The first rectangle to compare.</param>
		/// <param name="right">The second rectangle to compare.</param>
		public static bool operator !=(Rectangle left, Rectangle right)
		{
			return !(left == right);
		}

		/// <summary>
		///   Returns a string representation of this rectangle.
		/// </summary>
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "Left: {0}, Top: {1}, Width: {2}, Height: {3}", Left, Top, Width, Height);
		}

		/// <summary>
		///   Checks whether this rectangle intersects with the given rectangle.
		/// </summary>
		/// <param name="rectangle">The rectangle that should be checked.</param>
		public bool Intersects(Rectangle rectangle)
		{
			var xOverlap = (Left >= rectangle.Left && Left <= rectangle.Right) ||
				(rectangle.Left >= Left && rectangle.Left <= Right);

			var yOverlap = (Top >= rectangle.Top && Top <= rectangle.Bottom) ||
				(rectangle.Top >= Top && rectangle.Top <= Bottom);

			return xOverlap && yOverlap;
		}

		/// <summary>
		///   Checks whether this rectangle intersects with the given circle.
		/// </summary>
		/// <param name="circle">The circle that should be checked.</param>
		public bool Intersects(Circle circle)
		{
			return circle.Intersects(this);
		}

		/// <summary>
		///   Checks whether the given point lies within the rectangle.
		/// </summary>
		/// <param name="point">The point that should be checked.</param>
		public bool Intersects(Vector2i point)
		{
			return point.X >= Left && point.X <= Right && point.Y >= Top && point.Y <= Bottom;
		}
	}

	/// <summary>
	///   Represents a rectangle with the left, top, width, and height stored as 32-bit floating point values.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct RectangleF : IEquatable<RectangleF>
	{
		/// <summary>
		///   Represents a rectangle at the origin with an area of 0.
		/// </summary>
		public static readonly RectangleF Empty;

		/// <summary>
		///   The X-coordinate of the left edge of the rectangle.
		/// </summary>
		public float Left;

		/// <summary>
		///   The X-coordinate of the top edge of the rectangle.
		/// </summary>
		public float Top;

		/// <summary>
		///   The width of the rectangle.
		/// </summary>
		public float Width;

		/// <summary>
		///   The width of the rectangle.
		/// </summary>
		public float Height;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="left">The X-coordinate of the left edge of the rectangle.</param>
		/// <param name="top">The Y-coordinate of the left edge of the rectangle.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		public RectangleF(float left, float top, float width, float height)
		{
			Left = left;
			Top = top;
			Width = width;
			Height = height;
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="position">The position of the rectangle's top left corner.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		public RectangleF(Vector2 position, float width, float height)
		{
			Left = position.X;
			Top = position.Y;
			Width = width;
			Height = height;
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="position">The position of the rectangle's top left corner.</param>
		/// <param name="size">The size of the rectangle.</param>
		public RectangleF(Vector2 position, SizeF size)
		{
			Left = position.X;
			Top = position.Y;
			Width = size.Width;
			Height = size.Height;
		}

		/// <summary>
		///   Gets the X-coordinate of the right edge of the rectangle.
		/// </summary>
		public float Right
		{
			get { return Left + Width; }
		}

		/// <summary>
		///   Gets the Y-coordinate of the bottom edge of the rectangle.
		/// </summary>
		public float Bottom
		{
			get { return Top + Height; }
		}

		/// <summary>
		///   Gets the position of the rectangle's top left corner.
		/// </summary>
		public Vector2 Position
		{
			get { return new Vector2(Left, Top); }
		}

		/// <summary>
		///   Gets the size of the rectangle.
		/// </summary>
		public SizeF Size
		{
			get { return new SizeF(Width, Height); }
		}

		/// <summary>
		///   Gets the position of the rectangle's top left corner.
		/// </summary>
		public Vector2 TopLeft
		{
			get { return Position; }
		}

		/// <summary>
		///   Gets the position of the rectangle's top right corner.
		/// </summary>
		public Vector2 TopRight
		{
			get { return Position + new Vector2(Width, 0); }
		}

		/// <summary>
		///   Gets the position of the rectangle's bottom left corner.
		/// </summary>
		public Vector2 BottomLeft
		{
			get { return Position + new Vector2(0, Height); }
		}

		/// <summary>
		///   Gets the position of the rectangle's bottom right corner.
		/// </summary>
		public Vector2 BottomRight
		{
			get { return Position + new Vector2(Width, Height); }
		}

		/// <summary>
		///   Returns a copy of the rectangle with the given offsets added to the position of the returned rectangle.
		/// </summary>
		/// <param name="x">The offset that should be applied to the rectangle's left edge.</param>
		/// <param name="y">The offset that should be applied to the rectangle's top edge.</param>
		public RectangleF Offset(float x, float y)
		{
			return new RectangleF(Left + x, Top + y, Width, Height);
		}

		/// <summary>
		///   Returns a copy of the rectangle with the given offsets added to the position of the returned rectangle.
		/// </summary>
		/// <param name="offset">The offset that should be applied to the rectangle's position.</param>
		public RectangleF Offset(Vector2 offset)
		{
			return Offset(offset.X, offset.Y);
		}

		/// <summary>
		///   Determines whether the given rectangle is equal to this rectangle.
		/// </summary>
		/// <param name="other">The other rectangle to compare with this rectangle.</param>
		public bool Equals(RectangleF other)
		{
			return MathUtils.FloatEquality(Left, other.Left) && MathUtils.FloatEquality(Top, other.Top) && 
				MathUtils.FloatEquality(Width, other.Width) && MathUtils.FloatEquality(Height, other.Height);
		}

		/// <summary>
		///   Determines whether the specified object is equal to this rectangle.
		/// </summary>
		/// <param name="value">The object to compare with this rectangle.</param>
		public override bool Equals(object value)
		{
			if (ReferenceEquals(null, value))
				return false;

			if (value.GetType() != typeof(RectangleF))
				return false;

			return Equals((RectangleF)value);
		}

		/// <summary>
		///   Returns a hash code for this rectangle.
		/// </summary>
		public override int GetHashCode()
		{
			var result = Left.GetHashCode();
			result = (result * 397) ^ Top.GetHashCode();
			result = (result * 397) ^ Width.GetHashCode();
			result = (result * 397) ^ Height.GetHashCode();
			return result;
		}

		/// <summary>
		///   Tests for equality between two rectangles.
		/// </summary>
		/// <param name="left">The first rectangle to compare.</param>
		/// <param name="right">The second rectangle to compare.</param>
		public static bool operator ==(RectangleF left, RectangleF right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///   Tests for inequality between two rectangles.
		/// </summary>
		/// <param name="left">The first rectangle to compare.</param>
		/// <param name="right">The second rectangle to compare.</param>
		public static bool operator !=(RectangleF left, RectangleF right)
		{
			return !(left == right);
		}

		/// <summary>
		///   Returns a string representation of this rectangle.
		/// </summary>
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "Left: {0}, Top: {1}, Width: {2}, Height: {3}", Left, Top, Width, Height);
		}

		/// <summary>
		///   Checks whether this rectangle intersects with the given rectangle.
		/// </summary>
		/// <param name="rectangle">The rectangle that should be checked.</param>
		public bool Intersects(RectangleF rectangle)
		{
			var xOverlap = (Left >= rectangle.Left && Left <= rectangle.Right) ||
				(rectangle.Left >= Left && rectangle.Left <= Right);

			var yOverlap = (Top >= rectangle.Top && Top <= rectangle.Bottom) ||
				(rectangle.Top >= Top && rectangle.Top <= Bottom);

			return xOverlap && yOverlap;
		}

		/// <summary>
		///   Checks whether this rectangle intersects with the given circle.
		/// </summary>
		/// <param name="circle">The circle that should be checked.</param>
		public bool Intersects(CircleF circle)
		{
			return circle.Intersects(this);
		}

		/// <summary>
		///   Checks whether the given point lies within the rectangle.
		/// </summary>
		/// <param name="point">The point that should be checked.</param>
		public bool Intersects(Vector2 point)
		{
			return point.X >= Left && point.X <= Right && point.Y >= Top && point.Y <= Bottom;
		}
	}

	/// <summary>
	///   Represents a rectangle with the left, top, width, and height stored as 32-bit signed fixed-point (in 24.8 format) values.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct RectangleF8 : IEquatable<RectangleF8>
	{
		/// <summary>
		///   Represents a rectangle at the origin with an area of 0.
		/// </summary>
		public static readonly RectangleF8 Empty;

		/// <summary>
		///   The X-coordinate of the left edge of the rectangle.
		/// </summary>
		public Fixed8 Left;

		/// <summary>
		///   The X-coordinate of the top edge of the rectangle.
		/// </summary>
		public Fixed8 Top;

		/// <summary>
		///   The width of the rectangle.
		/// </summary>
		public Fixed8 Width;

		/// <summary>
		///   The width of the rectangle.
		/// </summary>
		public Fixed8 Height;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="left">The X-coordinate of the left edge of the rectangle.</param>
		/// <param name="top">The Y-coordinate of the left edge of the rectangle.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		public RectangleF8(Fixed8 left, Fixed8 top, Fixed8 width, Fixed8 height)
		{
			Left = left;
			Top = top;
			Width = width;
			Height = height;
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="position">The position of the rectangle's top left corner.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		public RectangleF8(Vector2f8 position, Fixed8 width, Fixed8 height)
		{
			Left = position.X;
			Top = position.Y;
			Width = width;
			Height = height;
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="position">The position of the rectangle's top left corner.</param>
		/// <param name="size">The size of the rectangle.</param>
		public RectangleF8(Vector2f8 position, SizeF8 size)
		{
			Left = position.X;
			Top = position.Y;
			Width = size.Width;
			Height = size.Height;
		}

		/// <summary>
		///   Gets the X-coordinate of the right edge of the rectangle.
		/// </summary>
		public Fixed8 Right
		{
			get { return Left + Width; }
		}

		/// <summary>
		///   Gets the Y-coordinate of the bottom edge of the rectangle.
		/// </summary>
		public Fixed8 Bottom
		{
			get { return Top + Height; }
		}

		/// <summary>
		///   Gets the position of the rectangle's top left corner.
		/// </summary>
		public Vector2f8 Position
		{
			get { return new Vector2f8(Left, Top); }
		}

		/// <summary>
		///   Gets the size of the rectangle.
		/// </summary>
		public SizeF8 Size
		{
			get { return new SizeF8(Width, Height); }
		}

		/// <summary>
		///   Gets the position of the rectangle's top left corner.
		/// </summary>
		public Vector2f8 TopLeft
		{
			get { return Position; }
		}

		/// <summary>
		///   Gets the position of the rectangle's top right corner.
		/// </summary>
		public Vector2f8 TopRight
		{
			get { return Position + new Vector2f8(Width, 0); }
		}

		/// <summary>
		///   Gets the position of the rectangle's bottom left corner.
		/// </summary>
		public Vector2f8 BottomLeft
		{
			get { return Position + new Vector2f8(0, Height); }
		}

		/// <summary>
		///   Gets the position of the rectangle's bottom right corner.
		/// </summary>
		public Vector2f8 BottomRight
		{
			get { return Position + new Vector2f8(Width, Height); }
		}

		/// <summary>
		///   Returns a copy of the rectangle with the given offsets added to the position of the returned rectangle.
		/// </summary>
		/// <param name="x">The offset that should be applied to the rectangle's left edge.</param>
		/// <param name="y">The offset that should be applied to the rectangle's top edge.</param>
		public RectangleF8 Offset(Fixed8 x, Fixed8 y)
		{
			return new RectangleF8(Left + x, Top + y, Width, Height);
		}

		/// <summary>
		///   Returns a copy of the rectangle with the given offsets added to the position of the returned rectangle.
		/// </summary>
		/// <param name="offset">The offset that should be applied to the rectangle's position.</param>
		public RectangleF8 Offset(Vector2f8 offset)
		{
			return Offset(offset.X, offset.Y);
		}

		/// <summary>
		///   Determines whether the given rectangle is equal to this rectangle.
		/// </summary>
		/// <param name="other">The other rectangle to compare with this rectangle.</param>
		public bool Equals(RectangleF8 other)
		{
			return Left == other.Left && Top == other.Top && 
				Width == other.Width && Height == other.Height;
		}

		/// <summary>
		///   Determines whether the specified object is equal to this rectangle.
		/// </summary>
		/// <param name="value">The object to compare with this rectangle.</param>
		public override bool Equals(object value)
		{
			if (ReferenceEquals(null, value))
				return false;

			if (value.GetType() != typeof(RectangleF8))
				return false;

			return Equals((RectangleF8)value);
		}

		/// <summary>
		///   Returns a hash code for this rectangle.
		/// </summary>
		public override int GetHashCode()
		{
			var result = Left.GetHashCode();
			result = (result * 397) ^ Top.GetHashCode();
			result = (result * 397) ^ Width.GetHashCode();
			result = (result * 397) ^ Height.GetHashCode();
			return result;
		}

		/// <summary>
		///   Tests for equality between two rectangles.
		/// </summary>
		/// <param name="left">The first rectangle to compare.</param>
		/// <param name="right">The second rectangle to compare.</param>
		public static bool operator ==(RectangleF8 left, RectangleF8 right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///   Tests for inequality between two rectangles.
		/// </summary>
		/// <param name="left">The first rectangle to compare.</param>
		/// <param name="right">The second rectangle to compare.</param>
		public static bool operator !=(RectangleF8 left, RectangleF8 right)
		{
			return !(left == right);
		}

		/// <summary>
		///   Returns a string representation of this rectangle.
		/// </summary>
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "Left: {0}, Top: {1}, Width: {2}, Height: {3}", Left, Top, Width, Height);
		}

		/// <summary>
		///   Checks whether this rectangle intersects with the given rectangle.
		/// </summary>
		/// <param name="rectangle">The rectangle that should be checked.</param>
		public bool Intersects(RectangleF8 rectangle)
		{
			var xOverlap = (Left >= rectangle.Left && Left <= rectangle.Right) ||
				(rectangle.Left >= Left && rectangle.Left <= Right);

			var yOverlap = (Top >= rectangle.Top && Top <= rectangle.Bottom) ||
				(rectangle.Top >= Top && rectangle.Top <= Bottom);

			return xOverlap && yOverlap;
		}

		/// <summary>
		///   Checks whether this rectangle intersects with the given circle.
		/// </summary>
		/// <param name="circle">The circle that should be checked.</param>
		public bool Intersects(CircleF8 circle)
		{
			return circle.Intersects(this);
		}

		/// <summary>
		///   Checks whether the given point lies within the rectangle.
		/// </summary>
		/// <param name="point">The point that should be checked.</param>
		public bool Intersects(Vector2f8 point)
		{
			return point.X >= Left && point.X <= Right && point.Y >= Top && point.Y <= Bottom;
		}
	}

	/// <summary>
	///   Represents a rectangle with the left, top, width, and height stored as 32-bit signed fixed-point (in 16.16 format) values.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct RectangleF16 : IEquatable<RectangleF16>
	{
		/// <summary>
		///   Represents a rectangle at the origin with an area of 0.
		/// </summary>
		public static readonly RectangleF16 Empty;

		/// <summary>
		///   The X-coordinate of the left edge of the rectangle.
		/// </summary>
		public Fixed16 Left;

		/// <summary>
		///   The X-coordinate of the top edge of the rectangle.
		/// </summary>
		public Fixed16 Top;

		/// <summary>
		///   The width of the rectangle.
		/// </summary>
		public Fixed16 Width;

		/// <summary>
		///   The width of the rectangle.
		/// </summary>
		public Fixed16 Height;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="left">The X-coordinate of the left edge of the rectangle.</param>
		/// <param name="top">The Y-coordinate of the left edge of the rectangle.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		public RectangleF16(Fixed16 left, Fixed16 top, Fixed16 width, Fixed16 height)
		{
			Left = left;
			Top = top;
			Width = width;
			Height = height;
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="position">The position of the rectangle's top left corner.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		public RectangleF16(Vector2f16 position, Fixed16 width, Fixed16 height)
		{
			Left = position.X;
			Top = position.Y;
			Width = width;
			Height = height;
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="position">The position of the rectangle's top left corner.</param>
		/// <param name="size">The size of the rectangle.</param>
		public RectangleF16(Vector2f16 position, SizeF16 size)
		{
			Left = position.X;
			Top = position.Y;
			Width = size.Width;
			Height = size.Height;
		}

		/// <summary>
		///   Gets the X-coordinate of the right edge of the rectangle.
		/// </summary>
		public Fixed16 Right
		{
			get { return Left + Width; }
		}

		/// <summary>
		///   Gets the Y-coordinate of the bottom edge of the rectangle.
		/// </summary>
		public Fixed16 Bottom
		{
			get { return Top + Height; }
		}

		/// <summary>
		///   Gets the position of the rectangle's top left corner.
		/// </summary>
		public Vector2f16 Position
		{
			get { return new Vector2f16(Left, Top); }
		}

		/// <summary>
		///   Gets the size of the rectangle.
		/// </summary>
		public SizeF16 Size
		{
			get { return new SizeF16(Width, Height); }
		}

		/// <summary>
		///   Gets the position of the rectangle's top left corner.
		/// </summary>
		public Vector2f16 TopLeft
		{
			get { return Position; }
		}

		/// <summary>
		///   Gets the position of the rectangle's top right corner.
		/// </summary>
		public Vector2f16 TopRight
		{
			get { return Position + new Vector2f16(Width, 0); }
		}

		/// <summary>
		///   Gets the position of the rectangle's bottom left corner.
		/// </summary>
		public Vector2f16 BottomLeft
		{
			get { return Position + new Vector2f16(0, Height); }
		}

		/// <summary>
		///   Gets the position of the rectangle's bottom right corner.
		/// </summary>
		public Vector2f16 BottomRight
		{
			get { return Position + new Vector2f16(Width, Height); }
		}

		/// <summary>
		///   Returns a copy of the rectangle with the given offsets added to the position of the returned rectangle.
		/// </summary>
		/// <param name="x">The offset that should be applied to the rectangle's left edge.</param>
		/// <param name="y">The offset that should be applied to the rectangle's top edge.</param>
		public RectangleF16 Offset(Fixed16 x, Fixed16 y)
		{
			return new RectangleF16(Left + x, Top + y, Width, Height);
		}

		/// <summary>
		///   Returns a copy of the rectangle with the given offsets added to the position of the returned rectangle.
		/// </summary>
		/// <param name="offset">The offset that should be applied to the rectangle's position.</param>
		public RectangleF16 Offset(Vector2f16 offset)
		{
			return Offset(offset.X, offset.Y);
		}

		/// <summary>
		///   Determines whether the given rectangle is equal to this rectangle.
		/// </summary>
		/// <param name="other">The other rectangle to compare with this rectangle.</param>
		public bool Equals(RectangleF16 other)
		{
			return Left == other.Left && Top == other.Top && 
				Width == other.Width && Height == other.Height;
		}

		/// <summary>
		///   Determines whether the specified object is equal to this rectangle.
		/// </summary>
		/// <param name="value">The object to compare with this rectangle.</param>
		public override bool Equals(object value)
		{
			if (ReferenceEquals(null, value))
				return false;

			if (value.GetType() != typeof(RectangleF16))
				return false;

			return Equals((RectangleF16)value);
		}

		/// <summary>
		///   Returns a hash code for this rectangle.
		/// </summary>
		public override int GetHashCode()
		{
			var result = Left.GetHashCode();
			result = (result * 397) ^ Top.GetHashCode();
			result = (result * 397) ^ Width.GetHashCode();
			result = (result * 397) ^ Height.GetHashCode();
			return result;
		}

		/// <summary>
		///   Tests for equality between two rectangles.
		/// </summary>
		/// <param name="left">The first rectangle to compare.</param>
		/// <param name="right">The second rectangle to compare.</param>
		public static bool operator ==(RectangleF16 left, RectangleF16 right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///   Tests for inequality between two rectangles.
		/// </summary>
		/// <param name="left">The first rectangle to compare.</param>
		/// <param name="right">The second rectangle to compare.</param>
		public static bool operator !=(RectangleF16 left, RectangleF16 right)
		{
			return !(left == right);
		}

		/// <summary>
		///   Returns a string representation of this rectangle.
		/// </summary>
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "Left: {0}, Top: {1}, Width: {2}, Height: {3}", Left, Top, Width, Height);
		}

		/// <summary>
		///   Checks whether this rectangle intersects with the given rectangle.
		/// </summary>
		/// <param name="rectangle">The rectangle that should be checked.</param>
		public bool Intersects(RectangleF16 rectangle)
		{
			var xOverlap = (Left >= rectangle.Left && Left <= rectangle.Right) ||
				(rectangle.Left >= Left && rectangle.Left <= Right);

			var yOverlap = (Top >= rectangle.Top && Top <= rectangle.Bottom) ||
				(rectangle.Top >= Top && rectangle.Top <= Bottom);

			return xOverlap && yOverlap;
		}

		/// <summary>
		///   Checks whether this rectangle intersects with the given circle.
		/// </summary>
		/// <param name="circle">The circle that should be checked.</param>
		public bool Intersects(CircleF16 circle)
		{
			return circle.Intersects(this);
		}

		/// <summary>
		///   Checks whether the given point lies within the rectangle.
		/// </summary>
		/// <param name="point">The point that should be checked.</param>
		public bool Intersects(Vector2f16 point)
		{
			return point.X >= Left && point.X <= Right && point.Y >= Top && point.Y <= Bottom;
		}
	}
}

