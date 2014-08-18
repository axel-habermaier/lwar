namespace Pegasus.Framework.UserInterface
{
	using System;
	using System.Collections.ObjectModel;

	/// <summary>
	///     Represents a list of objects that can no longer be changed once it has been sealed.
	/// </summary>
	/// <typeparam name="T">The type of the objects contained in the collection.</typeparam>
	public class SealableCollection<T> : Collection<T>, ISealable
		where T : ISealable
	{
		/// <summary>
		///     Represents an empty sealable collection that cannot be modified.
		/// </summary>
		public static readonly SealableCollection<T> Empty = new SealableCollection<T> { IsSealed = true };

		/// <summary>
		///     The version of the collection. Each modification of the collection increments the version number by one.
		/// </summary>
		private int _version;

		/// <summary>
		///     Gets a value indicating whether the collection is sealed and can no longer be modified.
		/// </summary>
		public bool IsSealed { get; private set; }

		/// <summary>
		///     Seals the collection such that it can no longer be modified.
		/// </summary>
		public void Seal()
		{
			IsSealed = true;

			foreach (var item in this)
				item.Seal();
		}

		/// <summary>
		///     Removes all elements from the collection.
		/// </summary>
		protected override void ClearItems()
		{
			Assert.NotSealed(this);

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
			Assert.NotSealed(this);

			++_version;
			base.InsertItem(index, item);
		}

		/// <summary>
		///     Removes the element at the specified index of the collection.
		/// </summary>
		/// <param name="index">The zero-based index of the element that should be removed.</param>
		protected override void RemoveItem(int index)
		{
			Assert.NotSealed(this);

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
			Assert.NotSealed(this);

			++_version;
			base.SetItem(index, item);
		}

		/// <summary>
		///     Gets an enumerator for the collection.
		/// </summary>
		/// <Remarks>This method returns a custom enumerator in order to avoid a heap allocation.</Remarks>
		public new Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		/// <summary>
		///     Enumerates a sealable collection.
		/// </summary>
		public struct Enumerator
		{
			/// <summary>
			///     The version of the collection when the enumerator was created.
			/// </summary>
			private readonly int _version;

			/// <summary>
			///     The elements that are enumerated.
			/// </summary>
			private SealableCollection<T> _collection;

			/// <summary>
			///     The index of the current enumerated element.
			/// </summary>
			private int _current;

			/// <summary>
			///     Creates a new instance.
			/// </summary>
			/// <param name="collection">The elements that should be enumerated.</param>
			public Enumerator(SealableCollection<T> collection)
				: this()
			{
				Assert.ArgumentNotNull(collection);

				_collection = collection;
				_version = collection._version;
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
		}
	}
}