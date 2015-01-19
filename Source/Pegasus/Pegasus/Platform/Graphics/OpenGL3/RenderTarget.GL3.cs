namespace Pegasus.Platform.Graphics.OpenGL3
{
	using System;
	using Bindings;
	using Interface;
	using Logging;
	using Math;
	using Rendering;
	using Utilities;

	/// <summary>
	///     Represents an OpenGL3-based target of a rendering operation.
	/// </summary>
	internal unsafe class RenderTargetGL3 : GraphicsObjectGL3, IRenderTarget
	{
		/// <summary>
		///     The frame buffer object handle.
		/// </summary>
		private readonly uint _handle;

		/// <summary>
		///     The swap chain the render target belongs to, if any.
		/// </summary>
		private readonly SwapChainGL3 _swapChain;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="swapChain">The swap chain the render target should belong to.</param>
		public RenderTargetGL3(GraphicsDeviceGL3 graphicsDevice, SwapChainGL3 swapChain)
			: base(graphicsDevice)
		{
			_swapChain = swapChain;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="depthStencil">The depth stencil buffer that should be bound to the render target.</param>
		/// <param name="colorBuffers">The color buffers that should be bound to the render target.</param>
		public RenderTargetGL3(GraphicsDeviceGL3 graphicsDevice, Texture2D depthStencil, Texture2D[] colorBuffers)
			: base(graphicsDevice)
		{
			_handle = GLContext.Allocate(_gl.GenFramebuffers, "RenderTarget");

			const int bufferCount = 4;
			var buffers = stackalloc uint[bufferCount];
			buffers[0] = GL.ColorAttachment0;
			buffers[1] = GL.ColorAttachment1;
			buffers[2] = GL.ColorAttachment2;
			buffers[3] = GL.ColorAttachment3;

			Assert.That(Graphics.GraphicsDevice.MaxColorBuffers == bufferCount, "Color buffer count mismatch.");
			_gl.BindFramebuffer(GL.DrawFramebuffer, _handle);

			if (depthStencil != null)
			{
				var texture = (TextureGL3)depthStencil.TextureObject;
				_gl.FramebufferTexture2D(GL.DrawFramebuffer, GL.DepthStencilAttachment, texture.TargetType, texture.Handle, 0);
			}

			for (var i = 0; i < colorBuffers.Length; ++i)
			{
				var texture = (TextureGL3)colorBuffers[i].TextureObject;
				_gl.FramebufferTexture2D(GL.DrawFramebuffer, (uint)(GL.ColorAttachment0 + i), texture.TargetType, texture.Handle, 0);
			}

			ValidateFramebufferCompleteness();
			_gl.DrawBuffers(colorBuffers.Length, buffers);

			RebindRenderTarget();
		}

		/// <summary>
		///     Gets or sets the size of the render target.
		/// </summary>
		public Size Size { get; set; }

		/// <summary>
		///     Gets a value indicating whether the render target is the back buffer of a swap chain.
		/// </summary>
		public bool IsBackBuffer
		{
			get { return _swapChain != null; }
		}

		/// <summary>
		///     Clears the color buffers of the render target.
		/// </summary>
		/// <param name="color">The color the color buffer should be set to.</param>
		public void ClearColor(Color color)
		{
			var scissorEnabled = GraphicsDevice.State.ScissorEnabled;
			GraphicsDevice.EnableScissor(false);

			GraphicsDevice.SetClearColor(color);
			_gl.Clear(GL.ColorBufferBit);

			if (scissorEnabled != null)
				GraphicsDevice.EnableScissor(scissorEnabled.Value);
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
			var scissorEnabled = GraphicsDevice.State.ScissorEnabled;
			var depthWritesEnabled = GraphicsDevice.State.DepthWritesEnabled;

			GraphicsDevice.EnableScissor(false);
			GraphicsDevice.EnableDepthWrites(true);

			uint glTargets = 0;
			if (clearDepth)
				glTargets |= GL.DepthBufferBit;
			if (clearStencil)
				glTargets |= GL.StencilBufferBit;

			GraphicsDevice.SetClearDepth(depth);
			GraphicsDevice.SetClearStencil(stencil);
			_gl.Clear(glTargets);

			if (scissorEnabled != null)
				GraphicsDevice.EnableScissor(scissorEnabled.Value);

			if (depthWritesEnabled != null)
				GraphicsDevice.EnableDepthWrites(depthWritesEnabled.Value);
		}

		/// <summary>
		///     Binds the render target to the output merger state.
		/// </summary>
		public void Bind()
		{
			var viewport = GraphicsDevice.State.Viewport;
			var scissorArea = GraphicsDevice.State.ScissorArea;

			Bind(this);

			// We have to update the viewport and scissor rectangle as the the new render target might have a different size 
			// than the old one; viewports and scissor rectangles depend on the size of the currently bound render target
			// as the Y coordinate has to be inverted.
			// Without the following four lines, code that sets the viewport/scissor rectangle before binding the render
			// target would not work correctly
			GraphicsDevice.ChangeViewport(ref viewport);
			GraphicsDevice.ChangeScissorArea(ref scissorArea);
		}

		/// <summary>
		///     Sets the debug name of the render target.
		/// </summary>
		/// <param name="name">The debug name of the render target.</param>
		public void SetName(string name)
		{
			// Not supported by OpenGL
		}

		/// <summary>
		///     Checks the status of the frame buffer.
		/// </summary>
		private void ValidateFramebufferCompleteness()
		{
			switch (_gl.CheckFramebufferStatus(GL.DrawFramebuffer))
			{
				case GL.FramebufferComplete:
					return;
				case GL.FramebufferIncompleteAttachment:
					Log.Die("Frame buffer status: GL_FRAMEBUFFER_INCOMPLETE_ATTACHMENT.");
					return;
				case GL.FramebufferIncompleteMissingAttachment:
					Log.Die("Frame buffer status: GL_FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT.");
					return;
				case GL.FramebufferIncompleteDrawBuffer:
					Log.Die("Frame buffer status: GL_FRAMEBUFFER_INCOMPLETE_DRAW_BUFFER.");
					return;
				case GL.FramebufferIncompleteReadBuffer:
					Log.Die("Frame buffer status: GL_FRAMEBUFFER_INCOMPLETE_READ_BUFFER.");
					return;
				case GL.FramebufferUnsupported:
					Log.Die("Frame buffer status: GL_FRAMEBUFFER_UNSUPPORTED.");
					return;
				case GL.FramebufferIncompleteMultisample:
					Log.Die("Frame buffer status: GL_FRAMEBUFFER_INCOMPLETE_MULTISAMPLE.");
					return;
				case GL.FramebufferIncompleteLayerTargets:
					Log.Die("Frame buffer status: GL_FRAMEBUFFER_INCOMPLETE_LAYER_TARGETS.");
					return;
				default:
					Log.Die("The frame buffer is incomplete for an unknown reason.");
					return;
			}
		}

		/// <summary>
		///     Rebinds the previously bound render target.
		/// </summary>
		private void RebindRenderTarget()
		{
			if (GraphicsDevice.State.RenderTarget != null)
				Bind(GraphicsDevice.State.RenderTarget);
		}

		/// <summary>
		///     Binds the given render target to the graphics device.
		/// </summary>
		/// <param name="renderTarget">The render target that should be bound.</param>
		private void Bind(RenderTargetGL3 renderTarget)
		{
			Assert.ArgumentNotNull(renderTarget);

			GraphicsDevice.State.RenderTarget = this;

			if (_swapChain != null)
			{
				_swapChain.MakeCurrent();
				_gl.BindFramebuffer(GL.DrawFramebuffer, 0);
			}
			else
				_gl.BindFramebuffer(GL.DrawFramebuffer, renderTarget._handle);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			GLContext.Deallocate(_gl.DeleteFramebuffers, _handle);
		}
	}
}