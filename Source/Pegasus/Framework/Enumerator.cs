namespace Pegasus.Framework
{
	using System;

	/// <summary>
	///     Enumerates a custom collection.
	/// </summary>
	public struct Enumerator<T>
	{
		/// <summary>
		///     Represents an enumerator that does not enumerate any items.
		/// </summary>
		public static readonly Enumerator<T> Empty = new Enumerator<T>();

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
		public static Enumerator<T> FromItem(T item)
		{
			return new Enumerator<T> { _item = item, _singleItem = item != null };
		}

		/// <summary>
		///     Creates an enumerator for a collection with multiple items.
		/// </summary>
		/// <param name="collection">The collection that should be enumerated.</param>
		public static Enumerator<T> FromElements(CustomCollection<T> collection)
		{
			Assert.ArgumentNotNull(collection);
			return new Enumerator<T> { _collection = collection, _version = collection.Version };
		}

		/// <summary>
		///     Creates an enumerator that wraps the given enumerator.
		/// </summary>
		/// <typeparam name="TWrapped">The type of the items of the wrapped enumerator.</typeparam>
		/// <param name="enumerator">The enumerator that should be wrapped.</param>
		public static WrappedEnumerator<TWrapped> FromEnumerator<TWrapped>(Enumerator<TWrapped> enumerator)
			where TWrapped : T
		{
			return new WrappedEnumerator<TWrapped>(enumerator);
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

			Assert.That(_collection.Version == _version, "The collection has been modified while it is enumerated.");

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
		public Enumerator<T> GetEnumerator()
		{
			return this;
		}

		/// <summary>
		///     Wraps another enumerator, optionally casting the enumerated items.
		/// </summary>
		/// <typeparam name="TWrapped">The type of the items of the wrapped enumerator.</typeparam>
		public struct WrappedEnumerator<TWrapped>
			where TWrapped : T
		{
			/// <summary>
			///     The wrapped enumerator that is enumerated.
			/// </summary>
			private Enumerator<TWrapped> _enumerator;

			/// <summary>
			///     Initializes a new instance.
			/// </summary>
			/// <param name="enumerator">The wrapped enumerator that is enumerated.</param>
			public WrappedEnumerator(Enumerator<TWrapped> enumerator)
				: this()
			{
				_enumerator = enumerator;
			}

			/// <summary>
			///     Gets the item at the current position of the enumerator.
			/// </summary>
			public TWrapped Current { get; private set; }

			/// <summary>
			///     Advances the enumerator to the next item.
			/// </summary>
			public bool MoveNext()
			{
				var hasNext = _enumerator.MoveNext();

				if (hasNext)
					Current = (TWrapped)_enumerator.Current;

				return hasNext;
			}

			/// <summary>
			///     Gets the enumerator that can be used with C#'s foreach loops.
			/// </summary>
			/// <remarks>
			///     This method just returns the enumerator object. It is only required to enable foreach support.
			/// </remarks>
			public WrappedEnumerator<TWrapped> GetEnumerator()
			{
				return this;
			}
		}
	}
}