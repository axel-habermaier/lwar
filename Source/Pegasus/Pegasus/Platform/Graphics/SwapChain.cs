namespace Pegasus.Platform.Graphics
{
	using System;
	using Interface;
	using Math;
	using Memory;
	using UserInterface;
	using Utilities;

	/// <summary>
	///     A swap chain provides a front buffer and a back buffer for a window that can be used as the target of a
	///     rendering operation by a graphics device.
	/// </summary>
	public sealed class SwapChain : GraphicsObject
	{
		/// <summary>
		///     The underlying swap chain object.
		/// </summary>
		private readonly ISwapChain _swapChain;

		/// <summary>
		///     The window the swap chain belongs to.
		/// </summary>
		private readonly NativeWindow _window;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="window">The window the swap chain should be bound to.</param>
		internal SwapChain(GraphicsDevice graphicsDevice, NativeWindow window)
			: base(graphicsDevice)
		{
			Assert.ArgumentNotNull(window);
			Assert.ArgumentSatisfies(window.SwapChain == null, "A swap chain has already been allocated for the given window.");

			_window = window;
			_window.SwapChain = this;

			_swapChain = graphicsDevice.CreateSwapChain(window);

			BackBuffer = new RenderTarget(graphicsDevice, _swapChain.BackBuffer);
			Resize(window.Size);

			BackBuffer.Bind();
			BackBuffer.SetName("BackBuffer");
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
			_swapChain.Present();
		}

		/// <summary>
		///     Resizes the swap chain to the given size.
		/// </summary>
		/// <param name="size">The new size of the swap chain.</param>
		public void Resize(Size size)
		{
			Assert.NotDisposed(this);

			if (BackBuffer.Size == size || size.IntegralWidth == 0 || size.IntegralHeight == 0)
				return;

			_swapChain.Resize(size);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			DeviceState.Unset(ref GraphicsDevice.State.RenderTarget, BackBuffer);

			_swapChain.SafeDispose();
			BackBuffer.SafeDispose();

			_window.SwapChain = null;
		}
	}
}