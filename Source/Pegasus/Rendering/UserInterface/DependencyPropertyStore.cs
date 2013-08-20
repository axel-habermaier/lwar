using System;

namespace Pegasus.Rendering.UserInterface
{
	using System.Linq;

	/// <summary>
	///   A sparse storage for dependency property values. The values are stored in an array, sorted by dependency property
	///   index. A binary search is used to find the value of a dependency property, whereas insertions are guaranteed to
	///   insert the value at the correct array index.
	/// </summary>
	internal struct DependencyPropertyStore
	{
		/// <summary>
		///   The number of stored values.
		/// </summary>
		private int _valueCount;

		/// <summary>
		///   The values that are currently stored.
		/// </summary>
		private DependencyPropertyValue[] _values;

		/// <summary>
		///   Gets the value for the given dependency property.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property the value should be returned for.</param>
		/// <param name="addIfNotFound">If true, a new value is added if none is found; otherwise, null is returned.</param>
		public DependencyPropertyValue<T> GetValue<T>(DependencyProperty<T> property, bool addIfNotFound)
		{
			Assert.ArgumentNotNull(property);

			DependencyPropertyValue<T> value = null;
			var valueIndex = FindValueIndex(property.Index);

			if (valueIndex == -1 && addIfNotFound)
				AddValue(value = new DependencyPropertyValue<T>(property.Index));
			else if (valueIndex != -1)
				value = _values[valueIndex] as DependencyPropertyValue<T>;

			return value;
		}

		/// <summary>
		///   Adds the value to the store. The new value is added such that the ordering of the values array is maintained.
		/// </summary>
		/// <param name="value">The value that should be added.</param>
		private void AddValue(DependencyPropertyValue value)
		{
			// Add the value at the beginning of the list if it is empty
			if (_values == null)
			{
				_values = new DependencyPropertyValue[2];
				_values[0] = value;
				_valueCount = 1;
				return;
			}

			Assert.That(_values.All(v => v == null || v.PropertyIndex != value.PropertyIndex), "The property value has already been stored.");

			// Use a linear search to find the insertion index
			var index = 0;
			while (index < _valueCount && _values[index].PropertyIndex < value.PropertyIndex)
				++index;

			if (_values.Length == _valueCount)
			{
				// We have to increase the size of the store before we can add the value
				var newLength = (int)(_values.Length * 1.5);
				if (newLength == _values.Length)
					++newLength;

				var values = new DependencyPropertyValue[newLength];

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
		///   Performs a binary search to find the value index for the given property index. Returns -1 if no value has been stored
		///   yet for the given property index.
		/// </summary>
		/// <param name="propertyIndex">The index of the property the value index should be returned for.</param>
		private int FindValueIndex(int propertyIndex)
		{
			if (_valueCount == 0)
				return -1;

			var start = 0;
			var end = _valueCount;

			// Perform a binary search while there are more than three elements
			while (end - start > 3)
			{
				// Split the interval into two halves
				var center = start + (end - start) / 2;

				// Check if the center of the interval is what we're looking for
				if (_values[center].PropertyIndex == propertyIndex)
					return center;

				// Continue searching the lower or the upper half
				if (_values[center].PropertyIndex > propertyIndex)
					end = center - 1;
				else
					start = center + 1;
			}

			// For less than three elements, just use a linear search
			for (var i = start; i < end + 1 && i < _valueCount; ++i)
			{
				if (_values[i].PropertyIndex == propertyIndex)
					return i;
			}

			return -1;
		}
	}
}