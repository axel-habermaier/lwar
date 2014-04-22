﻿namespace Pegasus.Rendering
{
	using System;
	using Math;
	using Platform.Graphics;
	using Platform.Memory;

	/// <summary>
	///     Represents a rendering output configuration that can be used to draw geometry into the configured render target.
	/// </summary>
	public class RenderOutput : DisposableObject
	{
		/// <summary>
		///     The slot that is used to pass the viewport buffer to the vertex shaders.
		/// </summary>
		private const int ViewportBufferSlot = 1;

		/// <summary>
		///     The constant buffer that holds the viewport-related data that is passed to each vertex shader.
		/// </summary>
		private readonly ConstantBuffer _viewportBuffer;

		/// <summary>
		///     The viewport of the rendering operation.
		/// </summary>
		private Rectangle _viewport;

		/// <summary>
		///     Indicates whether the viewport has changed and the constant buffer must be updated.
		/// </summary>
		private bool _viewportHasChanged = true;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public RenderOutput()
		{
			_viewportBuffer = new ConstantBuffer(16, ViewportBufferSlot);
			_viewportBuffer.SetName("RenderOutput.ViewportBuffer");
		}

		/// <summary>
		///     Gets or sets the render target that should be rendered to.
		/// </summary>
		public RenderTarget RenderTarget { get; set; }

		/// <summary>
		///     Gets or sets the camera that should be used for rendering.
		/// </summary>
		public Camera Camera { get; set; }

		/// <summary>
		///     Gets or sets the viewport of the rendering operation.
		/// </summary>
		public Rectangle Viewport
		{
			get { return _viewport; }
			set
			{
				if (_viewport == value)
					return;

				_viewportHasChanged = true;
				_viewport = value;
			}
		}

		/// <summary>
		///     Gets or sets the scissor area of the rendering operation.
		/// </summary>
		public Rectangle ScissorArea { get; set; }

		/// <summary>
		///     Draws primitiveCount-many primitives, starting at the given offset into the currently bound vertex buffers.
		/// </summary>
		/// <param name="effect">The effect that should be used for drawing.</param>
		/// <param name="primitiveCount">The number of primitives that should be drawn.</param>
		/// <param name="offset">The offset into the vertex buffers.</param>
		/// <param name="primitiveType">The type of the primitives that should be drawn.</param>
		public void Draw(EffectTechnique effect, int primitiveCount, int offset = 0,
						 PrimitiveType primitiveType = PrimitiveType.Triangles)
		{
			Bind();
			effect.Bind();

			GraphicsDevice.Current.PrimitiveType = primitiveType;
			GraphicsDevice.Current.Draw(primitiveCount, offset);

			effect.Unbind();
		}

		/// <summary>
		///     Draws indexCount-many indices, starting at the given index offset into the currently bound index buffer, where the
		///     vertex offset is added to each index before accessing the currently bound vertex buffers.
		/// </summary>
		/// <param name="effect">The effect that should be used for drawing.</param>
		/// <param name="indexCount">The number of indices to draw.</param>
		/// <param name="indexOffset">The location of the first index read by the GPU from the index buffer.</param>
		/// <param name="vertexOffset">The value that should be added to each index before reading a vertex from the vertex buffer.</param>
		/// <param name="primitiveType">The type of the primitives that should be drawn.</param>
		public void DrawIndexed(EffectTechnique effect, int indexCount, int indexOffset = 0, int vertexOffset = 0,
								PrimitiveType primitiveType = PrimitiveType.Triangles)
		{
			Bind();
			effect.Bind();

			GraphicsDevice.Current.PrimitiveType = primitiveType;
			GraphicsDevice.Current.DrawIndexed(indexCount, indexOffset, vertexOffset);

			effect.Unbind();
		}

		/// <summary>
		///     Clears the color buffers of the render target.
		/// </summary>
		/// <param name="color">The color the color buffer should be set to.</param>
		public void ClearColor(Color color)
		{
			Assert.NotDisposed(this);
			Assert.NotNull(RenderTarget, "No render target has been set.");

			RenderTarget.Bind();
			RenderTarget.ClearColor(color);
		}

		/// <summary>
		///     Clears the depth buffer of the render target.
		/// </summary>
		/// <param name="depth">The value the depth buffer should be set to.</param>
		public void ClearDepth(float depth = 1.0f)
		{
			Assert.NotDisposed(this);
			Assert.NotNull(RenderTarget, "No render target has been set.");

			RenderTarget.Bind();
			RenderTarget.ClearDepthStencil(true, false, depth);
		}

		/// <summary>
		///     Binds the required state to the graphics device.
		/// </summary>
		private unsafe void Bind()
		{
			Assert.NotDisposed(this);
			Assert.NotNull(GraphicsDevice.Current, "No graphics device has been set.");
			Assert.NotNull(RenderTarget, "No render target has been set.");
			Assert.NotNull(Camera, "No camera has been set.");
			Assert.That(Viewport.Width * Viewport.Height > 0, "Viewport has area 0.");

			RenderTarget.Bind();
			Camera.Bind();

			GraphicsDevice.Current.Viewport = Viewport;
			GraphicsDevice.Current.ScissorArea = ScissorArea;

			if (_viewportHasChanged)
			{
				var viewportSize = new Vector2(_viewport.Width, _viewport.Height);
				_viewportBuffer.CopyData(&viewportSize);

				_viewportHasChanged = false;
			}

			_viewportBuffer.Bind();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_viewportBuffer.SafeDispose();
		}
	}
}