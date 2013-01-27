using System;

namespace Client.Gameplay
{
	using Pegasus.Framework;

	/// <summary>
	///   Represents a unique identifier for entities. There are UInt16.MaxValue different identifiers, all assigned to
	///   UInt16.MaxValue different Entity instances. This assignment is static and entity instances are pooled, so the
	///   same identifier might refer to two logically different entities if the same entity instance is removed from a
	///   level and then re-used later on. In order to distinguish between these logically different entities, the identifier
	///   contains a generation that is updated every time the entity is added to the level. Splitting the identifier into
	///   these two parts allows for an efficient array-based look-up of entities by identifier, followed by a check of the
	///   entity generation.
	/// </summary>
	public struct EntityIdentifier : IEquatable<EntityIdentifier>
	{
		/// <summary>
		///   The identifier that was assigned last.
		/// </summary>
		private static ushort _lastAssignedId;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="identifier">The unique identifier of the entity.</param>
		/// <param name="generation">The generation of the entity.</param>
		public EntityIdentifier(ushort identifier, ushort generation)
			: this()
		{
			Identifier = identifier;
			Generation = generation;
		}

		/// <summary>
		///   Gets the unique identifier of the entity.
		/// </summary>
		public ushort Identifier { get; private set; }

		/// <summary>
		///   Gets the generation of the entity.
		/// </summary>
		public ushort Generation { get; private set; }

		/// <summary>
		///   Indicates whether the current identifier is equal to another identifier.
		/// </summary>
		/// <param name="other">An identifier to compare with this identifier.</param>
		public bool Equals(EntityIdentifier other)
		{
			return Identifier == other.Identifier && Generation == other.Generation;
		}

		/// <summary>
		///   Creates a new entity identifier with generation 0.
		/// </summary>
		public static EntityIdentifier Create()
		{
			Assert.InRange(_lastAssignedId, 0, UInt16.MaxValue - 1);
			return new EntityIdentifier(_lastAssignedId++, 0);
		}

		/// <summary>
		///   Returns a new entity identifier with an increased generation count.
		/// </summary>
		public EntityIdentifier IncreaseGenerationCount()
		{
			Assert.InRange(Generation, 0, UInt16.MaxValue - 1);
			return new EntityIdentifier(Identifier, (ushort)(Generation + 1));
		}

		/// <summary>
		///   Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to.</param>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			return obj is EntityIdentifier && Equals((EntityIdentifier)obj);
		}

		/// <summary>
		///   Returns the hash code for this instance.
		/// </summary>
		public override int GetHashCode()
		{
			return (Identifier.GetHashCode() * 397) ^ Generation.GetHashCode();
		}

		/// <summary>
		///   Checks whether the two identifiers are equal.
		/// </summary>
		/// <param name="left">The first identifier.</param>
		/// <param name="right">The second identifier.</param>
		public static bool operator ==(EntityIdentifier left, EntityIdentifier right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///   Checks whether the two identifiers are not equal.
		/// </summary>
		/// <param name="left">The first identifier.</param>
		/// <param name="right">The second identifier.</param>
		public static bool operator !=(EntityIdentifier left, EntityIdentifier right)
		{
			return !left.Equals(right);
		}

		/// <summary>
		///   Returns the string representation of the identifier.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0} (gen {1})", Identifier, Generation);
		}
	}
}