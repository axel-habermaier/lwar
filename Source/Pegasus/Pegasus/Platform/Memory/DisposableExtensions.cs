namespace Pegasus.Platform.Memory
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using UserInterface;

	/// <summary>
	///     Provides extension methods for classes implementing the IDisposable interface.
	/// </summary>
	public static class DisposableExtensions
	{
		/// <summary>
		///     Disposes all objects contained in the list if the list is not null and clears the list.
		/// </summary>
		/// <param name="list">The list that should be disposed.</param>
		[DebuggerHidden]
		public static void SafeDisposeAll<T>(this List<T> list)
			where T : IDisposable
		{
			if (list == null)
				return;

			foreach (var obj in list)
				obj.SafeDispose();

			list.Clear();
		}

		/// <summary>
		///     Disposes all objects contained in the queue if the queue is not null and clears the queue.
		/// </summary>
		/// <param name="queue">The queue that should be disposed.</param>
		[DebuggerHidden]
		public static void SafeDisposeAll<T>(this Queue<T> queue)
			where T : IDisposable
		{
			if (queue == null)
				return;

			foreach (var obj in queue)
				obj.SafeDispose();

			queue.Clear();
		}

		/// <summary>
		///     Disposes all objects contained in the collection if the collection is not null and clears the collection.
		/// </summary>
		/// <param name="collection">The collection that should be disposed.</param>
		[DebuggerHidden]
		public static void SafeDisposeAll<T>(this ObservableCollection<T> collection)
			where T : IDisposable
		{
			if (collection == null)
				return;

			foreach (var obj in collection)
				obj.SafeDispose();

			collection.Clear();
		}

		/// <summary>
		///     Disposes all objects contained in the array if the array is not null.
		/// </summary>
		/// <param name="array">The array that should be disposed.</param>
		[DebuggerHidden]
		public static void SafeDisposeAll<T>(this T[] array)
			where T : IDisposable
		{
			if (array == null)
				return;

			foreach (var obj in array)
				obj.SafeDispose();
		}

		/// <summary>
		///     Disposes the object if it is not null.
		/// </summary>
		/// <param name="obj">The object that should be disposed.</param>
		[DebuggerHidden]
		public static void SafeDispose(this IDisposable obj)
		{
			var disposableObject = obj as DisposableObject;
			if (disposableObject != null && disposableObject.IsDisposed)
				return;

			if (obj != null)
				obj.Dispose();
		}

		/// <summary>
		///     Disposes the object if it is not null and has not yet been disposed.
		/// </summary>
		/// <param name="obj">The object that should be disposed.</param>
		[DebuggerHidden]
		public static void SafeDispose(this DisposableObject obj)
		{
			if (obj != null && !obj.IsDisposed)
				((IDisposable)obj).Dispose();
		}

		/// <summary>
		///     Returns the pooled object to the pool the object if it is not null and has not yet been returned.
		/// </summary>
		/// <param name="obj">The object that should be returned.</param>
		[DebuggerHidden]
		public static void SafeDispose(this PooledObject obj)
		{
			if (obj != null)
				((IDisposable)obj).Dispose();
		}
	}
}