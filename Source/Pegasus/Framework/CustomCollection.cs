namespace Pegasus.Framework
{
	using System;
	using System.Collections.ObjectModel;

	/// <summary>
	///     Represents base class for custom collections.
	/// </summary>
	public abstract class CustomCollection<T> : Collection<T>
	{
		/// <summary>
		///     The version of the collection. Each modification of the collection increments the version number by one.
		/// </summary>
		private int _version;

		/// <summary>
		///     Removes all elements from the collection.
		/// </summary>
		protected override void ClearItems()
		{
			++_version;
			base.ClearItems();
		}

		/// <summary>
		///     Inserts an element into the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which the item that should be inserted.</param>
		/// <param name="item">The item that should be inserted.</param>
		protected override void InsertItem(int index, T item)
		{
			++_version;
			base.InsertItem(index, item);
		}

		/// <summary>
		///     Removes the element at the specified index of the collection.
		/// </summary>
		/// <param name="index">The zero-based index of the element that should be removed.</param>
		protected override void RemoveItem(int index)
		{
			++_version;
			base.RemoveItem(index);
		}

		/// <summary>
		///     Replaces the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element that should be replaced.</param>
		/// <param name="item">The new value for the element at the specified index.</param>
		protected override void SetItem(int index, T item)
		{
			++_version;
			base.SetItem(index, item);
		}

		/// <summary>
		///     Gets an enumerator for the collection.
		/// </summary>
		public new Enumerator GetEnumerator()
		{
			if (Count == 0)
				return Enumerator.Empty;

			return Enumerator.FromElements(this);
		}

		/// <summary>
		///     Enumerates a custom collection.
		/// </summary>
		public struct Enumerator
		{
			/// <summary>
			///     Represents an enumerator that does not enumerate any items.
			/// </summary>
			public static readonly Enumerator Empty = new Enumerator();

			/// <summary>
			///     The collection that is enumerated.
			/// </summary>
			private CustomCollection<T> _collection;

			/// <summary>
			///     The index of the current enumerated item.
			/// </summary>
			private int _current;

			/// <summary>
			///     The single item that is enumerated.
			/// </summary>
			private T _item;

			/// <summary>
			///     Indicates whether only a single item should be enumerated.
			/// </summary>
			private bool _singleItem;

			/// <summary>
			///     The version of the collection when the enumerator was created.
			/// </summary>
			private int _version;

			/// <summary>
			///     Gets the item at the current position of the enumerator.
			/// </summary>
			public T Current { get; private set; }

			/// <summary>
			///     Creates an enumerator for a single item.
			/// </summary>
			/// <param name="item">The item that should be enumerated.</param>
			public static Enumerator FromItem(T item)
			{
				return new Enumerator { _item = item, _singleItem = true };
			}

			/// <summary>
			///     Creates an enumerator for a collection with multiple items.
			/// </summary>
			/// <param name="collection">The collection that should be enumerated.</param>
			public static Enumerator FromElements(CustomCollection<T> collection)
			{
				Assert.ArgumentNotNull(collection);
				return new Enumerator { _collection = collection, _version = collection._version };
			}

			/// <summary>
			///     Advances the enumerator to the next item.
			/// </summary>
			public bool MoveNext()
			{
				// If we neither have a single item nor a collection, we're done
				if (!_singleItem && _collection == null)
					return false;

				// If we have a single item, enumerate it and make sure the next call to MoveNext returns false
				if (_singleItem)
				{
					Current = _item;
					_singleItem = false;

					return true;
				}

				Assert.That(_collection._version == _version, "The collection has been modified while it is enumerated.");

				// If we've reached the end of the collection, we're done
				if (_current == _collection.Count)
				{
					_collection = null;
					return false;
				}

				// Otherwise, enumerate the next element
				Current = _collection[_current++];
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
	}
}