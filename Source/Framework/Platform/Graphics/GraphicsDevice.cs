using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using System.Runtime.InteropServices;
	using Math;

	/// <summary>
	///   Represents the graphics device.
	/// </summary>
	public sealed class GraphicsDevice : DisposableObject
	{
		/// <summary>
		///   The native graphics device instance.
		/// </summary>
		private readonly IntPtr _device;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		internal GraphicsDevice()
		{
			Log.Info("Initializing graphics device...");
			_device = NativeMethods.CreateGraphicsDevice();

			RasterizerState.InitializeDefaultInstances(this);
			SamplerState.InitializeDefaultInstances(this);
			DepthStencilState.InitializeDefaultInstances(this);
			BlendState.InitializeDefaultInstances(this);
			Texture2D.InitializeDefaultInstances(this);
		}

		/// <summary>
		///   Gets the native graphics device instance.
		/// </summary>
		internal IntPtr NativePtr
		{
			get { return _device; }
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			RasterizerState.DisposeDefaultInstances();
			SamplerState.DisposeDefaultInstances();
			DepthStencilState.DisposeDefaultInstances();
			BlendState.DisposeDefaultInstances();
			Texture2D.DisposeDefaultInstances();

			NativeMethods.DestroyGraphicsDevice(_device);
		}

		/// <summary>
		///   Draws primitiveCount-many primitives of the given type, starting at the given offset
		///   into the currently bound vertex buffers.
		/// </summary>
		/// <param name="primitiveCount">The number of primitives that should be drawn.</param>
		/// <param name="offset">The offset into the vertex buffers.</param>
		public void Draw(int primitiveCount, int offset)
		{
			Assert.NotDisposed(this);
			NativeMethods.Draw(_device, primitiveCount, offset);
		}

		/// <summary>
		///   Draws indexed geometry.
		/// </summary>
		/// <param name="indexCount">The number of indices to draw.</param>
		/// <param name="indexOffset">The location of the first index read by the GPU from the index buffer.</param>
		/// <param name="vertexOffset">A value added to each index before reading a vertex from the vertex buffer.</param>
		public void DrawIndexed(int indexCount, int indexOffset = 0, int vertexOffset = 0)
		{
			Assert.NotDisposed(this);
			NativeMethods.DrawIndexed(_device, indexCount, indexOffset, vertexOffset);
		}

		/// <summary>
		///   Binds the given viewport to the rasterizer stage of the device. The given viewport is only
		///   valid for the currently bound render target; once another render target is bound, the viewport
		///   is reset.
		/// </summary>
		/// <param name="viewport">The viewport that should be set.</param>
		public void SetViewport(Rectangle viewport)
		{
			Assert.NotDisposed(this);
			NativeMethods.SetViewport(_device, viewport.Left, viewport.Top, viewport.Width, viewport.Height);
		}

		/// <summary>
		///   Sets the primitive type of the input assembler stage.
		/// </summary>
		/// <param name="primitiveType">The primitive type that should be set.</param>
		public void SetPrimitiveType(PrimitiveType primitiveType)
		{
			Assert.NotDisposed(this);
			NativeMethods.SetPrimitiveType(_device, primitiveType);
		}

		/// <summary>
		///   Binds the scissor rectangle to the rasterizer stage of the device. The given rectangle is only
		///   valid for the currently bound render target; once another render target is bound, the scissor
		///   rectangle is reset.
		/// </summary>
		/// <param name="rectangle">The scissor rectangle.</param>
		public void SetScissorRectangle(Rectangle rectangle)
		{
			Assert.NotDisposed(this);
			NativeMethods.SetScissorRect(_device, rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
		}

		/// <summary>
		///   Provides access to the native graphics device functions.
		/// </summary>
#if !DEBUG
		[System.Security.SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCreateGraphicsDevice")]
			public static extern IntPtr CreateGraphicsDevice();

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDestroyGraphicsDevice")]
			public static extern void DestroyGraphicsDevice(IntPtr device);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetViewport")]
			public static extern void SetViewport(IntPtr device, int left, int top, int width, int height);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetScissorRect")]
			public static extern void SetScissorRect(IntPtr device, int left, int top, int width, int height);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetPrimitiveType")]
			public static extern void SetPrimitiveType(IntPtr device, PrimitiveType primitiveType);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDraw")]
			public static extern void Draw(IntPtr device, int primitiveCount, int offset);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDrawIndexed")]
			public static extern void DrawIndexed(IntPtr device, int indexCount, int indexOffset, int vertexOffset);
		}
	}
}