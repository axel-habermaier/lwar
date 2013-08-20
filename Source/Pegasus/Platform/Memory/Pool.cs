using System;

namespace Pegasus.Platform.Memory
{
	using System.Collections.Generic;
	using Logging;

	/// <summary>
	///   Base implementation of pooled allocators.
	/// </summary>
	/// <typeparam name="T">The type of the pooled objects.</typeparam>
	public abstract class Pool<T>
		where T : class
	{
		/// <summary>
		///   The initial number of pooled instances. If the pool runs out of instances, the capacity is doubled.
		/// </summary>
		private const int InitialCapacity = 16;

		/// <summary>
		///   The pooled items that are currently not in use.
		/// </summary>
		private readonly List<T> _items = new List<T>(InitialCapacity);

		/// <summary>
		///   The total number of instances allocated by the pool.
		/// </summary>
		private int _allocationCount = InitialCapacity;

		/// <summary>
		///   The maximum number of instances that have been in use at the same time.
		/// </summary>
		private int _maxInUse;

		/// <summary>
		///   Allocates new array instances.
		/// </summary>
		private void AllocateObjects()
		{
			Log.DebugInfo("Pool<{0}>: Allocating new instances ({1} instances total).", typeof(T).Name, _allocationCount);

			AllocateObjects(_items, _allocationCount);
			_allocationCount *= 2;
		}

		/// <summary>
		///   Allocates new objects.
		/// </summary>
		/// <param name="items">The list in which the newly allocated items should be stored.</param>
		/// <param name="count">The number of items that should be allocated.</param>
		protected abstract void AllocateObjects(List<T> items, int count);

		/// <summary>
		///   Gets a pooled object.
		/// </summary>
		public T Get()
		{
			if (_items.Count == 0)
				AllocateObjects();

			var index = _items.Count - 1;
			var item = _items[index];
			_items.RemoveAt(index);

			var inUse = _allocationCount - _items.Count;
			if (inUse > _maxInUse)
				_maxInUse = inUse;

			return item;
		}

		/// <summary>
		///   Returns an object to the pool that is no longer being used.
		/// </summary>
		/// <param name="item">The object that should be returned to the pool.</param>
		public void Return(T item)
		{
			Assert.ArgumentNotNull(item);
			Assert.ArgumentSatisfies(!_items.Contains(item), "The item has already been returned.");
			Assert.That(_items.Count < _allocationCount, "More items returned than allocated.");

			_items.Add(item);
		}
	}
}