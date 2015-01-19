namespace Pegasus.Platform.Graphics.OpenGL3
{
	using System;
	using System.Runtime.InteropServices;
	using Bindings;
	using Interface;
	using Math;
	using SDL2;
	using UserInterface;

	/// <summary>
	///     A swap chain provides a front buffer and a back buffer for a window that can be used as the target of a
	///     rendering operation by a graphics device.
	/// </summary>
	internal class SwapChainGL3 : GraphicsObjectGL3, ISwapChain
	{
		/// <summary>
		///     The native OpenGL context the swap chain belongs to.
		/// </summary>
		private readonly GLContext _context;

		/// <summary>
		///     The window the swap chain belongs to.
		/// </summary>
		private readonly NativeWindow _window;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="window">The window the swap chain belongs to.</param>
		public SwapChainGL3(GraphicsDeviceGL3 graphicsDevice, NativeWindow window)
			: base(graphicsDevice)
		{
			_window = window;
			_context = GraphicsDevice.Context;

			BackBuffer = new RenderTargetGL3(graphicsDevice, this);
			MakeCurrent();
		}

		/// <summary>
		///     Gets the back buffer of the swap chain.
		/// </summary>
		public IRenderTarget BackBuffer { get; private set; }

		/// <summary>
		///     Presents the back buffer to the screen.
		/// </summary>
		public void Present()
		{
			MakeCurrent();
			SDL_GL_SwapWindow(_window.NativePtr);
		}

		/// <summary>
		///     Resizes the swap chain to the given size.
		/// </summary>
		/// <param name="size">The new size of the swap chain.</param>
		public void Resize(Size size)
		{
			BackBuffer.Size = size;
		}

		/// <summary>
		///     Ensures that the swap chain's OpenGL context is the current one on the calling thread.
		/// </summary>
		internal void MakeCurrent()
		{
			_context.MakeCurrent(_window);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_context.MakeCurrent();
		}

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern void SDL_GL_SwapWindow(IntPtr window);
	}
}