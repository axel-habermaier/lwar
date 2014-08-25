namespace Pegasus.Platform.Memory
{
	using System;
	using System.Diagnostics;
	using Logging;

	/// <summary>
	///     Base implementation for the IDisposable interface. In debug builds, throws an exception if the finalizer runs
	///     because of the object not having been disposed by calling the Dispose() method. In release builds, the finalizer
	///     is not executed and the object might silently leak unmanaged resources.
	/// </summary>
	public abstract class DisposableObject : IDisposable
	{
		/// <summary>
		///     Gets a value indicating whether the object has already been disposed.
		/// </summary>
		public bool IsDisposed { get; private set; }

		/// <summary>
		///     Gets a value indicating  whether the object is currently being disposed.
		/// </summary>
		protected bool IsDisposing { get; private set; }

#if DEBUG
		/// <summary>
		///     A description for the instance in order to make debugging easier.
		/// </summary>
		private string _description;

		/// <summary>
		///     Ensures that the instance has been disposed.
		/// </summary>
		~DisposableObject()
		{
			Log.Error("Finalizer runs for a disposable object of type '{0}'.\nInstance description: '{1}'",
				GetType().Name, _description ?? "None");
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
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			Assert.That(!IsDisposed, "The instance has already been disposed.");

			IsDisposing = true;
			OnDisposing();
			IsDisposing = false;
			IsDisposed = true;

#if DEBUG
			GC.SuppressFinalize(this);
#endif
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected abstract void OnDisposing();
	}
}