namespace Pegasus.Platform.Memory
{
	using System;
	using System.Diagnostics;
	using Logging;
	using Utilities;

	/// <summary>
	///     An abstract base class for objects whose instances are pooled in order to reduce the pressure on the garbage
	///     collector. Pooled types should override the OnReturning method to perform all their cleanup logic that must be run when
	///     an instance is returned to the pool.
	/// </summary>
	public abstract class PooledObject : IPooledObject
	{
		/// <summary>
		///     The pool the instance should be returned to.
		/// </summary>
		private ObjectPool _pool;

		/// <summary>
		///     The number of times Dispose must be called before the object is returned to the pool.
		/// </summary>
		private int _referenceCount;

		/// <summary>
		///     Gets a value indicating whether the instance is currently in use, i.e., not pooled.
		/// </summary>
		public bool InUse
		{
			get { return _referenceCount > 0; }
		}

		/// <summary>
		///     Returns the instance to the pool.
		/// </summary>
		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			Assert.That(InUse, "The instance has already been returned.");
			Assert.NotNull(_pool, "Unknown object pool.");

			// We have to ensure that while OnReturning is executed, the object is still considered to be in-use
			if (_referenceCount - 1 <= 0)
			{
				OnReturning();
				_pool.Free(this);
			}

			--_referenceCount;
		}

		/// <summary>
		///     Allows the caller to acquire shared ownership of the object. The object will not be returned to the pool before the
		///     caller called its Dispose method.
		/// </summary>
		/// <remarks>Unless, of course, some malicious caller invokes Dispose multiple times...</remarks>
		public IDisposable AcquireOwnership()
		{
			Assert.NotPooled(this);

			++_referenceCount;
			return this;
		}

		/// <summary>
		///     In debug builds, sets a description for the instance in order to make debugging easier.
		/// </summary>
		/// <param name="description">The description of the instance.</param>
		/// <param name="arguments">The arguments that should be copied into the description.</param>
		[Conditional("DEBUG"), StringFormatMethod("description")]
		public void SetDescription(string description, params object[] arguments)
		{
			Assert.ArgumentNotNullOrWhitespace(description);

#if DEBUG
			_description = String.Format(description, arguments);
#endif
		}

		/// <summary>
		///     Marks the instance as allocated from the given pool.
		/// </summary>
		/// <param name="objectPool">The object pool the instance is allocated from.</param>
		internal void AllocatedFrom(ObjectPool objectPool)
		{
			Assert.ArgumentNotNull(objectPool);

			_pool = objectPool;
			_referenceCount = 1;
		}

		/// <summary>
		///     Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected virtual void OnReturning()
		{
		}

		/// <summary>
		///     Marks the instance as freed, meaning that it is no longer pooled and will be garbage collected.
		/// </summary>
		internal void Free()
		{
			OnDisposing();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected virtual void OnDisposing()
		{
		}

#if DEBUG
		/// <summary>
		///     A description for the instance in order to make debugging easier.
		/// </summary>
		private string _description;

		/// <summary>
		///     Checks whether the instance has been returned to the pool.
		/// </summary>
		~PooledObject()
		{
			if (InUse)
				Log.Error("A pooled object of type '{0}' was not returned to the pool.\nInstance description: '{1}'",
					GetType().FullName, _description ?? "None");
		}
#endif
	}
}