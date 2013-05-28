using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using System.Runtime.InteropServices;
	using System.Security;
	using Math;
	using Memory;

	/// <summary>
	///   A swap chain provides a front buffer and a back buffer for a window that can be used as the target of a
	///   rendering operation by a graphics device.
	/// </summary>
	public sealed class SwapChain : GraphicsObject
	{
		/// <summary>
		///   The native swap chain instance.
		/// </summary>
		private readonly IntPtr _swapChain;

		/// <summary>
		///   The window the swap chain is bound to.
		/// </summary>
		private readonly Window _window;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="window">The window the swap chain should be bound to.</param>
		internal SwapChain(GraphicsDevice graphicsDevice, Window window)
			: base(graphicsDevice)
		{
			_window = window;
			_window.Resized += ResizeBackBuffer;

			_swapChain = NativeMethods.CreateSwapChain(graphicsDevice.NativePtr, window.NativePtr);
			BackBuffer = new RenderTarget(graphicsDevice, NativeMethods.GetBackBuffer(_swapChain));

			BackBuffer.Bind();
		}

		/// <summary>
		///   Gets swap chain's back buffer.
		/// </summary>
		public RenderTarget BackBuffer { get; private set; }

		/// <summary>
		///   Presents the back buffer to the screen.
		/// </summary>
		public void Present()
		{
			Assert.NotDisposed(this);
			NativeMethods.Present(_swapChain);
		}

		/// <summary>
		///   Resizes the back buffer to the given size.
		/// </summary>
		/// <param name="size">The new size.</param>
		private void ResizeBackBuffer(Size size)
		{
			Assert.NotDisposed(this);
			NativeMethods.ResizeSwapChain(_swapChain, size.Width, size.Height);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_window.Resized -= ResizeBackBuffer;
			BackBuffer.SafeDispose();

			NativeMethods.DestroySwapChain(_swapChain);
		}

		/// <summary>
		///   Reinitializes the swap chain, taking the current resolution and video mode into account.
		/// </summary>
		/// <param name="width">The width of the swap chain's back buffer.</param>
		/// <param name="height">The width of the swap chain's back buffer.</param>
		/// <param name="fullscreen">Indicates whether the swap chain should be set to fullscreen mode.</param>
		public bool UpdateState(int width, int height, bool fullscreen)
		{
			return NativeMethods.UpdateState(_swapChain, width, height, fullscreen);
		}

		/// <summary>
		///   Provides access to the native swap chain functions.
		/// </summary>
#if !DEBUG
		[SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCreateSwapChain")]
			public static extern IntPtr CreateSwapChain(IntPtr device, IntPtr window);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDestroySwapChain")]
			public static extern void DestroySwapChain(IntPtr swapChain);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgPresent")]
			public static extern void Present(IntPtr swapChain);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgGetBackBuffer")]
			public static extern IntPtr GetBackBuffer(IntPtr swapChain);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgResizeSwapChain")]
			public static extern void ResizeSwapChain(IntPtr swapChain, int width, int height);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgUpdateSwapChainState")]
			public static extern bool UpdateState(IntPtr swapChain, int width, int height, bool fullscreen);
		}
	}
}