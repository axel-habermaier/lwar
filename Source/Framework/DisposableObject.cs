using System;

namespace Pegasus.Framework
{
	using System.Diagnostics;
	using Platform;

	/// <summary>
	///   Base implementation for the IDisposable interface. In debug builds, throws an exception if the finalizer runs
	///   because of the object not having been disposed by calling the Dispose() method. In release builds, the finalizer
	///   is not executed and the object might silently leak unmanaged resources.
	/// </summary>
	public abstract class DisposableObject : IDisposable
	{
		/// <summary>
		///   Indicates whether the object has already been disposed.
		/// </summary>
		public bool IsDisposed { get; private set; }

		/// <summary>
		///   Indicates whether the object is currently being disposed.
		/// </summary>
		protected bool IsDisposing { get; private set; }

#if DEBUG
		/// <summary>
		///   The finalizer ensures that all unmanaged resources are freed.
		/// </summary>
		~DisposableObject()
		{
			Log.Die("Finalizer runs for a disposable object of type '{0}'.", GetType().Name);
		}
#endif

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		///   Throws an ObjectDisposedException if the object has already been disposed.
		/// </summary>
		[DebuggerHidden]
		public void Dispose()
		{
			Assert.That(!IsDisposed, "Dispose() can only be invoked once per object.");

			IsDisposing = true;
			OnDisposing();
			IsDisposing = false;
			IsDisposed = true;

#if DEBUG
			GC.SuppressFinalize(this);
#endif
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected abstract void OnDisposing();
	}
}