namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Runtime.InteropServices;
	using System.Security;
	using Framework.UserInterface;
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
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="window">The window the swap chain should be bound to.</param>
		/// <param name="resolution">Indicates the swap chain's default resolution in full screen mode.</param>
		internal SwapChain(GraphicsDevice graphicsDevice, NativeWindow window, Size resolution)
			: base(graphicsDevice)
		{
			Assert.ArgumentNotNull(window);

			_swapChain = NativeMethods.CreateSwapChain(graphicsDevice.NativePtr, window.NativePtr,
				resolution.IntegralWidth, resolution.IntegralHeight);
			BackBuffer = new RenderTarget(graphicsDevice, NativeMethods.GetBackBuffer(_swapChain));

			BackBuffer.Bind();
		}

		/// <summary>
		///     Gets swap chain's back buffer.
		/// </summary>
		public RenderTarget BackBuffer { get; private set; }

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
		///     Provides access to the native swap chain functions.
		/// </summary>
		[SuppressUnmanagedCodeSecurity]
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCreateSwapChain")]
			public static extern IntPtr CreateSwapChain(IntPtr device, IntPtr window, int width, int height);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDestroySwapChain")]
			public static extern void DestroySwapChain(IntPtr swapChain);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgPresent")]
			public static extern void Present(IntPtr swapChain);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgGetBackBuffer")]
			public static extern IntPtr GetBackBuffer(IntPtr swapChain);
		}
	}
}