using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using System.Runtime.InteropServices;

	/// <summary>
	///   Base class for graphics buffers.
	/// </summary>
	public abstract class Buffer : GraphicsObject
	{
		/// <summary>
		///   The native buffer instance.
		/// </summary>
		private readonly IntPtr _buffer;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="type">The type of the buffer.</param>
		/// <param name="usage">A value describing the usage pattern of the buffer.</param>
		/// <param name="data">The data that should be copied into the constant buffer.</param>
		/// <param name="size">The size of the constant buffer.</param>
		protected Buffer(GraphicsDevice graphicsDevice, BufferType type, ResourceUsage usage, IntPtr data, int size)
			: base(graphicsDevice)
		{
			Assert.ArgumentSatisfies(size > 0, () => size, "A buffer must have a size greater than 0.");
			_buffer = NativeMethods.CreateBuffer(graphicsDevice.NativePtr, type, usage, data, size);
		}

		/// <summary>
		///   Gets the native buffer instance.
		/// </summary>
		internal IntPtr NativePtr
		{
			get { return _buffer; }
		}

		/// <summary>
		///   Maps the buffer and returns a pointer that the CPU can access. The operations that are allowed on the
		///   returned pointer depend on the given map mode.
		/// </summary>
		/// <param name="mapMode">Indicates which CPU operations are allowed on the buffer memory.</param>
		/// <returns>Returns a pointer to the buffer memory.</returns>
		public IntPtr Map(MapMode mapMode)
		{
			Assert.NotDisposed(this);
			return NativeMethods.MapBuffer(_buffer, mapMode);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			NativeMethods.DestroyBuffer(_buffer);
		}

		/// <summary>
		///   Unmaps the buffer.
		/// </summary>
		public void Unmap()
		{
			Assert.NotDisposed(this);
			NativeMethods.UnmapBuffer(_buffer);
		}

		/// <summary>
		///   Copies the given data to the buffer, overwriting all previous data.
		/// </summary>
		/// <param name="data">The data that should be copied.</param>
		/// <param name="size">The size of the data that should be copied in bytes.</param>
		protected void CopyData(IntPtr data, int size)
		{
			Assert.ArgumentNotNull(data, () => data);
			Assert.ArgumentSatisfies(size >= 0, () => size, "Invalid size.");
			Assert.NotDisposed(this);

			var gpuData = Map(MapMode.WriteDiscard);
			Interop.Copy(gpuData, data, size);
			Unmap();
		}

		/// <summary>
		///   Binds the buffer to the given slot.
		/// </summary>
		/// <param name="slot">The slot the constant buffer should be bound to.</param>
		protected void BindBuffer(int slot)
		{
			NativeMethods.BindConstantBuffer(_buffer, slot);
		}

		/// <summary>
		///   Provides access to the native buffer functions.
		/// </summary>
#if !DEBUG
		[System.Security.SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCreateBuffer")]
			public static extern IntPtr CreateBuffer(IntPtr device, BufferType type, ResourceUsage usage, IntPtr data, int size);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDestroyBuffer")]
			public static extern void DestroyBuffer(IntPtr buffer);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgMapBuffer")]
			public static extern IntPtr MapBuffer(IntPtr buffer, MapMode mode);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgUnmapBuffer")]
			public static extern void UnmapBuffer(IntPtr buffer);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgBindConstantBuffer")]
			public static extern void BindConstantBuffer(IntPtr buffer, int slot);
		}
	}
}