namespace Lwar.Gameplay
{
	using System;
	using Pegasus;

	/// <summary>
	///     Maps generational identifiers to concrete objects.
	/// </summary>
	/// <typeparam name="T">The type of the mapped objects.</typeparam>
	public sealed class IdentifierMap<T>
		where T : class
	{
		/// <summary>
		///     Maps each identifier to the corresponding object.
		/// </summary>
		private readonly ObjectIdentity[] _map = new ObjectIdentity[UInt16.MaxValue];

		/// <summary>
		///     Gets the object that corresponds to the given identifier. Returns null if no object with the given identifier could
		///     be found, or if the generation did not match.
		/// </summary>
		/// <param name="identifier">The identifier of the object that should be returned.</param>
		public T this[Identifier identifier]
		{
			get
			{
				var obj = _map[identifier.Identity];
				if (obj.Object == null)
					return null;

				return obj.Identifier.Generation == identifier.Generation ? obj.Object : null;
			}
		}

		/// <summary>
		///     Adds a mapping for the given object.
		/// </summary>
		/// <param name="obj">The object that should be mapped.</param>
		/// <param name="identifier">The identifier of the object.</param>
		public void Add(T obj, Identifier identifier)
		{
			Assert.ArgumentNotNull(obj);
			Assert.That(_map[identifier.Identity].Object == null, "There already is a mapping for the object's identifier.");

			_map[identifier.Identity] = new ObjectIdentity { Object = obj, Identifier = identifier };
		}

		/// <summary>
		///     Removes the mapping for the given object.
		/// </summary>
		/// <param name="identifier">The identifier of the object whose mapping should be removed.</param>
		public void Remove(Identifier identifier)
		{
			Assert.That(_map[identifier.Identity].Object != null, "The object is not mapped.");
			Assert.That(_map[identifier.Identity].Identifier.Generation == identifier.Generation,
				"Attempted to unmap an object of a different generation.");

			_map[identifier.Identity] = new ObjectIdentity();
		}

		/// <summary>
		///     Associates an object with its identifier.
		/// </summary>
		private struct ObjectIdentity
		{
			/// <summary>
			///     The generational identity of the object.
			/// </summary>
			public Identifier Identifier;

			/// <summary>
			///     The object with the generational identity.
			/// </summary>
			public T Object;
		}
	}
}