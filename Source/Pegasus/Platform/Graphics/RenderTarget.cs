namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Runtime.InteropServices;
	using Math;
	using Memory;

	/// <summary>
	///     Represents the target of a rendering operation.
	/// </summary>
	public sealed class RenderTarget : GraphicsObject
	{
		/// <summary>
		///     A value indicating whether this render target instance owns the native object and is responsible for
		///     destroying it when it is no longer used.
		/// </summary>
		private readonly bool _ownsNativeObject;

		/// <summary>
		///     The native render target instance.
		/// </summary>
		private readonly IntPtr _renderTarget;

		/// <summary>
		///     The depth stencil buffer that is bound to the render target.
		/// </summary>
		private readonly Texture2D _depthStencil;

		/// <summary>
		///     The color buffers that are bound to the render target.
		/// </summary>
		private readonly Texture[] _colorBuffers;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="renderTarget">The native render target instance.</param>
		internal RenderTarget(GraphicsDevice graphicsDevice, IntPtr renderTarget)
			: base(graphicsDevice)
		{
			_renderTarget = renderTarget;
			_ownsNativeObject = false;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="depthStencil">The depth stencil buffer that should be bound to the render target.</param>
		/// <param name="colorBuffers">The color buffers that should be bound to the render target.</param>
		public RenderTarget(GraphicsDevice graphicsDevice, Texture2D depthStencil, params Texture[] colorBuffers)
			: base(graphicsDevice)
		{
			IntPtr[] colorBuffersPtrs = null;
			var count = 0;
			if (colorBuffers != null)
			{
				colorBuffersPtrs = colorBuffers.Select(b => b.NativePtr).ToArray();
				count = colorBuffers.Length;
			}

			var depthStencilPtr = IntPtr.Zero;
			if (depthStencil != null)
				depthStencilPtr = depthStencil.NativePtr;

			_renderTarget = NativeMethods.CreateRenderTarget(graphicsDevice.NativePtr, colorBuffersPtrs, count, depthStencilPtr);
			_ownsNativeObject = true;
			_depthStencil = depthStencil;
			_colorBuffers = colorBuffers;
		}

		/// <summary>
		///     Gets the size of the render target.
		/// </summary>
		public Size Size
		{
			get
			{
				int width, height;
				NativeMethods.GetRenderTargetSize(_renderTarget, out width, out height);
				return new Size(width, height);
			}
		}

		/// <summary>
		///     Gets the width of the render target.
		/// </summary>
		public int Width
		{
			get { return Size.Width; }
		}

		/// <summary>
		///     Gets the height of the render target.
		/// </summary>
		public int Height
		{
			get { return Size.Height; }
		}

		/// <summary>
		///     Clears the color buffers of the render target.
		/// </summary>
		/// <param name="color">The color the color buffer should be set to.</param>
		internal void ClearColor(Color color)
		{
			Assert.NotDisposed(this);
			NativeMethods.ClearColor(_renderTarget, color);
		}

		/// <summary>
		///     Clears the depth stencil buffer of the render target.
		/// </summary>
		/// <param name="clearDepth">Indicates whether the depth buffer should be cleared.</param>
		/// <param name="clearStencil">Indicates whether the stencil buffer should be cleared.</param>
		/// <param name="depth">The value the depth buffer should be set to.</param>
		/// <param name="stencil">The value the stencil buffer should be set to.</param>
		internal void ClearDepthStencil(bool clearDepth, bool clearStencil, float depth = 1.0f, byte stencil = 0)
		{
			Assert.NotDisposed(this);
			NativeMethods.ClearDepthStencil(_renderTarget, clearDepth, clearStencil, depth, stencil);
		}

		/// <summary>
		///     Binds the render target to the output merger state.
		/// </summary>
		internal void Bind()
		{
			Assert.NotDisposed(this);
			NativeMethods.BindRenderTarget(_renderTarget);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			if (!_ownsNativeObject)
				return;

			NativeMethods.DestroyRenderTarget(_renderTarget);
			_depthStencil.SafeDispose();
			_colorBuffers.SafeDisposeAll();
		}

#if DEBUG
		/// <summary>
		///     Invoked after the name of the graphics object has changed. This method is only available in debug builds.
		/// </summary>
		protected override void OnRenamed()
		{
			if (_renderTarget != IntPtr.Zero)
				NativeMethods.SetName(_renderTarget, Name);
		}
#endif

		/// <summary>
		///     Provides access to the native render target functions.
		/// </summary>
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCreateRenderTarget")]
			public static extern IntPtr CreateRenderTarget(IntPtr device, IntPtr[] colorBuffers, int count, IntPtr depthStencil);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDestroyRenderTarget")]
			public static extern void DestroyRenderTarget(IntPtr renderTarget);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgGetRenderTargetSize")]
			public static extern void GetRenderTargetSize(IntPtr renderTarget, out int width, out int height);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgClearColor")]
			public static extern void ClearColor(IntPtr renderTarget, Color color);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgClearDepthStencil")]
			public static extern void ClearDepthStencil(IntPtr renderTarget, bool clearDepth, bool clearStencil, float depth,
														byte stencil);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgBindRenderTarget")]
			public static extern void BindRenderTarget(IntPtr renderTarget);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetRenderTargetName")]
			[Conditional("DEBUG")]
			public static extern void SetName(IntPtr renderTarget, string name);
		}
	}
}