namespace Pegasus.Platform.Graphics
{
	using System;
	using Math;
	using Memory;
	using Rendering;
	using Utilities;

	/// <summary>
	///     Represents the target of a rendering operation.
	/// </summary>
	public sealed unsafe class RenderTarget : GraphicsObject
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
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="backBuffer">The native render target object that represents the back buffer.</param>
		internal RenderTarget(GraphicsDevice graphicsDevice, void* backBuffer)
			: base(graphicsDevice)
		{
			Assert.ArgumentNotNull(new IntPtr(backBuffer));

			NativeObject = backBuffer;
			IsBackBuffer = true;
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

			Size size;
			if (depthStencil != null)
				size = depthStencil.Size;
			else
				size = colorBuffers[0].Size;

			var width = size.IntegralWidth;
			var height = size.IntegralHeight;
			var depthStencilPtr = depthStencil == null ? null : depthStencil.NativeObject;

			if (colorBuffers != null)
			{
				var colorBuffersArray = stackalloc void*[colorBuffers.Length];
				for (var i = 0; i < colorBuffers.Length; ++i)
					colorBuffersArray[i] = colorBuffers[i].NativeObject;

				NativeObject = DeviceInterface->InitializeRenderTarget(colorBuffersArray, colorBuffers.Length, depthStencilPtr, width, height);
			}
			else
				NativeObject = DeviceInterface->InitializeRenderTarget(null, 0, depthStencilPtr, width, height);
		}

		/// <summary>
		///     Gets a value indicating whether the render target is the back buffer of a swap chain.
		/// </summary>
		public bool IsBackBuffer { get; private set; }

		/// <summary>
		///     Gets the size of the render target.
		/// </summary>
		public Size Size
		{
			get
			{
				int width, height;
				DeviceInterface->GetRenderTargetSize(NativeObject, &width, &height);
				return new Size(width, height);
			}
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
		///     Gets the function that should be used to set the debug name of the native object.
		/// </summary>
		protected override SetNameDelegate SetNameFunction
		{
			get { return DeviceInterface->SetRenderTargetName; }
		}

		/// <summary>
		///     Binds the render target to the output merger state.
		/// </summary>
		internal void Bind()
		{
			Assert.NotDisposed(this);

			if (DeviceState.Change(ref DeviceState.RenderTarget, this))
				DeviceInterface->BindRenderTarget(NativeObject);
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

			DeviceInterface->ClearColor(NativeObject, &color);
		}

		/// <summary>
		///     Clears the depth buffer of the render target to the given value.
		/// </summary>
		/// <param name="depth">The value the depth buffer should be set to.</param>
		internal void ClearDepth(float depth)
		{
			Assert.NotDisposed(this);
			Assert.NotNull(_depthStencil, "Cannot clear depth of a render target without a depth stencil buffer.");

			DeviceInterface->ClearDepthStencil(NativeObject, true, false, depth, 0);
		}

		/// <summary>
		///     Clears the stencil buffer of the render target to the given value.
		/// </summary>
		/// <param name="stencil">The value the stencil buffer should be set to.</param>
		internal void ClearStencil(byte stencil)
		{
			Assert.NotDisposed(this);
			Assert.NotNull(_depthStencil, "Cannot clear stencil of a render target without a depth stencil buffer.");

			DeviceInterface->ClearDepthStencil(NativeObject, false, true, 0.0f, stencil);
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

			DeviceInterface->ClearDepthStencil(NativeObject, true, true, depth, stencil);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			if (IsBackBuffer)
				return;

			DeviceInterface->FreeRenderTarget(NativeObject);
			_depthStencil.SafeDispose();
			_colorBuffers.SafeDisposeAll();
		}
	}
}