﻿namespace Pegasus.Entities
{
	using System;

	/// <summary>
	///     Represents a unique identifier for entities that can also be used to synchronize entities over the network. There are
	///     UInt16.MaxValue different identifiers with a generation that is updated every time an identifier is re-used.
	/// </summary>
	public struct Identity : IEquatable<Identity>
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="identifier">The unique identifier of the instance.</param>
		/// <param name="generation">The generation of the instance.</param>
		public Identity(ushort identifier, ushort generation)
			: this()
		{
			Identifier = identifier;
			Generation = generation;
		}

		/// <summary>
		///     Gets the identifier of the instance.
		/// </summary>
		public ushort Identifier { get; private set; }

		/// <summary>
		///     Gets the generation of the instance.
		/// </summary>
		public ushort Generation { get; private set; }

		/// <summary>
		///     Indicates whether the current identifier is equal to another identifier.
		/// </summary>
		/// <param name="other">An identifier to compare with this identifier.</param>
		public bool Equals(Identity other)
		{
			return Identifier == other.Identifier && Generation == other.Generation;
		}

		/// <summary>
		///     Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to.</param>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			return obj is Identity && Equals((Identity)obj);
		}

		/// <summary>
		///     Returns the hash code for this instance.
		/// </summary>
		public override int GetHashCode()
		{
			return (Identifier.GetHashCode() * 397) ^ Generation.GetHashCode();
		}

		/// <summary>
		///     Checks whether the two identifiers are equal.
		/// </summary>
		/// <param name="left">The first identifier.</param>
		/// <param name="right">The second identifier.</param>
		public static bool operator ==(Identity left, Identity right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///     Checks whether the two identifiers are not equal.
		/// </summary>
		/// <param name="left">The first identifier.</param>
		/// <param name="right">The second identifier.</param>
		public static bool operator !=(Identity left, Identity right)
		{
			return !left.Equals(right);
		}

		/// <summary>
		///     Returns the string representation of the identifier.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0}({1})", Identifier, Generation);
		}
	}
}