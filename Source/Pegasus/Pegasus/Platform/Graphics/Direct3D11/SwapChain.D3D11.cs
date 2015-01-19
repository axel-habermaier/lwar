namespace Pegasus.Platform.Graphics.Direct3D11
{
	using System;
	using Bindings;
	using Interface;
	using Math;
	using UserInterface;

	/// <summary>
	///     A swap chain provides a front buffer and a back buffer for a window that can be used as the target of a
	///     rendering operation by a graphics device.
	/// </summary>
	internal class SwapChainD3D11 : GraphicsObjectD3D11, ISwapChain
	{
		/// <summary>
		///     The format of the swap chain.
		/// </summary>
		private const DXGIFormat SwapChainFormat = DXGIFormat.R8G8B8A8_UNorm;

		/// <summary>
		///     The back buffer of the swap chain.
		/// </summary>
		private readonly RenderTargetD3D11 _backBuffer;

		/// <summary>
		///     The underlying DXGI swap chain.
		/// </summary>
		private DXGISwapChain _swapChain;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="window">The window the swap chain belongs to.</param>
		public unsafe SwapChainD3D11(GraphicsDeviceD3D11 graphicsDevice, NativeWindow window)
			: base(graphicsDevice)
		{
			var size = window.Size;
			var desc = new DXGISwapChainDescription
			{
				BufferCount = 2,
				Flags = 0,
				IsWindowed = true,
				ModeDescription =
				{
					Width = size.IntegralWidth,
					Height = size.IntegralHeight,
					RefreshRate =
					{
						Numerator = 0,
						Denominator = 0
					},
					Format = SwapChainFormat
				},
				Usage = DXGIUsage.RenderTargetOutput,
				OutputHandle = window.PlatformHandle,
				SampleDescription =
				{
					Count = 1,
					Quality = 0
				},
				SwapEffect = DXGISwapEffect.Discard,
			};

			DXGISwapChain swapChain;
			graphicsDevice.Factory.CreateSwapChain(Device, &desc, &swapChain);
			_swapChain = swapChain;
			graphicsDevice.Factory.MakeWindowAssociation(window.PlatformHandle, DXGIWindowAssociationFlags.IgnoreAll);

			_backBuffer = new RenderTargetD3D11(graphicsDevice);
		}

		/// <summary>
		///     Gets the back buffer of the swap chain.
		/// </summary>
		public IRenderTarget BackBuffer
		{
			get { return _backBuffer; }
		}

		/// <summary>
		///     Presents the back buffer to the screen.
		/// </summary>
		public void Present()
		{
			_swapChain.Present(0, 0);
		}

		/// <summary>
		///     Resizes the swap chain to the given size.
		/// </summary>
		/// <param name="size">The new size of the swap chain.</param>
		public void Resize(Size size)
		{
			if (BackBuffer.Size == size || size.IntegralWidth == 0 || size.IntegralHeight == 0)
				return;

			ReleaseBackBuffer();
			_swapChain.ResizeBuffers(2, size.IntegralWidth, size.IntegralHeight, SwapChainFormat, 0);
			BackBuffer.Size = size;
			InitializeBackBuffer();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_swapChain.Release();
		}

		/// <summary>
		///     Releases the back buffer.
		/// </summary>
		private unsafe void ReleaseBackBuffer()
		{
			if (!_backBuffer.ColorBuffers[0].IsInitialized)
				return;

			Context.OMSetRenderTargets(0, null, new D3D11DepthStencilView());
			_backBuffer.ColorBuffers[0].Release();
		}

		/// <summary>
		///     Initializes the back buffer.
		/// </summary>
		private unsafe void InitializeBackBuffer()
		{
			var textureId = new Guid("6f15aaf2-d208-4e89-9ab4-489535d34f9c");
			D3D11Resource texture;

			_swapChain.GetBuffer(0, &textureId, out texture).CheckSuccess("Failed to get back buffer from swap chain.");
			Device.CreateRenderTargetView(texture, null, out _backBuffer.ColorBuffers[0])
				  .CheckSuccess("Failed to initialize back buffer render target view.");

			texture.Release();
			_backBuffer.Bind();
		}
	}
}