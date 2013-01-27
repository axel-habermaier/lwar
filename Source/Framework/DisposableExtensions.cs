using System;

namespace Pegasus.Framework
{
	using System.Diagnostics;

	/// <summary>
	///   Provides extension methods for classes implementing the IDisposable interface.
	/// </summary>
	public static class DisposableExtensions
	{
		/// <summary>
		///   Disposes the object if it is not null.
		/// </summary>
		/// <param name="obj">The object that should be disposed.</param>
		[DebuggerHidden]
		public static void SafeDispose(this IDisposable obj)
		{
			if (obj != null)
				obj.Dispose();
		}

		/// <summary>
		///   Disposes the object if it is not null and has not yet been disposed.
		/// </summary>
		/// <param name="obj">The object that should be disposed.</param>
		[DebuggerHidden]
		public static void SafeDispose(this DisposableObject obj)
		{
			if (obj != null && !obj.IsDisposed)
				obj.Dispose();
		}

		/// <summary>
		///   Returns the pooled object to the pool the object if it is not null and has not yet been returned.
		/// </summary>
		/// <typeparam name="T">The type of the pooled object.</typeparam>
		/// <param name="obj">The object that should be returned.</param>
		[DebuggerHidden]
		public static void SafeDispose<T>(this PooledObject<T> obj)
			where T : PooledObject<T>, new()
		{
			if (obj != null && !obj.IsAvailable)
				obj.Dispose();
		}
	}
}