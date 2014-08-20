namespace Pegasus.Platform.Memory
{
	using System;

	/// <summary>
	///     Holds a reference to an object allocated from an object pool, returning it to the pool when the instance is disposed.
	/// </summary>
	/// <typeparam name="T">The type of the pooled object.</typeparam>
	public struct PooledObject<T> : IDisposable
		where T : class
	{
		/// <summary>
		///     The object pool the object should be returned to.
		/// </summary>
		private readonly ObjectPool<T> _pool;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="obj">The pooled object.</param>
		/// <param name="pool">The object pool the object should be returned to.</param>
		public PooledObject(T obj, ObjectPool<T> pool)
			: this()
		{
			Assert.ArgumentNotNull(obj);
			Assert.ArgumentNotNull(pool);

			Object = obj;
			_pool = pool;
		}

		/// <summary>
		///     Gets the pooled object.
		/// </summary>
		public T Object { get; private set; }

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			_pool.Free(Object);
		}
	}
}