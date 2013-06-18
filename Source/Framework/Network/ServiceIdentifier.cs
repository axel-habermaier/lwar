using System;

namespace Pegasus.Framework.Network
{
	/// <summary>
	///   Represents a unique identifier of a service with a size of 16 bytes.
	/// </summary>
	public struct ServiceIdentifier : IEquatable<ServiceIdentifier>
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="firstPart">The first part of the service identifier.</param>
		/// <param name="secondPart">The second part of the service identifier.</param>
		public ServiceIdentifier(ulong firstPart, ulong secondPart)
			: this()
		{
			FirstPart = firstPart;
			SecondPart = secondPart;
		}

		/// <summary>
		///   Gets the first part of the service identifier.
		/// </summary>
		internal ulong FirstPart { get; private set; }

		/// <summary>
		///   Gets the second part of the service identifier.
		/// </summary>
		internal ulong SecondPart { get; private set; }

		/// <summary>
		///   Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		public bool Equals(ServiceIdentifier other)
		{
			return FirstPart == other.FirstPart && SecondPart == other.SecondPart;
		}

		/// <summary>
		///   Indicates whether this instance and a specified object are equal.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			return obj is ServiceIdentifier && Equals((ServiceIdentifier)obj);
		}

		/// <summary>
		///   Returns the hash code for this instance.
		/// </summary>
		public override int GetHashCode()
		{
			unchecked
			{
				return (FirstPart.GetHashCode() * 397) ^ SecondPart.GetHashCode();
			}
		}

		/// <summary>
		///   Determines whether the two service identifiers are equal.
		/// </summary>
		public static bool operator ==(ServiceIdentifier left, ServiceIdentifier right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///   Determines whether the two service identifiers are equal.
		/// </summary>
		public static bool operator !=(ServiceIdentifier left, ServiceIdentifier right)
		{
			return !left.Equals(right);
		}

		/// <summary>
		///   Returns the fully qualified type name of this instance.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0}, {1}", FirstPart, SecondPart);
		}
	}
}