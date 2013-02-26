using System;

namespace Lwar.Client.Gameplay
{
	using System.Collections;
	using System.Collections.Generic;
	using Pegasus.Framework;

	/// <summary>
	///   Represents a list where additions and removals are deferred until some later point in time.
	/// </summary>
	/// <typeparam name="T">The type of the elements contained in the list.</typeparam>
	public sealed class DeferredList<T> : IEnumerable<T>
		where T : class, IDisposable
	{
		/// <summary>
		///   The items that will be added to the list.
		/// </summary>
		private readonly List<T> _addedItems = new List<T>();

		/// <summary>
		///   The items that are in the list.
		/// </summary>
		private readonly List<T> _items = new List<T>();

		/// <summary>
		///   Indicates whether the order of the items in the list is preserved across removal operations.
		/// </summary>
		private readonly bool _preserveOrder;

		/// <summary>
		///   The items that will be removed from the list.
		/// </summary>
		private readonly List<T> _removedItems = new List<T>();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="preserveOrder">
		///   Indicates whether the order of the items in the list should be preserved during removal operations. If the order is
		///   unimportant, removal operations will be more efficient if this flag is set to true.
		/// </param>
		public DeferredList(bool preserveOrder = true)
		{
			_preserveOrder = preserveOrder;
		}

		/// <summary>
		///   Gets the number of items in the list.
		/// </summary>
		public int Count
		{
			get { return _items.Count; }
		}

		/// <summary>
		///   Returns the item at the indicated position.
		/// </summary>
		/// <param name="index">The index of the item that should be returned.</param>
		public T this[int index]
		{
			get
			{
				Assert.ArgumentInRange(index, () => index, 0, _items.Count);
				return _items[index];
			}
		}

		/// <summary>
		///   Enumerates all items in the list.
		/// </summary>
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		///   Enumerates all items in the list.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		///   Updates the list, processing all deferred additions and removals.
		/// </summary>
		public void Update()
		{
			foreach (var removedItem in _removedItems)
			{
				if (_preserveOrder)
					_items.Remove(removedItem);
				else
				{
					var index = _items.IndexOf(removedItem);
					if (index == -1)
						continue;

					var last = _items.Count - 1;
					_items[index] = _items[last];
					_items.RemoveAt(last);
				}
			}

			_items.AddRange(_addedItems);
			_removedItems.SafeDisposeAll();

			_removedItems.Clear();
			_addedItems.Clear();
		}

		/// <summary>
		///   Adds the item to the list. The actual addition is deferred until the next time the list is updated.
		/// </summary>
		/// <param name="item">The item that should be added to the list.</param>
		public void Add(T item)
		{
			Assert.ArgumentSatisfies(!_addedItems.Contains(item), () => item, "The item has already been added to the list.");
			Assert.ArgumentSatisfies(!_items.Contains(item), () => item, "The item is already contained in the list.");
			Assert.ArgumentSatisfies(!_removedItems.Contains(item), () => item, "The item has already been removed from the list.");

			_addedItems.Add(item);
		}

		/// <summary>
		///   Removes the item from the list. The actual removal is deferred until the next time the list is updated.
		/// </summary>
		/// <param name="item">The item that should be removed from the list.</param>
		public void Remove(T item)
		{
			Assert.ArgumentSatisfies(!_addedItems.Contains(item), () => item, "The item is being added to the list.");
			Assert.ArgumentSatisfies(_items.Contains(item), () => item, "The item is not contained in the list.");
			Assert.ArgumentSatisfies(!_removedItems.Contains(item), () => item, "The item has already been removed from the list.");

			_removedItems.Add(item);
		}

		/// <summary>
		///   Removes all items and newly added items from the list. The actual removal is deferred until the next time the list is
		///   updated.
		/// </summary>
		public void Clear()
		{
			_removedItems.AddRange(_addedItems);
			_removedItems.AddRange(_items);
		}

		/// <summary>
		///   Enumerates all items in the list.
		/// </summary>
		public List<T>.Enumerator GetEnumerator()
		{
			return _items.GetEnumerator();
		}
	}
}