
using System;

namespace Pegasus.Framework.Math
{
	using System.Globalization;
	using System.Runtime.InteropServices;

	/// <summary>
	///   Represents a circle with the position and radius stored as 32-bit signed integer values.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Circle : IEquatable<Circle>
	{
		/// <summary>
		///   The position of the circle's center.
		/// </summary>
		public Vector2i Position;

		/// <summary>
		///   The circle's radius.
		/// </summary>
		public int Radius;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="x">The X-component of the circle's position.</param>
		/// <param name="y">The Y-component of the circle's position.</param>
		/// <param name="radius">The circle's radius.</param>
		public Circle(int x, int y, int radius)
			: this(new Vector2i(x, y), radius)
		{
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="position">The position of the circle's center.</param>
		/// <param name="radius">The circle's radius.</param>
		public Circle(Vector2i position, int radius)
		{
			Position = position;
			Radius = radius;
		}

		/// <summary>
		///   Returns a copy of the circle with the given offsets added to the position of the returned circle.
		/// </summary>
		/// <param name="x">The offset that should be applied to the circle's position in x-direction.</param>
		/// <param name="y">The offset that should be applied to the circle's position in y-direction.</param>
		public Circle Offset(int x, int y)
		{
			return Offset(new Vector2i(Position.X + x, Position.Y + y));
		}

		/// <summary>
		///   Returns a copy of the circle with the given offsets added to the position of the returned circle.
		/// </summary>
		/// <param name="offset">The offset that should be applied to the circle's position.</param>
		public Circle Offset(Vector2i offset)
		{
			return new Circle(Position + offset, Radius);
		}

		/// <summary>
		///   Determines whether the given circle is equal to this circle.
		/// </summary>
		/// <param name="other">The other circle to compare with this circle.</param>
		public bool Equals(Circle other)
		{
			return Position.X == other.Position.X && Position.Y == other.Position.Y 
				&& Radius == other.Radius;
		}

		/// <summary>
		///   Determines whether the specified object is equal to this circle.
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
		///   Returns a hash code for this circle.
		/// </summary>
		public override int GetHashCode()
		{
			return (Position.GetHashCode() * 397) ^ Radius.GetHashCode();
		}

		/// <summary>
		///   Tests for equality between two circle.
		/// </summary>
		/// <param name="left">The first circle to compare.</param>
		/// <param name="right">The second circle to compare.</param>
		public static bool operator ==(Circle left, Circle right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///   Tests for inequality between two circle.
		/// </summary>
		/// <param name="left">The first circle to compare.</param>
		/// <param name="right">The second circle to compare.</param>
		public static bool operator !=(Circle left, Circle right)
		{
			return !(left == right);
		}

		/// <summary>
		///   Returns a string representation of this circle.
		/// </summary>
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "Position: {0}, Radius: {1}", Position, Radius);
		}

		/// <summary>
		///   Checks whether this circle intersects with the given circle.
		/// </summary>
		/// <param name="circle">The circle that should be checked.</param>
		public bool Intersects(Circle circle)
		{
			var distance = (Position - circle.Position).SquaredLength;
			var radiusSum = Radius + circle.Radius;
			return distance <= radiusSum * radiusSum;
		}

		/// <summary>
		///   Checks whether this circle intersects with the given rectangle.
		/// </summary>
		/// <param name="rectangle">The rectangle that should be checked.</param>
		public bool Intersects(Rectangle rectangle)
		{
			// Find the closest point to the circle that lies within the rectangle
			var closestX = MathUtils.Clamp(Position.X, rectangle.Left, rectangle.Right);
			var closestY = MathUtils.Clamp(Position.Y, rectangle.Top, rectangle.Bottom);
			var closest = new Vector2i(closestX, closestY);

			// Calculate the distance between the circle's center and the closest point
			var distance = Position - closest;

			// There is an intersection only if the distance is less than or equal to the circle's radius
			return distance.SquaredLength <= Radius * Radius;
		}

		/// <summary>
		///   Checks whether the given point lies within the circle.
		/// </summary>
		/// <param name="point">The point that should be checked.</param>
		public bool Intersects(Vector2i point)
		{
			var distance = (Position - point).SquaredLength;
			return distance <= Radius * Radius;
		}
	}

