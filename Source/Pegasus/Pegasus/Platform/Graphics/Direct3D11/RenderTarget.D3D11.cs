namespace Pegasus.Platform.Graphics.Direct3D11
{
	using System;
	using Bindings;
	using Interface;
	using Math;
	using Rendering;

	/// <summary>
	///     Represents an Direct3D11-based target of a rendering operation.
	/// </summary>
	internal class RenderTargetD3D11 : GraphicsObjectD3D11, IRenderTarget
	{
		/// <summary>
		///     The underlying Direct3D11 depth stencil view.
		/// </summary>
		private D3D11DepthStencilView _depthStencil;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		public RenderTargetD3D11(GraphicsDeviceD3D11 graphicsDevice)
			: base(graphicsDevice)
		{
			IsBackBuffer = true;
			ColorBuffers = new D3D11RenderTargetView[1];
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="depthStencil">The depth stencil buffer that should be bound to the render target.</param>
		/// <param name="colorBuffers">The color buffers that should be bound to the render target.</param>
		public unsafe RenderTargetD3D11(GraphicsDeviceD3D11 graphicsDevice, Texture2D depthStencil, Texture2D[] colorBuffers)
			: base(graphicsDevice)
		{
			if (depthStencil != null)
			{
				Device.CreateDepthStencilView(((TextureD3D11)depthStencil.TextureObject).Texture, null, out _depthStencil)
					  .CheckSuccess("Failed to create depth stencil view.");
			}

			if (colorBuffers == null || colorBuffers.Length == 0)
				return;

			ColorBuffers = new D3D11RenderTargetView[colorBuffers.Length];
			for (var i = 0; i < colorBuffers.Length; ++i)
			{
				Device.CreateRenderTargetView(((TextureD3D11)colorBuffers[i].TextureObject).Texture, null, out ColorBuffers[i])
					  .CheckSuccess("Failed to create depth stencil view.");
			}
		}

		/// <summary>
		///     Gets the underlying Direct3D11 color buffers.
		/// </summary>
		internal D3D11RenderTargetView[] ColorBuffers { get; private set; }

		/// <summary>
		///     Gets or sets the size of the render target.
		/// </summary>
		public Size Size { get; set; }

		/// <summary>
		///     Gets a value indicating whether the render target is the back buffer of a swap chain.
		/// </summary>
		public bool IsBackBuffer { get; private set; }

		/// <summary>
		///     Clears the color buffers of the render target.
		/// </summary>
		/// <param name="color">The color the color buffer should be set to.</param>
		public unsafe void ClearColor(Color color)
		{
			var c = stackalloc float[4];
			color.ToFloatArray(c);

			for (var i = 0; ColorBuffers != null && i < ColorBuffers.Length; ++i)
				Context.ClearRenderTargetView(ColorBuffers[i], c);
		}

		/// <summary>
		///     Clears the depth stencil buffer of the render target.
		/// </summary>
		/// <param name="clearDepth">Indicates whether the depth buffer should be cleared.</param>
		/// <param name="clearStencil">Indicates whether the stencil buffer should be cleared.</param>
		/// <param name="depth">The value the depth buffer should be set to.</param>
		/// <param name="stencil">The value the stencil buffer should be set to.</param>
		public void ClearDepthStencil(bool clearDepth, bool clearStencil, float depth, byte stencil)
		{
			var flags = D3D11DepthStencilClearFlags.None;
			if (clearDepth)
				flags |= D3D11DepthStencilClearFlags.Depth;
			if (clearStencil)
				flags |= D3D11DepthStencilClearFlags.Stencil;

			Context.ClearDepthStencilView(_depthStencil, flags, depth, stencil);
		}

		/// <summary>
		///     Binds the render target to the output merger state.
		/// </summary>
		public unsafe void Bind()
		{
			if (ColorBuffers == null)
				Context.OMSetRenderTargets(0, null, _depthStencil);
			else
			{
				fixed (D3D11RenderTargetView* views = ColorBuffers)
					Context.OMSetRenderTargets(ColorBuffers.Length, views, _depthStencil);
			}
		}

		/// <summary>
		///     Sets the debug name of the render target.
		/// </summary>
		/// <param name="name">The debug name of the render target.</param>
		public void SetName(string name)
		{
			if (_depthStencil.IsInitialized)
				_depthStencil.SetDebugName(String.Format("Depth Stencil Buffer of '{0}'", name));

			for (var i = 0; ColorBuffers != null && i < ColorBuffers.Length; ++i)
				ColorBuffers[i].SetDebugName(String.Format("Color Buffer {0} of '{1}'", i, name));
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			for (var i = 0; ColorBuffers != null && i < ColorBuffers.Length; ++i)
				ColorBuffers[i].Release();

			_depthStencil.Release();
		}
	}
}