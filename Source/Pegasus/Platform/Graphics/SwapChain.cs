namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Runtime.InteropServices;
	using System.Security;
	using Math;
	using Memory;

	/// <summary>
	///     A swap chain provides a front buffer and a back buffer for a window that can be used as the target of a
	///     rendering operation by a graphics device.
	/// </summary>
	public sealed class SwapChain : GraphicsObject
	{
		/// <summary>
		///     The native swap chain instance.
		/// </summary>
		private readonly IntPtr _swapChain;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="window">The window the swap chain should be bound to.</param>
		/// <param name="fullscreen">Indicates whether the swap chain should be set to full screen mode.</param>
		/// <param name="resolution">Indicates the swap chain's default resolution in full screen mode.</param>
		internal SwapChain(NativeWindow window, bool fullscreen, Size resolution)
		{
			Assert.ArgumentNotNull(window);

			_swapChain = NativeMethods.CreateSwapChain(GraphicsDevice.Current.NativePtr, window.NativePtr, fullscreen, resolution.Width, resolution.Height);
			BackBuffer = new RenderTarget(NativeMethods.GetBackBuffer(_swapChain));

			BackBuffer.Bind();
		}

		/// <summary>
		///     Gets swap chain's back buffer.
		/// </summary>
		public RenderTarget BackBuffer { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the swap chain is currently in full screen mode.
		/// </summary>
		public bool IsFullscreen
		{
			get
			{
				Assert.NotDisposed(this);
				return NativeMethods.IsFullscreen(_swapChain);
			}
		}

		/// <summary>
		///     Presents the back buffer to the screen.
		/// </summary>
		public void Present()
		{
			Assert.NotDisposed(this);
			NativeMethods.Present(_swapChain);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			BackBuffer.SafeDispose();
			NativeMethods.DestroySwapChain(_swapChain);
		}

		/// <summary>
		///     Switches to full screen mode with the given resolution. If the swap chain already is in full screen mode, the
		///     resolution is changed.
		/// </summary>
		/// <param name="resolution">The resolution that should be used.</param>
		public bool SwitchToFullscreen(Size resolution)
		{
			return NativeMethods.SwitchToFullscreen(_swapChain, resolution.Width, resolution.Height);
		}

		/// <summary>
		///     Switches to windowed mode if the swap chain is currently in full screen mode.
		/// </summary>
		public bool SwitchToWindowed()
		{
			return NativeMethods.SwitchToWindowed(_swapChain);
		}

		/// <summary>
		///     Provides access to the native swap chain functions.
		/// </summary>
#if !DEBUG
		[SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCreateSwapChain")]
			public static extern IntPtr CreateSwapChain(IntPtr device, IntPtr window, bool fullscreen, int width, int height);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDestroySwapChain")]
			public static extern void DestroySwapChain(IntPtr swapChain);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgPresent")]
			public static extern void Present(IntPtr swapChain);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgGetBackBuffer")]
			public static extern IntPtr GetBackBuffer(IntPtr swapChain);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSwapChainFullscreen")]
			public static extern bool SwitchToFullscreen(IntPtr swapChain, int width, int height);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSwapChainWindowed")]
			public static extern bool SwitchToWindowed(IntPtr swapChain);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSwapChainIsFullscreen")]
			public static extern bool IsFullscreen(IntPtr swapChain);
		}
	}
}