	/// <summary>
	///   Represents a circle with the position and radius stored as 32-bit floating point values.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct CircleF : IEquatable<CircleF>
	{
		/// <summary>
		///   The position of the circle's center.
		/// </summary>
		public Vector2 Position;

		/// <summary>
		///   The circle's radius.
		/// </summary>
		public float Radius;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="x">The X-component of the circle's position.</param>
		/// <param name="y">The Y-component of the circle's position.</param>
		/// <param name="radius">The circle's radius.</param>
		public CircleF(float x, float y, float radius)
			: this(new Vector2(x, y), radius)
		{
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="position">The position of the circle's center.</param>
		/// <param name="radius">The circle's radius.</param>
		public CircleF(Vector2 position, float radius)
		{
			Position = position;
			Radius = radius;
		}

		/// <summary>
		///   Returns a copy of the circle with the given offsets added to the position of the returned circle.
		/// </summary>
		/// <param name="x">The offset that should be applied to the circle's position in x-direction.</param>
		/// <param name="y">The offset that should be applied to the circle's position in y-direction.</param>
		public CircleF Offset(float x, float y)
		{
			return Offset(new Vector2(Position.X + x, Position.Y + y));
		}

		/// <summary>
		///   Returns a copy of the circle with the given offsets added to the position of the returned circle.
		/// </summary>
		/// <param name="offset">The offset that should be applied to the circle's position.</param>
		public CircleF Offset(Vector2 offset)
		{
			return new CircleF(Position + offset, Radius);
		}

		/// <summary>
		///   Determines whether the given circle is equal to this circle.
		/// </summary>
		/// <param name="other">The other circle to compare with this circle.</param>
		public bool Equals(CircleF other)
		{
			return MathUtils.FloatEquality(Position.X, other.Position.X) && MathUtils.FloatEquality(Position.Y, other.Position.Y) 
				&& MathUtils.FloatEquality(Radius, other.Radius);
		}

		/// <summary>
		///   Determines whether the specified object is equal to this circle.
		/// </summary>
		/// <param name="value">The object to compare with this circle.</param>
		public override bool Equals(object value)
		{
			if (ReferenceEquals(null, value))
				return false;

			if (value.GetType() != typeof(CircleF))
				return false;

			return Equals((CircleF)value);
		}

		/// <summary>
		///   Returns a hash code for this circle.
		/// </summary>
		public override int GetHashCode()
		{
			return (Position.GetHashCode() * 397) ^ Radius.GetHashCode();
		}

		/// <summary>
		///   Tests for equality between two circle.
		/// </summary>
		/// <param name="left">The first circle to compare.</param>
		/// <param name="right">The second circle to compare.</param>
		public static bool operator ==(CircleF left, CircleF right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///   Tests for inequality between two circle.
		/// </summary>
		/// <param name="left">The first circle to compare.</param>
		/// <param name="right">The second circle to compare.</param>
		public static bool operator !=(CircleF left, CircleF right)
		{
			return !(left == right);
		}

		/// <summary>
		///   Returns a string representation of this circle.
		/// </summary>
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "Position: {0}, Radius: {1}", Position, Radius);
		}

		/// <summary>
		///   Checks whether this circle intersects with the given circle.
		/// </summary>
		/// <param name="circle">The circle that should be checked.</param>
		public bool Intersects(CircleF circle)
		{
			var distance = (Position - circle.Position).SquaredLength;
			var radiusSum = Radius + circle.Radius;
			return distance <= radiusSum * radiusSum;
		}

		/// <summary>
		///   Checks whether this circle intersects with the given rectangle.
		/// </summary>
		/// <param name="rectangle">The rectangle that should be checked.</param>
		public bool Intersects(RectangleF rectangle)
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
		///   Checks whether the given point lies within the circle.
		/// </summary>
		/// <param name="point">The point that should be checked.</param>
		public bool Intersects(Vector2 point)
		{
			var distance = (Position - point).SquaredLength;
			return distance <= Radius * Radius;
		}
	}

