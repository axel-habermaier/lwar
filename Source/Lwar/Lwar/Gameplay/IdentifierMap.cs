namespace Lwar.Gameplay
{
	using System;
	using Pegasus;

	/// <summary>
	///     Maps generational identifiers to concrete objects.
	/// </summary>
	/// <typeparam name="T">The type of the mapped objects.</typeparam>
	public sealed class IdentifierMap<T>
		where T : class, IGenerationalIdentity
	{
		/// <summary>
		///     Maps each identifier to the corresponding object.
		/// </summary>
		private readonly T[] _map = new T[UInt16.MaxValue];

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
				if (obj == null)
					return null;

				return obj.Identifier.Generation == identifier.Generation ? obj : null;
			}
		}

		/// <summary>
		///     Adds a mapping for the given object.
		/// </summary>
		/// <param name="obj">The object that should be mapped.</param>
		public void Add(T obj)
		{
			Assert.ArgumentNotNull(obj);
			Assert.That(_map[obj.Identifier.Identity] == null, "There already is a mapping for the object's identifier.");

			_map[obj.Identifier.Identity] = obj;
		}

		/// <summary>
		///     Removes the mapping for the given object.
		/// </summary>
		/// <param name="obj">The object whose mapping should be removed.</param>
		public void Remove(T obj)
		{
			Assert.ArgumentNotNull(obj);
			Assert.That(_map[obj.Identifier.Identity] != null, "The object is not mapped.");
			Assert.That(_map[obj.Identifier.Identity].Identifier.Generation == obj.Identifier.Generation,
				"Attempted to unmap an object of a different generation.");

			_map[obj.Identifier.Identity] = null;
		}
	}
}