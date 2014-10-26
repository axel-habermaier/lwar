namespace Pegasus.UserInterface
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Utilities;

	/// <summary>
	///     A sparse storage for objects uniquely identified by an index. The values are stored in an array by index.
	///     A binary search is used to find the value of an object, whereas insertions are guaranteed to insert the
	///     value at the correct array index.
	/// </summary>
	internal struct SparseObjectStorage<T>
		where T : class
	{
		/// <summary>
		///     The number of stored objects.
		/// </summary>
		private int _count;

		/// <summary>
		///     The indexed objects that are currently stored.
		/// </summary>
		private IndexedObject[] _objects;

		/// <summary>
		///     Gets an enumerator for all objects currently stored in the sparse object storage.
		/// </summary>
		public Enumerator GetEnumerator()
		{
			return new Enumerator(_count, _objects);
		}

		/// <summary>
		///     Adds the objects to the store. The new objects is added such that the ordering of the objects array is maintained.
		/// </summary>
		/// <param name="obj">The object that should be added.</param>
		/// <param name="objectIndex">The index of the object that should be added.</param>
		internal void Add(T obj, int objectIndex)
		{
			Assert.ArgumentSatisfies(objectIndex >= 0, "Invalid negative index.");

			// Add the object at the beginning of the list if it is empty
			if (_objects == null)
			{
				_objects = new IndexedObject[2];
				_objects[0] = new IndexedObject { Object = obj, Index = objectIndex };
				_count = 1;
				return;
			}

			Assert.That(_objects.All(o => o.Object == null || o.Index != objectIndex), "The property value has already been stored.");

			// Use a linear search to find the insertion index
			var index = 0;
			while (index < _count && _objects[index].Index < objectIndex)
				++index;

			if (_objects.Length == _count)
			{
				// We have to increase the size of the store before we can add the object
				var newLength = (int)(_objects.Length * 1.5);
				if (newLength == _objects.Length)
					++newLength;

				var objects = new IndexedObject[newLength];

				// Copy all old objects before the insertion index
				Array.Copy(_objects, 0, objects, 0, index);

				// Copy all old objects after the insertion index
				Array.Copy(_objects, index, objects, index + 1, _count - index);

				_objects = objects;
			}
			else
			{
				// Shift up all objects at indices greater than the insertion index
				Array.Copy(_objects, index, _objects, index + 1, _count - index);
			}

			// Add the new object at the insertion index and increase the object count
			_objects[index] = new IndexedObject { Object = obj, Index = objectIndex };
			++_count;
		}

		/// <summary>
		///     Performs a binary search to find the object for the given index. Returns null if no object has been stored
		///     yet at the given index.
		/// </summary>
		/// <param name="index">The index of the object that should be returned.</param>
		internal T Get(int index)
		{
			if (_count == 0)
				return null;

			const int linearSearchThreshold = 4;
			var start = 0;
			var end = _count;

			// Perform a binary search while the object count exceeds the linear search threshold
			while (end - start > linearSearchThreshold)
			{
				// Split the interval into two halves
				var center = start + (end - start) / 2;

				// Check if the center of the interval is what we're looking for
				if (_objects[center].Index == index)
					return _objects[center].Object;

				// Continue searching the lower or the upper half
				if (_objects[center].Index > index)
					end = center - 1;
				else
					start = center + 1;
			}

			// We'll use a linear search if there are only a few elements left to search, as profiling seems to indicate
			// that a simple linear search is more efficient in this case
			for (var i = start; i < end + 1 && i < _count; ++i)
			{
				if (_objects[i].Index == index)
					return _objects[i].Object;
			}

			return null;
		}

		/// <summary>
		///     Enumerates all stored objects of a sparse object storage.
		/// </summary>
		internal struct Enumerator
		{
			/// <summary>
			///     Cached instance storing the copies of the object arrays. Necessary as the enumerator must be reentrant, but on the other
			///     hand we do not want to create new arrays all the time.
			/// </summary>
			private static readonly Stack<IndexedObject[]> Pooled = new Stack<IndexedObject[]>();

			/// <summary>
			///     The number of objects that are enumerated.
			/// </summary>
			private readonly int _count;

			/// <summary>
			///     The objects that are enumerated.
			/// </summary>
			private readonly IndexedObject[] _objects;

			/// <summary>
			///     The index of the current enumerated object.
			/// </summary>
			private int _current;

			/// <summary>
			///     Initializes a new instance.
			/// </summary>
			/// <param name="count">The number of objects that should be enumerated.</param>
			/// <param name="objects">The objects that should be enumerated.</param>
			internal Enumerator(int count, IndexedObject[] objects)
				: this()
			{
				Assert.ArgumentSatisfies(objects == null || count <= objects.Length, "Too many values.");

				_count = count;

				// We have to make a copy of the objects here, as the list may be changed while it is being enumerated.
				// We copying the objects to a temporary array that is used for as long as the enumerator hasn't reached
				// the end; this leaks the temporary arrays if the enumerator is not fully enumerated, however, that should
				// never be the case. We're pooling the temporary arrays to reduce garbage collections.

				if (objects == null)
					return;

				if (Pooled.Count == 0)
					_objects = new IndexedObject[objects.Length];
				else
				{
					// We simply take the first temporary array on the stack and discard it if it is too small. This might
					// cause more garbage than necessary, however, eventually all pooled arrays will become large enough
					// and no further allocations occur.
					_objects = Pooled.Pop();
					if (_objects.Length < objects.Length)
						_objects = new IndexedObject[objects.Length];
				}

				Array.Copy(objects, _objects, objects.Length);
			}

			/// <summary>
			///     Gets the object at the current position of the enumerator.
			/// </summary>
			public T Current { get; private set; }

			/// <summary>
			///     Advances the enumerator to the next object.
			/// </summary>
			public bool MoveNext()
			{
				// If we've reached the end of the collection, we're done
				if (_current >= _count)
				{
					if (_objects != null)
						Pooled.Push(_objects);

					return false;
				}

				// Otherwise, enumerate the next element
				Current = _objects[_current++].Object;
				return true;
			}
		}

		/// <summary>
		///     Associates an object with its index.
		/// </summary>
		internal struct IndexedObject
		{
			/// <summary>
			///     The index of the object.
			/// </summary>
			public int Index;

			/// <summary>
			///     The index object.
			/// </summary>
			public T Object;
		}
	}
}