	/// <summary>
	///   Represents a circle with the position and radius stored as 32-bit signed fixed-point (in 24.8 format) values.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct CircleF8 : IEquatable<CircleF8>
	{
		/// <summary>
		///   The position of the circle's center.
		/// </summary>
		public Vector2f8 Position;

		/// <summary>
		///   The circle's radius.
		/// </summary>
		public Fixed8 Radius;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="x">The X-component of the circle's position.</param>
		/// <param name="y">The Y-component of the circle's position.</param>
		/// <param name="radius">The circle's radius.</param>
		public CircleF8(Fixed8 x, Fixed8 y, Fixed8 radius)
			: this(new Vector2f8(x, y), radius)
		{
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="position">The position of the circle's center.</param>
		/// <param name="radius">The circle's radius.</param>
		public CircleF8(Vector2f8 position, Fixed8 radius)
		{
			Position = position;
			Radius = radius;
		}

		/// <summary>
		///   Returns a copy of the circle with the given offsets added to the position of the returned circle.
		/// </summary>
		/// <param name="x">The offset that should be applied to the circle's position in x-direction.</param>
		/// <param name="y">The offset that should be applied to the circle's position in y-direction.</param>
		public CircleF8 Offset(Fixed8 x, Fixed8 y)
		{
			return Offset(new Vector2f8(Position.X + x, Position.Y + y));
		}

		/// <summary>
		///   Returns a copy of the circle with the given offsets added to the position of the returned circle.
		/// </summary>
		/// <param name="offset">The offset that should be applied to the circle's position.</param>
		public CircleF8 Offset(Vector2f8 offset)
		{
			return new CircleF8(Position + offset, Radius);
		}

		/// <summary>
		///   Determines whether the given circle is equal to this circle.
		/// </summary>
		/// <param name="other">The other circle to compare with this circle.</param>
		public bool Equals(CircleF8 other)
		{
			return Position.X == other.Position.X && Position.Y == other.Position.Y 
				&& Radius == other.Radius;
		}

		/// <summary>
		///   Determines whether the specified object is equal to this circle.
		/// </summary>
		/// <param name="value">The object to compare with this circle.</param>
		public override bool Equals(object value)
		{
			if (ReferenceEquals(null, value))
				return false;

			if (value.GetType() != typeof(CircleF8))
				return false;

			return Equals((CircleF8)value);
		}

		/// <summary>
		///   Returns a hash code for this circle.
		/// </summary>
		public override int GetHashCode()
		{
			return (Position.GetHashCode() * 397) ^ Radius.GetHashCode();
		}

		/// <summary>
		///   Tests for equality between two circle.
		/// </summary>
		/// <param name="left">The first circle to compare.</param>
		/// <param name="right">The second circle to compare.</param>
		public static bool operator ==(CircleF8 left, CircleF8 right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///   Tests for inequality between two circle.
		/// </summary>
		/// <param name="left">The first circle to compare.</param>
		/// <param name="right">The second circle to compare.</param>
		public static bool operator !=(CircleF8 left, CircleF8 right)
		{
			return !(left == right);
		}

		/// <summary>
		///   Returns a string representation of this circle.
		/// </summary>
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "Position: {0}, Radius: {1}", Position, Radius);
		}

		/// <summary>
		///   Checks whether this circle intersects with the given circle.
		/// </summary>
		/// <param name="circle">The circle that should be checked.</param>
		public bool Intersects(CircleF8 circle)
		{
			var distance = (Position - circle.Position).SquaredLength;
			var radiusSum = Radius + circle.Radius;
			return distance <= radiusSum * radiusSum;
		}

		/// <summary>
		///   Checks whether this circle intersects with the given rectangle.
		/// </summary>
		/// <param name="rectangle">The rectangle that should be checked.</param>
		public bool Intersects(RectangleF8 rectangle)
		{
			// Find the closest point to the circle that lies within the rectangle
			var closestX = MathUtils.Clamp(Position.X, rectangle.Left, rectangle.Right);
			var closestY = MathUtils.Clamp(Position.Y, rectangle.Top, rectangle.Bottom);
			var closest = new Vector2f8(closestX, closestY);

			// Calculate the distance between the circle's center and the closest point
			var distance = Position - closest;

			// There is an intersection only if the distance is less than or equal to the circle's radius
			return distance.SquaredLength <= Radius * Radius;
		}

		/// <summary>
		///   Checks whether the given point lies within the circle.
		/// </summary>
		/// <param name="point">The point that should be checked.</param>
		public bool Intersects(Vector2f8 point)
		{
			var distance = (Position - point).SquaredLength;
			return distance <= Radius * Radius;
		}
	}

