namespace Pegasus.Platform.Graphics
{
	using System;
	using Interface;
	using Math;
	using Memory;
	using Rendering;
	using Utilities;

	/// <summary>
	///     Represents the target of a rendering operation.
	/// </summary>
	public sealed class RenderTarget : GraphicsObject
	{
		/// <summary>
		///     The color buffers that are bound to the render target.
		/// </summary>
		private readonly Texture2D[] _colorBuffers;

		/// <summary>
		///     The depth stencil buffer that is bound to the render target.
		/// </summary>
		private readonly Texture2D _depthStencil;

		/// <summary>
		///     The underlying render target object.
		/// </summary>
		private readonly IRenderTarget _renderTarget;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="renderTarget">The underlying render target object.</param>
		internal RenderTarget(GraphicsDevice graphicsDevice, IRenderTarget renderTarget)
			: base(graphicsDevice)
		{
			Assert.ArgumentNotNull(renderTarget);
			_renderTarget = renderTarget;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="depthStencil">The depth stencil buffer, if any, that should be bound to the render target.</param>
		/// <param name="colorBuffers">The color buffers that should be bound to the render target.</param>
		public RenderTarget(GraphicsDevice graphicsDevice, Texture2D depthStencil, params Texture2D[] colorBuffers)
			: base(graphicsDevice)
		{
			_colorBuffers = colorBuffers;
			_depthStencil = depthStencil;

			Assert.That(colorBuffers == null || colorBuffers.Length < GraphicsDevice.MaxColorBuffers, "Too many color buffers.");
			Assert.That((colorBuffers != null && colorBuffers.Length != 0) || depthStencil != null,
				"Color buffers or a depth stencil buffer is required.");

			_renderTarget = graphicsDevice.CreateRenderTarget(depthStencil, colorBuffers);

			if (depthStencil != null)
				Size = depthStencil.Size;
			else
				Size = colorBuffers[0].Size;
		}

		/// <summary>
		///     Gets a value indicating whether the render target is the back buffer of a swap chain.
		/// </summary>
		public bool IsBackBuffer
		{
			get { return _renderTarget.IsBackBuffer; }
		}

		/// <summary>
		///     Gets the size of the render target.
		/// </summary>
		public Size Size
		{
			get { return _renderTarget.Size; }
			private set { _renderTarget.Size = value; }
		}

		/// <summary>
		///     Gets the width of the render target.
		/// </summary>
		public float Width
		{
			get { return Size.Width; }
		}

		/// <summary>
		///     Gets the height of the render target.
		/// </summary>
		public float Height
		{
			get { return Size.Height; }
		}

		/// <summary>
		///     Binds the render target to the output merger state.
		/// </summary>
		internal void Bind()
		{
			Assert.NotDisposed(this);

			if (DeviceState.Change(ref GraphicsDevice.State.RenderTarget, this))
				_renderTarget.Bind();
		}

		/// <summary>
		///     Clears the color buffers of the render target.
		/// </summary>
		/// <param name="color">The color the color buffer should be set to.</param>
		internal void ClearColor(Color color)
		{
			Assert.NotDisposed(this);
			Assert.That(IsBackBuffer || _colorBuffers != null && _colorBuffers.Length > 0,
				"Cannot clear color of a render target without any color buffers.");

			_renderTarget.ClearColor(color);
		}

		/// <summary>
		///     Clears the depth buffer of the render target to the given value.
		/// </summary>
		/// <param name="depth">The value the depth buffer should be set to.</param>
		internal void ClearDepth(float depth)
		{
			Assert.NotDisposed(this);
			Assert.NotNull(_depthStencil, "Cannot clear depth of a render target without a depth stencil buffer.");

			_renderTarget.ClearDepthStencil(true, false, depth, 0);
		}

		/// <summary>
		///     Clears the stencil buffer of the render target to the given value.
		/// </summary>
		/// <param name="stencil">The value the stencil buffer should be set to.</param>
		internal void ClearStencil(byte stencil)
		{
			Assert.NotDisposed(this);
			Assert.NotNull(_depthStencil, "Cannot clear stencil of a render target without a depth stencil buffer.");

			_renderTarget.ClearDepthStencil(false, true, 0.0f, stencil);
		}

		/// <summary>
		///     Invoked after the name of the graphics object has changed. This method is only invoked in debug builds.
		/// </summary>
		/// <param name="name">The new name of the graphics object.</param>
		protected override void OnRenamed(string name)
		{
			_renderTarget.SetName(name);
		}

		/// <summary>
		///     Clears the depth and stencil buffers of the render target to the given values.
		/// </summary>
		/// <param name="depth">The value the depth buffer should be set to.</param>
		/// <param name="stencil">The value the stencil buffer should be set to.</param>
		internal void ClearDepthStencil(float depth, byte stencil)
		{
			Assert.NotDisposed(this);
			Assert.NotNull(_depthStencil, "Cannot clear depth and stencil of a render target without a depth stencil buffer.");

			_renderTarget.ClearDepthStencil(true, true, depth, stencil);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_renderTarget.SafeDispose();
			_depthStencil.SafeDispose();
			_colorBuffers.SafeDisposeAll();
		}
	}
}