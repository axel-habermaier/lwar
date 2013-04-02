using System;

namespace Pegasus.Framework.Rendering
{
	using Math;
	using Platform.Graphics;

	/// <summary>
	///   Represents a rendering output configuration that can be used to draw geometry on the configured render target.
	/// </summary>
	public class RenderOutput : DisposableObject
	{
		/// <summary>
		///   The slot that is used to pass the viewport buffer to the vertex shaders.
		/// </summary>
		private const int ViewportBufferSlot = 1;

		/// <summary>
		///   The graphics device that is used for rendering.
		/// </summary>
		private readonly GraphicsDevice _graphicsDevice;

		/// <summary>
		///   The constant buffer that holds the viewport-related data that is passed to each vertex shader.
		/// </summary>
		private readonly ConstantBuffer _viewportBuffer;

		/// <summary>
		///   The viewport of the rendering operation.
		/// </summary>
		private Rectangle _viewport;

		/// <summary>
		///   Indicates whether the viewport has changed and the constant buffer must be updated.
		/// </summary>
		private bool _viewportHasChanged = true;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for rendering.</param>
		public RenderOutput(GraphicsDevice graphicsDevice)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);

			_graphicsDevice = graphicsDevice;
			_viewportBuffer = new ConstantBuffer(graphicsDevice, 16, ViewportBufferSlot);
		}

		/// <summary>
		///   Gets or sets the render target that should be rendered to.
		/// </summary>
		public RenderTarget RenderTarget { get; set; }

		/// <summary>
		///   Gets or sets the camera that should be used for rendering.
		/// </summary>
		public Camera Camera { get; set; }

		/// <summary>
		///   Gets or sets the viewport of the rendering operation.
		/// </summary>
		public Rectangle Viewport
		{
			get { return _viewport; }
			set
			{
				if (_viewport != value)
					return;

				_viewportHasChanged = true;
				_viewport = value;
			}
		}

		/// <summary>
		///   Gets or sets the scissor area of the rendering operation.
		/// </summary>
		public Rectangle ScissorArea { get; set; }

		/// <summary>
		///   Draws primitiveCount-many primitives, starting at the given offset into the currently bound vertex buffers.
		/// </summary>
		/// <param name="primitiveCount">The number of primitives that should be drawn.</param>
		/// <param name="offset">The offset into the vertex buffers.</param>
		public void Draw(int primitiveCount, int offset)
		{
			Bind();
			_graphicsDevice.Draw(primitiveCount, offset);
		}

		/// <summary>
		///   Draws indexCount-many indices, starting at the given index offset into the currently bound index buffer, where the
		///   vertex offset is added to each index before accessing the currently bound vertex buffers.
		/// </summary>
		/// <param name="indexCount">The number of indices to draw.</param>
		/// <param name="indexOffset">The location of the first index read by the GPU from the index buffer.</param>
		/// <param name="vertexOffset">The value that should be added to each index before reading a vertex from the vertex buffer.</param>
		public void DrawIndexed(int indexCount, int indexOffset = 0, int vertexOffset = 0)
		{
			Bind();
			_graphicsDevice.DrawIndexed(indexCount, indexOffset, vertexOffset);
		}

		/// <summary>
		///   Binds the required state to the graphics device.
		/// </summary>
		private unsafe void Bind()
		{
			Assert.NotDisposed(this);
			Assert.NotNull(_graphicsDevice, "No graphics device has been set.");
			Assert.NotNull(RenderTarget, "No graphics device has been set.");
			Assert.NotNull(Camera, "No camera has been set.");
			Assert.That(Viewport.Width * Viewport.Height > 0, "Viewport has area 0.");

			RenderTarget.Bind();

			Camera.Viewport = Viewport;
			Camera.Bind();

			_graphicsDevice.Viewport = Viewport;
			_graphicsDevice.ScissorArea = ScissorArea;

			if (_viewportHasChanged)
			{
				var viewportSize = new Vector2(_viewport.Width, _viewport.Height);
				_viewportBuffer.CopyData(&viewportSize);

				_viewportHasChanged = false;
			}

			_viewportBuffer.Bind();
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_viewportBuffer.SafeDispose();
		}
	}
}