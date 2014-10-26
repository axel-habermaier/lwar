namespace Lwar.Gameplay.Client
{
	using System;
	using Pegasus.Entities;
	using Pegasus.Utilities;

	/// <summary>
	///     Maps generational identities to objects.
	/// </summary>
	/// <typeparam name="T">The type of the mapped objects.</typeparam>
	public sealed class IdentifierMap<T>
		where T : class
	{
		/// <summary>
		///     Maps each identity to the corresponding object.
		/// </summary>
		private readonly ObjectIdentity[] _map;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="capacity">
		///     The number of objects that can be mapped. The valid identities that can be mapped fall into the
		///     range [0;capacity-1].
		/// </param>
		public IdentifierMap(int capacity)
		{
			Assert.ArgumentInRange(capacity, 0, UInt16.MaxValue);
			_map = new ObjectIdentity[capacity];
		}

		/// <summary>
		///     Gets the object that corresponds to the given identity. Returns null if no object with the given identity could
		///     be found, or if the generation did not match.
		/// </summary>
		/// <param name="identity">The identity of the object that should be returned.</param>
		public T this[Identity identity]
		{
			get
			{
				Assert.ArgumentInRange(identity.Identifier, 0, _map.Length - 1);

				var obj = _map[identity.Identifier];
				if (obj.Object == null)
					return null;

				return obj.Identifier.Generation == identity.Generation ? obj.Object : null;
			}
		}

		/// <summary>
		///     Adds a mapping for the given object.
		/// </summary>
		/// <param name="identity">The identity of the object.</param>
		/// <param name="obj">The object that should be mapped.</param>
		public void Add(Identity identity, T obj)
		{
			Assert.ArgumentInRange(identity.Identifier, 0, _map.Length - 1);
			Assert.ArgumentNotNull(obj);
			Assert.That(_map[identity.Identifier].Object == null, "There already is a mapping for the object's identity.");

			_map[identity.Identifier] = new ObjectIdentity { Object = obj, Identifier = identity };
		}

		/// <summary>
		///     Removes the mapping for the given object.
		/// </summary>
		/// <param name="identity">The identity of the object whose mapping should be removed.</param>
		public void Remove(Identity identity)
		{
			Assert.ArgumentInRange(identity.Identifier, 0, _map.Length - 1);
			Assert.That(_map[identity.Identifier].Object != null, "The object is not mapped.");
			Assert.That(_map[identity.Identifier].Identifier.Generation == identity.Generation,
				"Attempted to unmap an object of a different generation.");

			_map[identity.Identifier] = new ObjectIdentity();
		}

		/// <summary>
		///     Gets a value indicating whether the an object with the given identity is currently mapped.
		/// </summary>
		/// <param name="identity">The identity that should be checked.</param>
		public bool Contains(Identity identity)
		{
			Assert.ArgumentInRange(identity.Identifier, 0, _map.Length - 1);

			var obj = _map[identity.Identifier];
			if (obj.Object == null)
				return false;

			return obj.Identifier == identity;
		}

		/// <summary>
		///     Associates an object with its identity.
		/// </summary>
		private struct ObjectIdentity
		{
			/// <summary>
			///     The generational identity of the object.
			/// </summary>
			public Identity Identifier;

			/// <summary>
			///     The object with the generational identity.
			/// </summary>
			public T Object;
		}
	}
}