namespace Pegasus.Framework
{
	using System;
	using System.Linq;

	/// <summary>
	///     A sparse storage for objects implementing SpareObjectStorage.IStorageLocation. The values are stored in an array,
	///     sorted by SpareObjectStorage.IStorageLocation.Location. A binary search is used to find the value of an object,
	///     whereas insertions are guaranteed to insert the value at the correct array index.
	/// </summary>
	internal struct SparseObjectStorage<T>
		where T : class, SparseObjectStorage<T>.IStorageLocation
	{
		/// <summary>
		///     The number of stored values.
		/// </summary>
		private int _valueCount;

		/// <summary>
		///     The values that are currently stored.
		/// </summary>
		private T[] _values;

		/// <summary>
		///     Gets an enumerator for all values currently stored in the sparse object storage.
		/// </summary>
		internal Enumerator GetEnumerator()
		{
			return new Enumerator(_valueCount, _values);
		}

		/// <summary>
		///     Adds the value to the store. The new value is added such that the ordering of the values array is maintained.
		/// </summary>
		/// <param name="value">The value that should be added.</param>
		internal void Add(T value)
		{
			// Add the value at the beginning of the list if it is empty
			if (_values == null)
			{
				_values = new T[2];
				_values[0] = value;
				_valueCount = 1;
				return;
			}

			Assert.That(_values.All(v => v == null || v.Location != value.Location), "The property value has already been stored.");

			// Use a linear search to find the insertion index
			var index = 0;
			while (index < _valueCount && _values[index].Location < value.Location)
				++index;

			if (_values.Length == _valueCount)
			{
				// We have to increase the size of the store before we can add the value
				var newLength = (int)(_values.Length * 1.5);
				if (newLength == _values.Length)
					++newLength;

				var values = new T[newLength];

				// Copy all old values before the insertion index
				Array.Copy(_values, 0, values, 0, index);

				// Copy all old values after the insertion index
				Array.Copy(_values, index, values, index + 1, _valueCount - index);

				_values = values;
			}
			else
			{
				// Shift up all values at indices greater than the insertion index
				Array.Copy(_values, index, _values, index + 1, _valueCount - index);
			}

			// Add the new value at the insertion index and increase the value count
			_values[index] = value;
			++_valueCount;
		}

		/// <summary>
		///     Performs a binary search to find the value index for the given location. Returns null if no value has been stored
		///     yet at the given location.
		/// </summary>
		/// <param name="location">The index of the property the value index should be returned for.</param>
		internal T Get(int location)
		{
			if (_valueCount == 0)
				return null;

			const int linearSearchThreshold = 4;
			var start = 0;
			var end = _valueCount;

			// Perform a binary search while the element count exceeds the linear search threshold
			while (end - start > linearSearchThreshold)
			{
				// Split the interval into two halves
				var center = start + (end - start) / 2;

				// Check if the center of the interval is what we're looking for
				if (_values[center].Location == location)
					return _values[center];

				// Continue searching the lower or the upper half
				if (_values[center].Location > location)
					end = center - 1;
				else
					start = center + 1;
			}

			// We'll use a linear search if there are only a few elements left to search, as profiling seems to indicate
			// that a simple linear search is more efficient in this case
			for (var i = start; i < end + 1 && i < _valueCount; ++i)
			{
				if (_values[i].Location == location)
					return _values[i];
			}

			return null;
		}

		/// <summary>
		///     Enumerates all stored values of a sparse object storage.
		/// </summary>
		internal struct Enumerator
		{
			/// <summary>
			///     The number of values that are enumerated.
			/// </summary>
			private readonly int _valueCount;

			/// <summary>
			///     The index of the current enumerated element.
			/// </summary>
			private int _current;

			/// <summary>
			///     The values that are enumerated.
			/// </summary>
			private T[] _values;

			/// <summary>
			///     Initializes a new instance.
			/// </summary>
			/// <param name="valueCount">The number of values that should be enumerated.</param>
			/// <param name="values">The values that should be enumerated.</param>
			public Enumerator(int valueCount, T[] values)
				: this()
			{
				Assert.ArgumentSatisfies(values == null || valueCount <= values.Length, "Too many values.");

				_valueCount = valueCount;
				_values = values;
			}

			/// <summary>
			///     Gets the element at the current position of the enumerator.
			/// </summary>
			public T Current { get; private set; }

			/// <summary>
			///     Advances the enumerator to the next UI element.
			/// </summary>
			public bool MoveNext()
			{
				// If the values array is null, we're done enumerating
				if (_values == null)
					return false;

				// If we've reached the end of the collection, we're done
				if (_current == _valueCount)
				{
					_values = null;
					return false;
				}

				// Otherwise, enumerate the next element
				Current = _values[_current++];
				return true;
			}

			/// <summary>
			///     Gets the enumerator that can be used with C#'s foreach loops.
			/// </summary>
			/// <remarks>
			///     This method just returns the enumerator object. It is only required to enable foreach support.
			/// </remarks>
			public Enumerator GetEnumerator()
			{
				return this;
			}
		}

		/// <summary>
		///     Provides a storage location for objects that should be stored in a sparse object storage.
		/// </summary>
		internal interface IStorageLocation
		{
			/// <summary>
			///     The storage location of the value that remains unchanged and unique throughout the lifetime of the application.
			/// </summary>
			int Location { get; }
		}
	}
}