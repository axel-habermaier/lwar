﻿namespace Lwar.Gameplay
{
	using System;
	using System.Collections.Generic;
	using Pegasus;
	using Pegasus.Platform.Memory;

	/// <summary>
	///     Represents a list where additions and removals are deferred until some later point in time.
	/// </summary>
	/// <typeparam name="T">The type of the elements contained in the list.</typeparam>
	public class DeferredList<T> : DisposableObject
		where T : class, IDisposable
	{
		/// <summary>
		///     A cached list that is used when the actual items are retrieved from the list.
		/// </summary>
		private readonly List<T> _actualItems = new List<T>();

		/// <summary>
		///     The items that will be added to the list.
		/// </summary>
		private readonly List<T> _addedItems = new List<T>();

		/// <summary>
		///     The items that are in the list.
		/// </summary>
		private readonly List<T> _items = new List<T>();

		/// <summary>
		///     Indicates whether the order of the items in the list is preserved across removal operations.
		/// </summary>
		private readonly bool _preserveOrder;

		/// <summary>
		///     The items that will be removed from the list.
		/// </summary>
		private readonly List<T> _removedItems = new List<T>();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="preserveOrder">
		///     Indicates whether the order of the items in the list should be preserved during removal operations. If the order is
		///     unimportant, removal operations will be more efficient if this flag is set to true.
		/// </param>
		public DeferredList(bool preserveOrder = true)
		{
			_preserveOrder = preserveOrder;
		}

		/// <summary>
		///     Gets the number of items in the list.
		/// </summary>
		public int Count
		{
			get
			{
				Assert.NotDisposed(this);
				return _items.Count;
			}
		}

		/// <summary>
		///     Returns the item at the indicated position.
		/// </summary>
		/// <param name="index">The index of the item that should be returned.</param>
		public T this[int index]
		{
			get
			{
				Assert.NotDisposed(this);
				Assert.ArgumentInRange(index, 0, _items.Count);

				return _items[index];
			}
		}

		/// <summary>
		///     Gets the items actually contained in the list as if all deferred removals and additions already had their effect on the
		///     list.
		/// </summary>
		public List<T> GetActualItems()
		{
			_actualItems.Clear();

			// Don't use AddRange here (it allocates)
			foreach (var item in _addedItems)
				_actualItems.Add(item);

			// Don't use Linq + AddRange here (high allocation cost)
			foreach (var item in _items)
			{
				if (!_removedItems.Contains(item))
					_actualItems.Add(item);
			}

			return _actualItems;
		}

		/// <summary>
		///     Updates the list, processing all deferred additions and removals.
		/// </summary>
		public void Update()
		{
			Assert.NotDisposed(this);

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

			foreach (var addedItem in _addedItems)
				_items.Add(addedItem);

			_removedItems.SafeDisposeAll();

			_removedItems.Clear();
			_addedItems.Clear();
		}

		/// <summary>
		///     Adds the item to the list. The actual addition is deferred until the next time the list is updated.
		/// </summary>
		/// <param name="item">The item that should be added to the list.</param>
		public void Add(T item)
		{
			Assert.NotDisposed(this);
			Assert.ArgumentSatisfies(!_addedItems.Contains(item), "The item has already been added to the list.");
			Assert.ArgumentSatisfies(!_items.Contains(item), "The item is already contained in the list.");
			Assert.ArgumentSatisfies(!_removedItems.Contains(item), "The item has already been removed from the list.");

			_addedItems.Add(item);
		}

		/// <summary>
		///     Removes the item from the list. The actual removal is deferred until the next time the list is updated.
		/// </summary>
		/// <param name="item">The item that should be removed from the list.</param>
		public void Remove(T item)
		{
			Assert.NotDisposed(this);
			Assert.ArgumentSatisfies(!_removedItems.Contains(item), "The item has already been removed from the list.");

			_addedItems.Remove(item);
			_removedItems.Add(item);
		}

		/// <summary>
		///     Removes all items and newly added items from the list. The actual clearing is deferred until the
		///     next time the list is updated.
		/// </summary>
		public void Clear()
		{
			Assert.NotDisposed(this);

			_removedItems.AddRange(_addedItems);
			_removedItems.AddRange(_items);
			_addedItems.Clear();
		}

		/// <summary>
		///     Enumerates all items in the list.
		/// </summary>
		public List<T>.Enumerator GetEnumerator()
		{
			Assert.NotDisposed(this);
			return _items.GetEnumerator();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Update();
			_items.SafeDisposeAll();
		}
	}
}