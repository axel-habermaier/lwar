namespace Pegasus.Platform.Memory
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	///     Provides access to a pointer to a pinned object.
	/// </summary>
	internal struct PinnedPointer : IDisposable
	{
		/// <summary>
		///     The handle of the pinned object.
		/// </summary>
		private GCHandle _handle;

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (_handle.IsAllocated)
				_handle.Free();
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		internal static PinnedPointer Create<T>(T obj)
		{
			return new PinnedPointer { _handle = GCHandle.Alloc(obj, GCHandleType.Pinned) };
		}

		/// <summary>
		///     Converts the pinned pointer to an IntPtr.
		/// </summary>
		/// <param name="pointer">The pinned pointer that should be converted.</param>
		public static implicit operator IntPtr(PinnedPointer pointer)
		{
			return pointer._handle.AddrOfPinnedObject();
		}

		/// <summary>
		///     Converts the pinned pointer to a void pointer.
		/// </summary>
		/// <param name="pointer">The pinned pointer that should be converted.</param>
		public static unsafe implicit operator void*(PinnedPointer pointer)
		{
			return pointer._handle.AddrOfPinnedObject().ToPointer();
		}
	}
}