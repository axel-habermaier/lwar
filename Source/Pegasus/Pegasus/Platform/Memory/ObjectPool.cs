namespace Pegasus.Platform.Memory
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;
	using Logging;

	/// <summary>
	///     Pools objects of type T in order to reduce the pressure on the garbage collector. Instead of new'ing up a new
	///     object of type T whenever one is needed, the pool's Allocate() method should be used to retrieve a previously allocated
	///     instance, if any, or allocate a new one. Once the object is no longer being used, it must be returned to the pool
	///     so that it can be reused later on.
	/// </summary>
	/// <typeparam name="T">The type of the pooled objects.</typeparam>
	public class ObjectPool<T> : DisposableObject
		where T : class
	{
		/// <summary>
		///     The factory method that is used to allocate new instances. Using a delegate instead of the generic new constraint allows
		///     more flexibility when creating new instances (i.e., providing constructor parameters, for instance), and also seems to
		///     be faster.
		/// </summary>
		private readonly Func<T> _factory;

		/// <summary>
		///     The initialization routine that is executed whenever an instance is retrieved from the pool.
		/// </summary>
		private readonly Action<T> _initialize;

		/// <summary>
		///     The pooled objects that are currently not in use.
		/// </summary>
		private readonly Stack<T> _pooledObjects = new Stack<T>();

		/// <summary>
		///     The release routine that is executed whenever an instance is returned to the pool.
		/// </summary>
		private readonly Action<T> _release;

		/// <summary>
		///     The total number of instances allocated by the pool.
		/// </summary>
		private int _allocationCount;

#if DEBUG

		/// <summary>
		///     The allocated objects that are tracked in debug builds so that memory leaks can be debugged more easily.
		/// </summary>
		private readonly List<T> _allocatedObjects = new List<T>();

		/// <summary>
		///     The identifier of the thread the pool was created on.
		/// </summary>
		private readonly int _threadId = Thread.CurrentThread.ManagedThreadId;
#endif

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="factory">The factory method that should be used to allocate new instances.</param>
		/// <param name="initialize">The initialization routine that should be executed whenever an instance is retrieved from the pool.</param>
		/// <param name="release">The release routine that should be executed whenever an instance is returned to the pool.</param>
		/// <param name="hasGlobalLifetime">
		///     Indicates whether the object pool should have global lifetime and should be
		///     disposed automatically during application shutdown.
		/// </param>
		public ObjectPool(Func<T> factory, Action<T> initialize, Action<T> release, bool hasGlobalLifetime = false)
		{
			Assert.ArgumentNotNull(factory);
			Assert.ArgumentNotNull(initialize);
			Assert.ArgumentNotNull(release);

			_factory = factory;
			_initialize = initialize;
			_release = release;

			if (hasGlobalLifetime)
				ObjectPools.AddGlobalPool(this);
		}

		/// <summary>
		///     Gets a pooled object or allocates a new instance if none are currently pooled.
		/// </summary>
		public PooledObject<T> Allocate()
		{
			ValidateThread();

			T obj;
			if (_pooledObjects.Count == 0)
			{
				++_allocationCount;
				obj = _factory();
#if DEBUG
				_allocatedObjects.Add(obj);
#endif
			}
			else
				obj = _pooledObjects.Pop();

			_initialize(obj);
			return new PooledObject<T>(obj, this);
		}

		/// <summary>
		///     Returns an object to the pool so that it can be reused later.
		/// </summary>
		/// <param name="item">The object that should be returned to the pool.</param>
		internal void Free(T item)
		{
			Assert.ArgumentNotNull(item);
			Assert.ArgumentSatisfies(!_pooledObjects.Contains(item), "The item has already been returned.");
			Assert.That(_pooledObjects.Count < _allocationCount, "More items returned than allocated.");
			ValidateThread();

			_release(item);
			_pooledObjects.Push(item);
		}

		/// <summary>
		///     In debug builds, checks that the object pool is only accessed from the thread it was created on.
		/// </summary>
		[Conditional("DEBUG")]
		private void ValidateThread()
		{
#if DEBUG
			Assert.That(_threadId == Thread.CurrentThread.ManagedThreadId,
				"Object pool is accessed from a thread other than the one that created it.");
#endif
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			ValidateThread();
			Log.Debug("ObjectPool<{0}>: {1} object(s) allocated.", typeof(T).Name, _allocationCount);

#if DEBUG
			var leakedObjects = _allocatedObjects.Except(_pooledObjects).ToArray();
			if (leakedObjects.Length > 0)
				Log.Debug("ObjectPool<{0}>: Leaked {1} object(s).", typeof(T).Name, leakedObjects.Length);

			if (leakedObjects.Length > 0 && Debugger.IsAttached)
				Debugger.Break();
#endif
		}
	}
}