namespace Pegasus.Platform.Memory
{
	using System;
	using System.Diagnostics;
	using Logging;

	/// <summary>
	///     An abstract base class for objects whose instances are pooled in order to reduce the pressure on the garbage
	///     collector. Pooled types should override the OnReturning method to perform all their cleanup logic that must be run when
	///     an instance is returned to the pool.
	/// </summary>
	public abstract class PooledObject : IPooledObject, IDisposable
	{
		/// <summary>
		///     Gets a value indicating whether the instance is currently available, that is, waiting in the pool to be reused.
		/// </summary>
		public bool IsAvailable { get; private set; }

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
			if (!IsAvailable)
				Log.Error("A pooled object of type '{0}' was not returned to the pool.\nInstance description: '{1}'",
					GetType().Name, _description ?? "None");
		}
#endif

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		protected PooledObject()
		{
			IsAvailable = true;
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
		void IPooledObject.AllocatedFrom(ObjectPool objectPool)
		{
			Assert.ArgumentNotNull(objectPool);

			_pool = objectPool;
			IsAvailable = false;
		}

		/// <summary>
		///     Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected virtual void OnReturning()
		{
		}

		/// <summary>
		///     Returns the instance to the pool.
		/// </summary>
		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			Assert.That(!IsAvailable, "The instance has already been returned.");
			Assert.NotNull(_pool, "Unknown object pool.");

			OnReturning();
			IsAvailable = true;
			_pool.Free(this);
		}
	}
}