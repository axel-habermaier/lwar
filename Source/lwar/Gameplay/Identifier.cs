using System;

namespace Lwar.Gameplay
{
	using Pegasus;

	/// <summary>
	///   Represents a unique identifier. There are UInt16.MaxValue different identifiers, all assigned to
	///   UInt16.MaxValue different instances. This assignment is static and instances are pooled, so the
	///   same identifier might refer to two logically different instances if the same instance is re-used later on. In order
	///   to distinguish between these logically different instances, the identifier contains a generation that is updated
	///   every time the instance is re-used. Splitting the identifier into these two parts allows for an efficient array-based
	///   look-up of instances by identifier, followed by a check of the generation.
	/// </summary>
	public struct Identifier : IEquatable<Identifier>
	{
		/// <summary>
		///   The identifier that was assigned last.
		/// </summary>
		private static ushort _lastAssignedId;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="identifier">The unique identifier of the instance.</param>
		/// <param name="generation">The generation of the instance.</param>
		public Identifier(ushort identifier, ushort generation)
			: this()
		{
			Identity = identifier;
			Generation = generation;
		}

		/// <summary>
		///   Gets the identity of the instance.
		/// </summary>
		public ushort Identity { get; private set; }

		/// <summary>
		///   Gets the generation of the instance.
		/// </summary>
		public ushort Generation { get; private set; }

		/// <summary>
		///   Indicates whether the current identifier is equal to another identifier.
		/// </summary>
		/// <param name="other">An identifier to compare with this identifier.</param>
		public bool Equals(Identifier other)
		{
			return Identity == other.Identity && Generation == other.Generation;
		}

		/// <summary>
		///   Creates a new identifier with generation 0.
		/// </summary>
		public static Identifier Create()
		{
			Assert.InRange(_lastAssignedId, 0, UInt16.MaxValue - 1);
			return new Identifier(_lastAssignedId++, 0);
		}

		/// <summary>
		///   Returns a new identifier with an increased generation count.
		/// </summary>
		public Identifier IncreaseGenerationCount()
		{
			Assert.InRange(Generation, 0, UInt16.MaxValue - 1);
			return new Identifier(Identity, (ushort)(Generation + 1));
		}

		/// <summary>
		///   Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to.</param>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			return obj is Identifier && Equals((Identifier)obj);
		}

		/// <summary>
		///   Returns the hash code for this instance.
		/// </summary>
		public override int GetHashCode()
		{
			return (Identity.GetHashCode() * 397) ^ Generation.GetHashCode();
		}

		/// <summary>
		///   Checks whether the two identifiers are equal.
		/// </summary>
		/// <param name="left">The first identifier.</param>
		/// <param name="right">The second identifier.</param>
		public static bool operator ==(Identifier left, Identifier right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///   Checks whether the two identifiers are not equal.
		/// </summary>
		/// <param name="left">The first identifier.</param>
		/// <param name="right">The second identifier.</param>
		public static bool operator !=(Identifier left, Identifier right)
		{
			return !left.Equals(right);
		}

		/// <summary>
		///   Returns the string representation of the identifier.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0} (gen {1})", Identity, Generation);
		}
	}
}