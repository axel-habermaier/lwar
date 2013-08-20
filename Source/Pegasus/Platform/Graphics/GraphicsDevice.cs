using System;

namespace Pegasus.Platform.Graphics
{
	using System.Runtime.InteropServices;
	using Logging;
	using Math;
	using Memory;

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
		///   The current primitive type of the input assembler stage.
		/// </summary>
		private PrimitiveType _primitiveType;

		/// <summary>
		///   The current scissor rectangle of the rasterizer stage of the device.
		/// </summary>
		private Rectangle _scissor;

		/// <summary>
		///   The current viewport of the rasterizer stage of the device.
		/// </summary>
		private Rectangle _viewport;

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
			get
			{
				Assert.NotDisposed(this);
				return _device;
			}
		}

		/// <summary>
		///   Gets or sets the current viewport of the rasterizer stage of the device.
		/// </summary>
		internal Rectangle Viewport
		{
			get
			{
				Assert.NotDisposed(this);
				return _viewport;
			}
			set
			{
				Assert.NotDisposed(this);

				_viewport = value;
				NativeMethods.SetViewport(_device, value);
			}
		}

		/// <summary>
		///   Gets or sets the current scissor area of the rasterizer stage of the device.
		/// </summary>
		internal Rectangle ScissorArea
		{
			get
			{
				Assert.NotDisposed(this);
				return _scissor;
			}
			set
			{
				Assert.NotDisposed(this);

				_scissor = value;
				NativeMethods.SetScissorArea(_device, value);
			}
		}

		/// <summary>
		///   Gets or sets the current primitive type of the input assembler stage.
		/// </summary>
		internal PrimitiveType PrimitiveType
		{
			get
			{
				Assert.NotDisposed(this);
				return _primitiveType;
			}
			set
			{
				Assert.NotDisposed(this);

				_primitiveType = value;
				NativeMethods.SetPrimitiveType(_device, _primitiveType);
			}
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
		///   Draws primitiveCount-many primitives, starting at the given offset into the currently bound vertex buffers.
		/// </summary>
		/// <param name="primitiveCount">The number of primitives that should be drawn.</param>
		/// <param name="offset">The offset into the vertex buffers.</param>
		internal void Draw(int primitiveCount, int offset = 0)
		{
			Assert.NotDisposed(this);
			NativeMethods.Draw(_device, primitiveCount, offset);
		}

		/// <summary>
		///   Draws indexCount-many indices, starting at the given index offset into the currently bound index buffer, where the
		///   vertex offset is added to each index before accessing the currently bound vertex buffers.
		/// </summary>
		/// <param name="indexCount">The number of indices to draw.</param>
		/// <param name="indexOffset">The location of the first index read by the GPU from the index buffer.</param>
		/// <param name="vertexOffset">The value that should be added to each index before reading a vertex from the vertex buffer.</param>
		internal void DrawIndexed(int indexCount, int indexOffset = 0, int vertexOffset = 0)
		{
			Assert.NotDisposed(this);
			NativeMethods.DrawIndexed(_device, indexCount, indexOffset, vertexOffset);
		}

		/// <summary>
		///   Gets the statistics and resets all values to zero. This method should be called once per frame.
		/// </summary>
		internal GraphicsDeviceStatistics GetStatistics()
		{
			GraphicsDeviceStatistics statistics;
			NativeMethods.GetStatistics(_device, out statistics);
			return statistics;
		}

		/// <summary>
		///   Provides access to the native graphics device functions.
		/// </summary>
#if !DEBUG
		[SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCreateGraphicsDevice")]
			public static extern IntPtr CreateGraphicsDevice();

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDestroyGraphicsDevice")]
			public static extern void DestroyGraphicsDevice(IntPtr device);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetViewport")]
			public static extern void SetViewport(IntPtr device, Rectangle viewport);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetScissorArea")]
			public static extern void SetScissorArea(IntPtr device, Rectangle scissorArea);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetPrimitiveType")]
			public static extern void SetPrimitiveType(IntPtr device, PrimitiveType primitiveType);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDraw")]
			public static extern void Draw(IntPtr device, int primitiveCount, int offset);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDrawIndexed")]
			public static extern void DrawIndexed(IntPtr device, int indexCount, int indexOffset, int vertexOffset);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgGetStatistics")]
			public static extern void GetStatistics(IntPtr device, out GraphicsDeviceStatistics statistics);
		}
	}
}