namespace Pegasus.Platform.Memory
{
	using System;
	using System.Diagnostics;
	using Logging;

	/// <summary>
	///     An abstract base class for objects whose instances are pooled in order to reduce the pressure on the garbage
	///     collector. Pooled types should perform all their initialization in the OnReusing() method, which is called whenever
	///     the instance is reused. Similarly, all cleanup logic that must be run when an instance is returned to the pool
	///     should be done in the OnReturning method. There is one static object pool for all objects of the given type
	///     within the app domain. Creating and returning pooled objects is not thread-safe.
	/// </summary>
	/// <typeparam name="TObject">The concrete type of the pooled object.</typeparam>
	public abstract class OldPooledObject<TObject> : IDisposable
		where TObject : OldPooledObject<TObject>, new()
	{
		/// <summary>
		///     Gets a value indicating whether the instance is currently available, that is, waiting in the pool to be reused.
		/// </summary>
		public bool IsAvailable { get; private set; }

		/// <summary>
		///     The pool that manages the instances of type TObject.
		/// </summary>
		private static readonly OldObjectPool<TObject> Pool = new OldObjectPool<TObject>();

#if DEBUG
		/// <summary>
		///     A description for the instance in order to make debugging easier.
		/// </summary>
		private string _description;

		/// <summary>
		///     Checks whether the instance has been returned to the pool.
		/// </summary>
		~OldPooledObject()
		{
			if (!IsAvailable)
				Log.Error("A pooled object of type '{0}' was not returned to the pool.\nInstance description: '{1}'",
					GetType().Name, _description ?? "None");
		}
#endif

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		protected OldPooledObject()
		{
			Assert.That(typeof(TObject) == GetType(), "TObject must be set to the correct type instance.");
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
		///     Reuses a pooled instance.
		/// </summary>
		protected static TObject GetInstance()
		{
			var instance = Pool.Get();
			instance.IsAvailable = false;
			instance.OnReusing();

			return instance;
		}

		/// <summary>
		///     Invoked when the pooled instance is reused and should reset or reinitialize its state.
		/// </summary>
		protected virtual void OnReusing()
		{
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

			OnReturning();
			IsAvailable = true;
			Pool.Return(this as TObject);
		}
	}
}