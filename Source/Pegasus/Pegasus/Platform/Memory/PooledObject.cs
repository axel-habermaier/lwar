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
		///     Gets a value indicating whether the instance is currently in use, i.e., not pooled.
		/// </summary>
		public bool InUse { get; private set; }

		/// <summary>
		///     The pool the instance should be returned to.
		/// </summary>
		private ObjectPool _pool;

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
			InUse = true;
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

		/// <summary>
		///     Invoked when an owner of the pooled object release its ownership. Returns true to indicate that
		///     the object should be returned to the pool.
		/// </summary>
		protected abstract bool OnOwnershipReleased();

		/// <summary>
		///     Returns the instance to the pool.
		/// </summary>
		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			Assert.That(InUse, "The instance has already been returned.");
			Assert.NotNull(_pool, "Unknown object pool.");

			if (!OnOwnershipReleased())
				return;

			OnReturning();
			InUse = false;
			_pool.Free(this);
		}
	}
}