using System;

namespace Pegasus.Framework.Platform
{
	using System.Diagnostics;
	using System.Runtime.InteropServices;

	/// <summary>
	///   Provides helper extension methods for native interop.
	/// </summary>
	internal static class InteropExtensions
	{
		/// <summary>
		///   Pins the given object in memory, gets a pointer to it and passes the pointer to the given action.
		///   The pointer is invalidated once the control flow leaves the UsePointer function.
		/// </summary>
		/// <typeparam name="T">The type of the data that should be pinned.</typeparam>
		/// <param name="obj">The data that should be pinned.</param>
		/// <param name="action">The action that should be performed on the pinned pointer.</param>
		[DebuggerHidden]
		internal static void UsePointer<T>(this T obj, Action<IntPtr> action)
		{
			Assert.ArgumentNotNull(action, () => action);

			var handle = GCHandle.Alloc(obj, GCHandleType.Pinned);
			try
			{
				action(handle.AddrOfPinnedObject());
			}
			finally
			{
				handle.Free();
			}
		}

		/// <summary>
		///   Pins the given object in memory, gets a pointer to it and passes the pointer to the given function.
		///   The pointer is invalidated once the control flow leaves the UsePointer function.
		/// </summary>
		/// <typeparam name="TObject">The type of the data that should be pinned.</typeparam>
		/// <typeparam name="TReturn">The return type of the given function.</typeparam>
		/// <param name="obj">The data that should be pinned.</param>
		/// <param name="action">The action that should be performed on the pinned pointer.</param>
		[DebuggerHidden]
		internal static TReturn UsePointer<TObject, TReturn>(this TObject obj, Func<IntPtr, TReturn> action)
		{
			Assert.ArgumentNotNull(action, () => action);

			var handle = GCHandle.Alloc(obj, GCHandleType.Pinned);
			try
			{
				return action(handle.AddrOfPinnedObject());
			}
			finally
			{
				handle.Free();
			}
		}

		/// <summary>
		///   Returns the size of the given instance in bytes.
		/// </summary>
		/// <typeparam name="T">The type of the object.</typeparam>
		/// <param name="obj">The object whose size should be returned.</param>
		internal static int Size<T>(this T obj)
			where T : struct
		{
			return Marshal.SizeOf(typeof(T));
		}

		/// <summary>
		///   Returns the size of the given array instance in bytes.
		/// </summary>
		/// <typeparam name="T">The type of the array elements.</typeparam>
		/// <param name="obj">The array whose size should be returned.</param>
		internal static int Size<T>(this T[] obj)
			where T : struct
		{
			Assert.ArgumentNotNull(obj, () => obj);
			return Marshal.SizeOf(typeof(T)) * obj.Length;
		}
	}
}