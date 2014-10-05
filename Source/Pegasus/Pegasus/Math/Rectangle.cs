namespace Pegasus.Math
{
	using System;
	using System.Globalization;
	using System.Runtime.InteropServices;

	/// <summary>
	///     Represents a rectangle.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Rectangle : IEquatable<Rectangle>
	{
		/// <summary>
		///     Represents a rectangle at the origin with an area of 0.
		/// </summary>
		public static readonly Rectangle Empty;

		/// <summary>
		///     The X-coordinate of the left edge of the rectangle.
		/// </summary>
		public float Left;

		/// <summary>
		///     The Y-coordinate of the top edge of the rectangle.
		/// </summary>
		public float Top;

		/// <summary>
		///     The width of the rectangle.
		/// </summary>
		public float Width;

		/// <summary>
		///     The height of the rectangle.
		/// </summary>
		public float Height;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="left">The X-coordinate of the left edge of the rectangle.</param>
		/// <param name="top">The Y-coordinate of the left edge of the rectangle.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		public Rectangle(float left, float top, float width, float height)
		{
			Left = left;
			Top = top;
			Width = width;
			Height = height;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="position">The position of the rectangle's top left corner.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		public Rectangle(Vector2 position, float width, float height)
		{
			Left = position.X;
			Top = position.Y;
			Width = width;
			Height = height;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="left">The X-coordinate of the left edge of the rectangle.</param>
		/// <param name="top">The Y-coordinate of the left edge of the rectangle.</param>
		/// <param name="size">The size of the rectangle.</param>
		public Rectangle(float left, float top, Size size)
		{
			Left = left;
			Top = top;
			Width = size.Width;
			Height = size.Height;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="position">The position of the rectangle's top left corner.</param>
		/// <param name="size">The size of the rectangle.</param>
		public Rectangle(Vector2 position, Size size)
		{
			Left = position.X;
			Top = position.Y;
			Width = size.Width;
			Height = size.Height;
		}

		/// <summary>
		///     Gets the X-coordinate of the right edge of the rectangle.
		/// </summary>
		public float Right
		{
			get { return Left + Width; }
		}

		/// <summary>
		///     Gets the Y-coordinate of the bottom edge of the rectangle.
		/// </summary>
		public float Bottom
		{
			get { return Top + Height; }
		}

		/// <summary>
		///     Gets the position of the rectangle's top left corner.
		/// </summary>
		public Vector2 Position
		{
			get { return new Vector2(Left, Top); }
		}

		/// <summary>
		///     Gets the size of the rectangle.
		/// </summary>
		public Size Size
		{
			get { return new Size(Width, Height); }
		}

		/// <summary>
		///     Gets the position of the rectangle's top left corner.
		/// </summary>
		public Vector2 TopLeft
		{
			get { return Position; }
		}

		/// <summary>
		///     Gets the position of the rectangle's top right corner.
		/// </summary>
		public Vector2 TopRight
		{
			get { return Position + new Vector2(Width, 0); }
		}

		/// <summary>
		///     Gets the position of the rectangle's bottom left corner.
		/// </summary>
		public Vector2 BottomLeft
		{
			get { return Position + new Vector2(0, Height); }
		}

		/// <summary>
		///     Gets the position of the rectangle's bottom right corner.
		/// </summary>
		public Vector2 BottomRight
		{
			get { return Position + new Vector2(Width, Height); }
		}

		/// <summary>
		///     Returns a copy of the rectangle with the given offsets added to the position of the returned rectangle.
		/// </summary>
		/// <param name="x">The offset that should be applied to the rectangle's left edge.</param>
		/// <param name="y">The offset that should be applied to the rectangle's top edge.</param>
		public Rectangle Offset(float x, float y)
		{
			return new Rectangle(Left + x, Top + y, Width, Height);
		}

		/// <summary>
		///     Returns a copy of the rectangle with the given offsets added to the position of the returned rectangle.
		/// </summary>
		/// <param name="offset">The offset that should be applied to the rectangle's position.</param>
		public Rectangle Offset(Vector2 offset)
		{
			return Offset(offset.X, offset.Y);
		}

		/// <summary>
		///     Returns an enlarged copy of the rectangle. The returned rectangle has a width and a height that is enlarged 
		///     by the given amount in both directions, and the rectangle is moved by the given amount in the (left, up) direction.
		/// </summary>
		/// <param name="amount">The amount that the rectangle should be enlarged in both X and Y directions.</param>
		public Rectangle Enlarge(float amount)
		{
			return Enlarge(amount, amount);
		}

		/// <summary>
		///     Returns an enlarged copy of the rectangle. The returned rectangle has a width and a height that is enlarged 
		///     by the given amount in both directions, and the rectangle is moved by the given amounts in the (left, up) direction.
		/// </summary>
		/// <param name="amount">The amount that the rectangle should be enlarged in both X and Y directions.</param>
		public Rectangle Enlarge(Vector2 amount)
		{
			return Enlarge(amount.X, amount.Y);
		}

		/// <summary>
		///     Returns an enlarged copy of the rectangle. The returned rectangle has a width and a height that is enlarged 
		///     by the given amount in both directions, and the rectangle is moved by the given amounts in the (left, up) direction.
		/// </summary>
		/// <param name="x">The amount that the rectangle should be enlarged in X-direction.</param>
		/// <param name="y">The amount that the rectangle should be enlarged in Y-direction.</param>
		public Rectangle Enlarge(float x, float y)
		{
			return new Rectangle(Left - x, Top - y, Width + 2 * x, Height + 2 * y);
		}

		/// <summary>
		///     Determines whether the given rectangle is equal to this rectangle.
		/// </summary>
		/// <param name="other">The other rectangle to compare with this rectangle.</param>
		public bool Equals(Rectangle other)
		{
			return MathUtils.Equals(Left, other.Left) && MathUtils.Equals(Top, other.Top) &&
				MathUtils.Equals(Width, other.Width) && MathUtils.Equals(Height, other.Height);
		}

		/// <summary>
		///     Determines whether the specified object is equal to this rectangle.
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
		///     Returns a hash code for this rectangle.
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
		///     Tests for equality between two rectangles.
		/// </summary>
		/// <param name="left">The first rectangle to compare.</param>
		/// <param name="right">The second rectangle to compare.</param>
		public static bool operator ==(Rectangle left, Rectangle right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///     Tests for inequality between two rectangles.
		/// </summary>
		/// <param name="left">The first rectangle to compare.</param>
		/// <param name="right">The second rectangle to compare.</param>
		public static bool operator !=(Rectangle left, Rectangle right)
		{
			return !(left == right);
		}

		/// <summary>
		///     Returns a string representation of this rectangle.
		/// </summary>
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "Left: {0}, Top: {1}, Width: {2}, Height: {3}", Left, Top, Width, Height);
		}

		/// <summary>
		///     Checks whether this rectangle intersects with the given rectangle.
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
		///     Checks whether this rectangle intersects with the given circle.
		/// </summary>
		/// <param name="circle">The circle that should be checked.</param>
		public bool Intersects(Circle circle)
		{
			return circle.Intersects(this);
		}

		/// <summary>
		///     Checks whether the given point lies within the rectangle.
		/// </summary>
		/// <param name="point">The point that should be checked.</param>
		public bool Intersects(Vector2 point)
		{
			return point.X >= Left && point.X <= Right && point.Y >= Top && point.Y <= Bottom;
		}
	}
}