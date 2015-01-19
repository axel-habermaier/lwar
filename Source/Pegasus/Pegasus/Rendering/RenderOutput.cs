namespace Pegasus.Rendering
{
	using System;
	using Math;
	using Platform.Graphics;
	using Utilities;

	/// <summary>
	///     Represents a rendering output configuration that can be used to draw geometry into the configured render target.
	/// </summary>
	public class RenderOutput
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="renderContext">The render context that should be used for rendering.</param>
		public RenderOutput(RenderContext renderContext)
		{
			Assert.ArgumentNotNull(renderContext);
			RenderContext = renderContext;
		}

		/// <summary>
		///     Gets the render context that is used for rendering.
		/// </summary>
		public RenderContext RenderContext { get; private set; }

		/// <summary>
		///     Gets the graphics device that is used for rendering.
		/// </summary>
		public GraphicsDevice GraphicsDevice
		{
			get { return RenderContext.GraphicsDevice; }
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
		public Rectangle Viewport { get; set; }

		/// <summary>
		///     Gets or sets the scissor area of the rendering operation.
		/// </summary>
		public Rectangle ScissorArea { get; set; }

		/// <summary>
		///     Draws primitiveCount-many primitives, starting at the given offset into the currently bound vertex buffers.
		/// </summary>
		/// <param name="effect">The effect that should be used for drawing.</param>
		/// <param name="primitiveCount">The number of primitives that should be drawn.</param>
		/// <param name="primitiveType">The type of the primitives that should be drawn.</param>
		/// <param name="offset">The offset into the vertex buffers.</param>
		public void Draw(EffectTechnique effect, int primitiveCount,
						 PrimitiveType primitiveType = PrimitiveType.TriangleList, int offset = 0)
		{
			Bind();
			effect.Bind();

			GraphicsDevice.ChangePrimitiveType(primitiveType);
			GraphicsDevice.Draw(primitiveCount, offset);

			effect.Unbind();
		}

		/// <summary>
		///     Draws indexCount-many indices, starting at the given index offset into the currently bound index buffer, where the
		///     vertex offset is added to each index before accessing the currently bound vertex buffers.
		/// </summary>
		/// <param name="effect">The effect that should be used for drawing.</param>
		/// <param name="indexCount">The number of indices to draw.</param>
		/// <param name="indexOffset">The location of the first index read by the GPU from the index buffer.</param>
		/// <param name="primitiveType">The type of the primitives that should be drawn.</param>
		/// <param name="vertexOffset">The value that should be added to each index before reading a vertex from the vertex buffer.</param>
		public void DrawIndexed(EffectTechnique effect, int indexCount, int indexOffset = 0,
								PrimitiveType primitiveType = PrimitiveType.TriangleList, int vertexOffset = 0)
		{
			Bind();
			effect.Bind();

			GraphicsDevice.ChangePrimitiveType(primitiveType);
			GraphicsDevice.DrawIndexed(indexCount, indexOffset, vertexOffset);

			effect.Unbind();
		}

		/// <summary>
		///     Draws primitiveCount-many instanced primitives, starting at the given offset into the currently bound vertex buffers.
		/// </summary>
		/// <param name="effect">The effect that should be used for drawing.</param>
		/// <param name="instanceCount">The number of instanced that should be drawn.</param>
		/// <param name="primitiveCount">The number of primitives that should be drawn per instance.</param>
		/// <param name="primitiveType">The type of the primitives that should be drawn.</param>
		/// <param name="offset">The offset into the vertex buffers.</param>
		/// <param name="instanceOffset">The offset applied to the instanced vertex buffers.</param>
		internal void DrawInstanced(EffectTechnique effect, int instanceCount, int primitiveCount,
									PrimitiveType primitiveType = PrimitiveType.TriangleList, int offset = 0, int instanceOffset = 0)
		{
			Bind();
			effect.Bind();

			GraphicsDevice.ChangePrimitiveType(primitiveType);
			GraphicsDevice.DrawInstanced(instanceCount, primitiveCount, offset, instanceOffset);

			effect.Unbind();
		}

		/// <summary>
		///     Draws indexCount-many instanced indices, starting at the given index offset into the currently bound index buffer.
		/// </summary>
		/// <param name="effect">The effect that should be used for drawing.</param>
		/// <param name="instanceCount">The number of instances to draw.</param>
		/// <param name="indexCount">The number of indices to draw per instance.</param>
		/// <param name="primitiveType">The type of the primitives that should be drawn.</param>
		/// <param name="indexOffset">The location of the first index read by the GPU from the index buffer.</param>
		/// <param name="vertexOffset">The offset applied to the non-instanced vertex buffers.</param>
		/// <param name="instanceOffset">The offset applied to the instanced vertex buffers.</param>
		internal void DrawIndexedInstanced(EffectTechnique effect, int instanceCount, int indexCount,
										   PrimitiveType primitiveType = PrimitiveType.TriangleList,
										   int indexOffset = 0, int vertexOffset = 0, int instanceOffset = 0)
		{
			Bind();
			effect.Bind();

			GraphicsDevice.ChangePrimitiveType(primitiveType);
			GraphicsDevice.DrawIndexedInstanced(instanceCount, indexCount, indexOffset, vertexOffset, instanceOffset);

			effect.Unbind();
		}

		/// <summary>
		///     Clears the color buffers of the render target.
		/// </summary>
		/// <param name="color">The color the color buffer should be set to.</param>
		public void ClearColor(Color color)
		{
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
			Assert.NotNull(RenderTarget, "No render target has been set.");

			RenderTarget.Bind();
			RenderTarget.ClearDepth(depth);
		}

		/// <summary>
		///     Binds the required state to the graphics device.
		/// </summary>
		private void Bind()
		{
			Assert.NotNull(GraphicsDevice, "No graphics device has been set.");
			Assert.NotNull(RenderTarget, "No render target has been set.");
			Assert.NotNull(Camera, "No camera has been set.");
			Assert.That(Viewport.Width * Viewport.Height > 0, "Viewport has area 0.");

			RenderTarget.Bind();
			Camera.Bind();

			GraphicsDevice.ChangeViewport(Viewport);
			GraphicsDevice.ChangeScissorArea(ScissorArea);
		}
	}
}