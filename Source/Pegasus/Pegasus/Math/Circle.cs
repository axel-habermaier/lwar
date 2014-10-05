namespace Pegasus.Math
{
	using System;
	using System.Globalization;
	using System.Runtime.InteropServices;

	/// <summary>
	///     Represents a circle with a position and a radius.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Circle : IEquatable<Circle>
	{
		/// <summary>
		///     The position of the circle's center.
		/// </summary>
		public readonly Vector2 Position;

		/// <summary>
		///     The circle's radius.
		/// </summary>
		public readonly float Radius;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="x">The X-component of the circle's position.</param>
		/// <param name="y">The Y-component of the circle's position.</param>
		/// <param name="radius">The circle's radius.</param>
		public Circle(float x, float y, float radius)
			: this(new Vector2(x, y), radius)
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="position">The position of the circle's center.</param>
		/// <param name="radius">The circle's radius.</param>
		public Circle(Vector2 position, float radius)
		{
			Position = position;
			Radius = radius;
		}

		/// <summary>
		///     Returns a copy of the circle with the given offsets added to the position of the returned circle.
		/// </summary>
		/// <param name="x">The offset that should be applied to the circle's position in x-direction.</param>
		/// <param name="y">The offset that should be applied to the circle's position in y-direction.</param>
		public Circle Offset(float x, float y)
		{
			return Offset(new Vector2(Position.X + x, Position.Y + y));
		}

		/// <summary>
		///     Returns a copy of the circle with the given offsets added to the position of the returned circle.
		/// </summary>
		/// <param name="offset">The offset that should be applied to the circle's position.</param>
		public Circle Offset(Vector2 offset)
		{
			return new Circle(Position + offset, Radius);
		}

		/// <summary>
		///     Determines whether the given circle is equal to this circle.
		/// </summary>
		/// <param name="other">The other circle to compare with this circle.</param>
		public bool Equals(Circle other)
		{
			return MathUtils.Equals(Position.X, other.Position.X) && MathUtils.Equals(Position.Y, other.Position.Y)
				   && MathUtils.Equals(Radius, other.Radius);
		}

		/// <summary>
		///     Determines whether the specified object is equal to this circle.
		/// </summary>
		/// <param name="value">The object to compare with this circle.</param>
		public override bool Equals(object value)
		{
			if (ReferenceEquals(null, value))
				return false;

			if (value.GetType() != typeof(Circle))
				return false;

			return Equals((Circle)value);
		}

		/// <summary>
		///     Returns a hash code for this circle.
		/// </summary>
		public override int GetHashCode()
		{
			return (Position.GetHashCode() * 397) ^ Radius.GetHashCode();
		}

		/// <summary>
		///     Tests for equality between two circle.
		/// </summary>
		/// <param name="left">The first circle to compare.</param>
		/// <param name="right">The second circle to compare.</param>
		public static bool operator ==(Circle left, Circle right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///     Tests for inequality between two circle.
		/// </summary>
		/// <param name="left">The first circle to compare.</param>
		/// <param name="right">The second circle to compare.</param>
		public static bool operator !=(Circle left, Circle right)
		{
			return !(left == right);
		}

		/// <summary>
		///     Returns a string representation of this circle.
		/// </summary>
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "Position: {0}, Radius: {1}", Position, Radius);
		}

		/// <summary>
		///     Checks whether this circle intersects with the given circle.
		/// </summary>
		/// <param name="circle">The circle that should be checked.</param>
		public bool Intersects(Circle circle)
		{
			var distance = (Position - circle.Position).SquaredLength;
			var radiusSum = Radius + circle.Radius;
			return distance <= radiusSum * radiusSum;
		}

		/// <summary>
		///     Checks whether this circle intersects with the given rectangle.
		/// </summary>
		/// <param name="rectangle">The rectangle that should be checked.</param>
		public bool Intersects(Rectangle rectangle)
		{
			// Find the closest point to the circle that lies within the rectangle
			var closestX = MathUtils.Clamp(Position.X, rectangle.Left, rectangle.Right);
			var closestY = MathUtils.Clamp(Position.Y, rectangle.Top, rectangle.Bottom);
			var closest = new Vector2(closestX, closestY);

			// Calculate the distance between the circle's center and the closest point
			var distance = Position - closest;

			// There is an intersection only if the distance is less than or equal to the circle's radius
			return distance.SquaredLength <= Radius * Radius;
		}

		/// <summary>
		///     Checks whether the given point lies within the circle.
		/// </summary>
		/// <param name="point">The point that should be checked.</param>
		public bool Intersects(Vector2 point)
		{
			var distance = (Position - point).SquaredLength;
			return distance <= Radius * Radius;
		}
	}
}