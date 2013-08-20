using System;

namespace Pegasus.Framework.UserInterface
{
	/// <summary>
	///   Describes the thickness of a rectangular frame.
	/// </summary>
	public struct Thickness : IEquatable<Thickness>
	{
		/// <summary>
		///   The width of the lower side of the rectangle.
		/// </summary>
		public double Bottom;

		/// <summary>
		///   The width of the left side of the rectangle.
		/// </summary>
		public double Left;

		/// <summary>
		///   The width of the right side of the rectangle.
		/// </summary>
		public double Right;

		/// <summary>
		///   The width of the upper side of the rectangle.
		/// </summary>
		public double Top;

		/// <summary>
		///   Initializes a new instance that has a uniform width on each side.
		/// </summary>
		/// <param name="width">The width of the left, right, upper, and lower sides of the rectangle.</param>
		public Thickness(double width)
		{
			Left = width;
			Right = width;
			Top = width;
			Bottom = width;
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="left">The width of the left side of the rectangle.</param>
		/// <param name="right">The width of the right side of the rectangle.</param>
		/// <param name="top">The width of the upper side of the rectangle.</param>
		/// <param name="bottom">The width of the lower side of the rectangle.</param>
		public Thickness(double left, double right, double top, double bottom)
		{
			Left = left;
			Right = right;
			Top = top;
			Bottom = bottom;
		}

		/// <summary>
		///   Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(Thickness other)
		{
			return Bottom.Equals(other.Bottom) && Left.Equals(other.Left) && Right.Equals(other.Right) && Top.Equals(other.Top);
		}

		/// <summary>
		///   Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to.</param>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			return obj is Thickness && Equals((Thickness)obj);
		}

		/// <summary>
		///   Returns the hash code for this instance.
		/// </summary>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = Bottom.GetHashCode();
				hashCode = (hashCode * 397) ^ Left.GetHashCode();
				hashCode = (hashCode * 397) ^ Right.GetHashCode();
				hashCode = (hashCode * 397) ^ Top.GetHashCode();
				return hashCode;
			}
		}

		/// <summary>
		///   Indicates whether the two thickness instances are equal.
		/// </summary>
		public static bool operator ==(Thickness left, Thickness right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///   Indicates whether the two thickness instances are not equal.
		/// </summary>
		public static bool operator !=(Thickness left, Thickness right)
		{
			return !left.Equals(right);
		}
	}
}