	/// <summary>
	///   Represents a circle with the position and radius stored as 32-bit signed fixed-point (in 16.16 format) values.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct CircleF16 : IEquatable<CircleF16>
	{
		/// <summary>
		///   The position of the circle's center.
		/// </summary>
		public Vector2f16 Position;

		/// <summary>
		///   The circle's radius.
		/// </summary>
		public Fixed16 Radius;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="x">The X-component of the circle's position.</param>
		/// <param name="y">The Y-component of the circle's position.</param>
		/// <param name="radius">The circle's radius.</param>
		public CircleF16(Fixed16 x, Fixed16 y, Fixed16 radius)
			: this(new Vector2f16(x, y), radius)
		{
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="position">The position of the circle's center.</param>
		/// <param name="radius">The circle's radius.</param>
		public CircleF16(Vector2f16 position, Fixed16 radius)
		{
			Position = position;
			Radius = radius;
		}

		/// <summary>
		///   Returns a copy of the circle with the given offsets added to the position of the returned circle.
		/// </summary>
		/// <param name="x">The offset that should be applied to the circle's position in x-direction.</param>
		/// <param name="y">The offset that should be applied to the circle's position in y-direction.</param>
		public CircleF16 Offset(Fixed16 x, Fixed16 y)
		{
			return Offset(new Vector2f16(Position.X + x, Position.Y + y));
		}

		/// <summary>
		///   Returns a copy of the circle with the given offsets added to the position of the returned circle.
		/// </summary>
		/// <param name="offset">The offset that should be applied to the circle's position.</param>
		public CircleF16 Offset(Vector2f16 offset)
		{
			return new CircleF16(Position + offset, Radius);
		}

		/// <summary>
		///   Determines whether the given circle is equal to this circle.
		/// </summary>
		/// <param name="other">The other circle to compare with this circle.</param>
		public bool Equals(CircleF16 other)
		{
			return Position.X == other.Position.X && Position.Y == other.Position.Y 
				&& Radius == other.Radius;
		}

		/// <summary>
		///   Determines whether the specified object is equal to this circle.
		/// </summary>
		/// <param name="value">The object to compare with this circle.</param>
		public override bool Equals(object value)
		{
			if (ReferenceEquals(null, value))
				return false;

			if (value.GetType() != typeof(CircleF16))
				return false;

			return Equals((CircleF16)value);
		}

		/// <summary>
		///   Returns a hash code for this circle.
		/// </summary>
		public override int GetHashCode()
		{
			return (Position.GetHashCode() * 397) ^ Radius.GetHashCode();
		}

		/// <summary>
		///   Tests for equality between two circle.
		/// </summary>
		/// <param name="left">The first circle to compare.</param>
		/// <param name="right">The second circle to compare.</param>
		public static bool operator ==(CircleF16 left, CircleF16 right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///   Tests for inequality between two circle.
		/// </summary>
		/// <param name="left">The first circle to compare.</param>
		/// <param name="right">The second circle to compare.</param>
		public static bool operator !=(CircleF16 left, CircleF16 right)
		{
			return !(left == right);
		}

		/// <summary>
		///   Returns a string representation of this circle.
		/// </summary>
		public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, "Position: {0}, Radius: {1}", Position, Radius);
		}

		/// <summary>
		///   Checks whether this circle intersects with the given circle.
		/// </summary>
		/// <param name="circle">The circle that should be checked.</param>
		public bool Intersects(CircleF16 circle)
		{
			var distance = (Position - circle.Position).SquaredLength;
			var radiusSum = Radius + circle.Radius;
			return distance <= radiusSum * radiusSum;
		}

		/// <summary>
		///   Checks whether this circle intersects with the given rectangle.
		/// </summary>
		/// <param name="rectangle">The rectangle that should be checked.</param>
		public bool Intersects(RectangleF16 rectangle)
		{
			// Find the closest point to the circle that lies within the rectangle
			var closestX = MathUtils.Clamp(Position.X, rectangle.Left, rectangle.Right);
			var closestY = MathUtils.Clamp(Position.Y, rectangle.Top, rectangle.Bottom);
			var closest = new Vector2f16(closestX, closestY);

			// Calculate the distance between the circle's center and the closest point
			var distance = Position - closest;

			// There is an intersection only if the distance is less than or equal to the circle's radius
			return distance.SquaredLength <= Radius * Radius;
		}

		/// <summary>
		///   Checks whether the given point lies within the circle.
		/// </summary>
		/// <param name="point">The point that should be checked.</param>
		public bool Intersects(Vector2f16 point)
		{
			var distance = (Position - point).SquaredLength;
			return distance <= Radius * Radius;
		}
	}
}

