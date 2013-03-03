using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using System.Runtime.InteropServices;

	/// <summary>
	///   Represents the target of a rendering operation.
	/// </summary>
	public sealed class RenderTarget : GraphicsObject
	{
		/// <summary>
		///   A value indicating whether this render target instance owns the native object and is responsible for
		///   destroying it when it is no longer used.
		/// </summary>
		private readonly bool _ownsNativeObject;

		/// <summary>
		///   The native render target instance.
		/// </summary>
		private readonly IntPtr _renderTarget;

		/// <summary>
		///   Initializes a new instance.
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
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="texture">The underlying texture for the render target.</param>
		internal RenderTarget(GraphicsDevice graphicsDevice, Texture2D texture)
			: base(graphicsDevice)
		{
			_renderTarget = NativeMethods.CreateRenderTarget(graphicsDevice.NativePtr, texture.NativePtr);
			_ownsNativeObject = true;
		}

		/// <summary>
		///   Clears the color buffers of the render target.
		/// </summary>
		/// <param name="color">The color the color buffer should be set to.</param>
		public void Clear(Color color)
		{
			Assert.NotDisposed(this);
			NativeMethods.ClearColor(_renderTarget, color);
		}

		/// <summary>
		///   Clears the depth stencil buffer of the render target.
		/// </summary>
		/// <param name="clearDepth">Indicates whether the depth buffer should be cleared.</param>
		/// <param name="clearStencil">Indicates whether the stencil buffer should be cleared.</param>
		/// <param name="depth">The value the depth buffer should be set to.</param>
		/// <param name="stencil">The value the stencil buffer should be set to.</param>
		public void Clear(bool clearDepth, bool clearStencil, float depth = 0.0f, byte stencil = 0)
		{
			Assert.NotDisposed(this);
			NativeMethods.ClearDepthStencil(_renderTarget, clearDepth, clearStencil, depth, stencil);
		}

		/// <summary>
		///   Binds the render target to the output merger state.
		/// </summary>
		public void Bind()
		{
			Assert.NotDisposed(this);
			NativeMethods.BindRenderTarget(_renderTarget);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			if (_ownsNativeObject)
				NativeMethods.DestroyRenderTarget(_renderTarget);
		}

		/// <summary>
		///   Provides access to the native render target functions.
		/// </summary>
#if !DEBUG
		[System.Security.SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCreateRenderTarget")]
			public static extern IntPtr CreateRenderTarget(IntPtr device, IntPtr texture);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDestroyRenderTarget")]
			public static extern void DestroyRenderTarget(IntPtr renderTarget);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgClearColor")]
			public static extern void ClearColor(IntPtr renderTarget, Color color);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgClearDepthStencil")]
			public static extern void ClearDepthStencil(IntPtr renderTarget, bool clearDepth, bool clearnStencil, float depth, byte stencil);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgBindRenderTarget")]
			public static extern void BindRenderTarget(IntPtr renderTarget);
		}
	}
}