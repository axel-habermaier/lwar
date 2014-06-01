namespace Pegasus.Framework
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;

	/// <summary>
	///     Represents base class for custom collections.
	/// </summary>
	public abstract class CustomCollection<T> : Collection<T>
	{
		/// <summary>
		///     Initializes a new, empty instance.
		/// </summary>
		protected CustomCollection()
		{
		}

		/// <summary>
		///     Initializes a new instance, containing all items from the given list.
		/// </summary>
		/// <param name="list">The list of items that should be contained in the new instance.</param>
		protected CustomCollection(IList<T> list)
			: base(list)
		{
		}

		/// <summary>
		///     Gets the version of the collection. Each modification of the collection increments the version number by one.
		/// </summary>
		internal int Version { get; private set; }

		/// <summary>
		///     Removes all elements from the collection.
		/// </summary>
		protected override void ClearItems()
		{
			++Version;
			base.ClearItems();
		}

		/// <summary>
		///     Adds the items to the collection.
		/// </summary>
		/// <param name="items">The items that should be added.</param>
		public void AddRange(IEnumerable<T> items)
		{
			Assert.ArgumentNotNull(items);

			foreach (var item in items)
				Add(item);
		}

		/// <summary>
		///     Adds the items to the collection.
		/// </summary>
		/// <param name="items">The items that should be added.</param>
		/// <remarks>Performance optimization (foreach does not allocate an enumerator object on the heap).</remarks>
		public void AddRange(List<T> items)
		{
			Assert.ArgumentNotNull(items);

			foreach (var item in items)
				Add(item);
		}

		/// <summary>
		///     Adds the items to the collection.
		/// </summary>
		/// <param name="items">The items that should be added.</param>
		/// <remarks>Performance optimization (foreach does not allocate an enumerator object on the heap).</remarks>
		public void AddRange(T[] items)
		{
			Assert.ArgumentNotNull(items);

			foreach (var item in items)
				Add(item);
		}

		/// <summary>
		///     Inserts an element into the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which the item that should be inserted.</param>
		/// <param name="item">The item that should be inserted.</param>
		protected override void InsertItem(int index, T item)
		{
			++Version;
			base.InsertItem(index, item);
		}

		/// <summary>
		///     Removes the element at the specified index of the collection.
		/// </summary>
		/// <param name="index">The zero-based index of the element that should be removed.</param>
		protected override void RemoveItem(int index)
		{
			++Version;
			base.RemoveItem(index);
		}

		/// <summary>
		///     Replaces the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element that should be replaced.</param>
		/// <param name="item">The new value for the element at the specified index.</param>
		protected override void SetItem(int index, T item)
		{
			++Version;
			base.SetItem(index, item);
		}

		/// <summary>
		///     Gets an enumerator for the collection.
		/// </summary>
		public new Enumerator<T> GetEnumerator()
		{
			if (Count == 0)
				return Enumerator<T>.Empty;

			return Enumerator<T>.FromElements(this);
		}
	}
}