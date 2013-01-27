using System;

namespace Pegasus.Framework
{
	using System.Collections.Generic;
	using Platform;

	/// <summary>
	///   Pools objects of type T in order to reduce the pressure on the garbage collector. Instead of new'ing up a new object
	///   of type T whenever one is needed, the pool's Get() method should be used to retrieve a previously allocated instance.
	///   Once the object is no longer being used, it must be returned to the pool so that it can be reused later on. If the
	///   pool runs out of instances, it batch-creates several new ones.
	/// </summary>
	/// <typeparam name="T">The type of the pooled objects.</typeparam>
	internal class Pool<T>
		where T : class, IDisposable, new()
	{
		/// <summary>
		///   The initial number of pooled instances. If the pool runs out of instances, another 'Capacity' many instances are
		///   allocated.
		/// </summary>
		private const int Capacity = 16;

		/// <summary>
		///   The pooled items that are currently not in use.
		/// </summary>
		private readonly List<T> _items = new List<T>(Capacity);

		/// <summary>
		///   The total number of instances allocated by the pool.
		/// </summary>
		private int _allocationCount;

		/// <summary>
		///   The maximum number of instances that have been in use at the same time.
		/// </summary>
		private int _maxInUse;

		/// <summary>
		///   Initializes the type.
		/// </summary>
		public Pool()
		{
			AllocateObjects();
		}

		/// <summary>
		///   Allocates the given number of objects.
		/// </summary>
		private void AllocateObjects()
		{
			_allocationCount += Capacity;
			Log.DebugInfo("Pool<{0}>: Allocating new objects ({1} objects total).", typeof(T).Name, _allocationCount);

			for (var i = 0; i < Capacity; ++i)
				_items.Add(new T());
		}

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
			Assert.ArgumentNotNull(item, () => item);
			Assert.ArgumentSatisfies(!_items.Contains(item), () => item, "The item has already been returned.");
			Assert.That(_items.Count < _allocationCount, "More items returned than allocated.");

			_items.Add(item);
		}
	}
}