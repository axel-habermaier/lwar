namespace Pegasus.Math
{
	using System;

	/// <summary>
	///     Represents a segment of a line in 2D space.
	/// </summary>
	public struct LineSegment
	{
		/// <summary>
		///     The end point of the line segment.
		/// </summary>
		public Vector2 End;

		/// <summary>
		///     The start point of the line segment.
		/// </summary>
		public Vector2 Start;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="start">The start point of the line segment.</param>
		/// <param name="end">The end point of the line segment.</param>
		public LineSegment(Vector2 start, Vector2 end)
		{
			Start = start;
			End = end;
		}

		/// <summary>
		///     Computes the intersection between the two line segments. Returns false if the lines do not intersect.
		/// </summary>
		/// <param name="other">The other line segment.</param>
		/// <param name="intersection">Returns the intersection point if an intersection was found.</param>
		public bool Intersects(LineSegment other, out Vector2 intersection)
		{
			intersection = Vector2.Zero;

			var directionThis = End - Start;
			var directionOther = other.End - other.Start;

			var determinant = directionOther.Y * directionThis.X - directionOther.X * directionThis.Y;
			if (MathUtils.Equals(determinant, 0.0f))
				return false;

			var distanceThis = directionOther.X * (Start.Y - other.Start.Y) - directionOther.Y * (Start.X - other.Start.X);
			var distanceOther = directionThis.X * (Start.Y - other.Start.Y) - directionThis.Y * (Start.X - other.Start.X);

			distanceThis /= determinant;
			distanceOther /= determinant;

			if (distanceThis < 0 || distanceThis > 1 || distanceOther < 0 || distanceOther > 1)
				return false;

			intersection = Start + distanceThis * directionThis;
			return true;
		}
	}
}