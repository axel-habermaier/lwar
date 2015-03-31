namespace Pegasus.Platform.Graphics
{
	using System;
	using Math;
	using Memory;
	using UserInterface;
	using Utilities;

	/// <summary>
	///     A swap chain provides a front buffer and a back buffer for a window that can be used as the target of a
	///     rendering operation by a graphics device.
	/// </summary>
	public sealed unsafe class SwapChain : GraphicsObject
	{
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

			NativeObject = DeviceInterface->InitializeSwapChain(window.NativePtr);

			BackBuffer = new RenderTarget(graphicsDevice, DeviceInterface->GetBackBuffer(NativeObject));
			Resize(window.Size);
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
			DeviceInterface->PresentSwapChain(NativeObject);
		}

		/// <summary>
		///     Resizes the swap chain to the given size.
		/// </summary>
		/// <param name="size">The new size of the swap chain.</param>
		internal void Resize(Size size)
		{
			Assert.NotDisposed(this);

			if (BackBuffer.Size == size || size.IntegralWidth == 0 || size.IntegralHeight == 0)
				return;

			DeviceInterface->ResizeSwapChain(NativeObject, size.IntegralWidth, size.IntegralHeight);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			DeviceState.Unset(ref DeviceState.RenderTarget, BackBuffer);

			DeviceInterface->FreeSwapChain(NativeObject);
			BackBuffer.SafeDispose();

			_window.SwapChain = null;
		}
	}
}