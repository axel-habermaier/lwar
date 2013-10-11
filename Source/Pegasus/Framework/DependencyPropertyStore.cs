namespace Pegasus.Framework
{
	using System;

	/// <summary>
	///   A sparse storage for dependency property values. The values are stored in an array, sorted by dependency property
	///   index. A binary search is used to find the value of a dependency property, whereas insertions are guaranteed to
	///   insert the value at the correct array index.
	/// </summary>
	internal struct DependencyPropertyStore
	{
		/// <summary>
		///   The values that are currently stored.
		/// </summary>
		private SparseObjectStorage<DependencyPropertyValue> _values;

		/// <summary>
		///   Gets the value for the given dependency property. If no value is found, null is returned.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property the value should be returned for.</param>
		public DependencyPropertyValue<T> GetValueOrNull<T>(DependencyProperty<T> property)
		{
			Assert.ArgumentNotNull(property);
			return GetValue(property, false);
		}

		/// <summary>
		///   Gets the value for the given dependency property or adds it if no value is found.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property the value should be returned for.</param>
		public DependencyPropertyValue<T> GetValueAddUnknown<T>(DependencyProperty<T> property)
		{
			Assert.ArgumentNotNull(property);
			return GetValue(property, true);
		}

		/// <summary>
		///   Gets the value for the given dependency property or adds it if no value is found and a value should be added.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property the value should be returned for.</param>
		/// <param name="addValueIfUnknown">Indicates whether a value should be added if none is found.</param>
		private DependencyPropertyValue<T> GetValue<T>(DependencyProperty<T> property, bool addValueIfUnknown)
		{
			Assert.ArgumentNotNull(property);

			var value = _values.Get(property.Index) as DependencyPropertyValue<T>;
			if (value == null && addValueIfUnknown)
				_values.Add(value = new DependencyPropertyValue<T>(property));

			return value;
		}

		/// <summary>
		///   Copies the values of all inheriting dependency properties from the given object to the given inheriting object.
		/// </summary>
		/// <param name="obj">The parent object the inherited values should be retrieved from.</param>
		/// <param name="inheritingObject">The inheriting object whose inheriting dependency properties should be set.</param>
		public void SetInheritedValues(DependencyObject obj, DependencyObject inheritingObject)
		{
			Assert.ArgumentNotNull(obj);
			Assert.ArgumentNotNull(inheritingObject);

			foreach (var value in _values.GetEnumerator())
			{
				if (!value.Property.Inherits)
					continue;

				value.Property.CopyInheritedValue(obj, inheritingObject);
			}
		}

		/// <summary>
		///   Unsets all inherited values of all inheriting dependency properties.
		/// </summary>
		/// <param name="obj">The dependency object whose inherited values should be unset.</param>
		public void UnsetInheritedValues(DependencyObject obj)
		{
			foreach (var value in _values.GetEnumerator())
			{
				if (!value.Property.Inherits)
					continue;

				value.Property.UnsetInheritedValue(obj);
			}
		}
	}
}