namespace Pegasus.Platform.Memory
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using Logging;
	using Utilities;

	/// <summary>
	///     Pools objects of type T in order to reduce the pressure on the garbage collector. Instead of allocating a new
	///     object of type T whenever one is needed, the pool's Allocate() method should be used to retrieve a previously allocated
	///     instance, if any, or allocate a new one. Once the object is no longer being used, it must be returned to the pool
	///     so that it can be reused later on.
	/// </summary>
	/// <typeparam name="T">The type of the pooled objects.</typeparam>
	[DebuggerDisplay("{_pooledObjects.Count} of {_allocationCount} available ({typeof(T)})")]
	public sealed class ObjectPool<T> : ObjectPool
		where T : class, new()
	{
		/// <summary>
		///     The pooled objects that are currently not in use.
		/// </summary>
		private readonly Stack<T> _pooledObjects = new Stack<T>();

		/// <summary>
		///     The total number of instances allocated by the pool.
		/// </summary>
		private int _allocationCount;

#if DEBUG
		/// <summary>
		///     The allocated objects that are tracked in debug builds so that memory leaks can be debugged more easily.
		/// </summary>
		private readonly List<T> _allocatedObjects = new List<T>();
#endif

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="hasGlobalLifetime">
		///     Indicates whether the object pool should have global lifetime and should be
		///     disposed automatically during application shutdown.
		/// </param>
		public ObjectPool(bool hasGlobalLifetime = false)
		{
			if (hasGlobalLifetime)
				AddGlobalPool(this);
		}

		/// <summary>
		///     Gets a pooled object or allocates a new instance if none are currently pooled.
		/// </summary>
		public T Allocate()
		{
			T obj;
			if (_pooledObjects.Count == 0)
			{
				++_allocationCount;
				obj = new T();
#if DEBUG
				_allocatedObjects.Add(obj);
#endif
			}
			else
				obj = _pooledObjects.Pop();

			var pooledObject = obj as PooledObject;
			if (pooledObject != null)
				pooledObject.AllocatedFrom(this);

			return obj;
		}

		/// <summary>
		///     Returns an object to the pool so that it can be reused later.
		/// </summary>
		/// <param name="obj">The object that should be returned to the pool.</param>
		public override void Free(object obj)
		{
			Assert.OfType<T>(obj);
			Free(obj as T);
		}

		/// <summary>
		///     Returns an object to the pool so that it can be reused later.
		/// </summary>
		/// <param name="obj">The object that should be returned to the pool.</param>
		public void Free(T obj)
		{
			Assert.ArgumentNotNull(obj);
			Assert.ArgumentSatisfies(!_pooledObjects.Contains(obj), "The object has already been returned.");
			Assert.That(_pooledObjects.Count < _allocationCount, "More objects returned than allocated.");

			_pooledObjects.Push(obj);
		}

		/// <summary>
		///     Frees all allocated instances.
		/// </summary>
		public override void Free()
		{
			if (_allocationCount == 0)
				return;

			foreach (var obj in _pooledObjects.OfType<PooledObject>())
				obj.Free();

			Log.Debug("Released {1} pooled object(s) of type '{0}'.", typeof(T).FullName, _allocationCount);

#if DEBUG
			var leakedObjects = _allocatedObjects.Except(_pooledObjects).ToArray();
			if (leakedObjects.Length > 0)
				Log.Error("Leaked {1} object(s) of type '{0}'.", typeof(T).FullName, leakedObjects.Length);

			if (leakedObjects.Length > 0 && Debugger.IsAttached && PlatformInfo.Platform == PlatformType.Windows)
				Debugger.Break();

			_allocatedObjects.Clear();
#endif

			_pooledObjects.Clear();
			_allocationCount = 0;
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Free();
		}
	}
}