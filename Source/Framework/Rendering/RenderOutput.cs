using System;

namespace Pegasus.Framework.Rendering
{
	using Math;
	using Platform.Graphics;

	/// <summary>
	///   Represents a rendering output configuration that can be used to draw geometry on the configured render target.
	/// </summary>
	public struct RenderOutput
	{
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
		public Rectangle Viewport { get; set; }

		/// <summary>
		///   Gets or sets the scissor area of the rendering operation.
		/// </summary>
		public Rectangle ScissorArea { get; set; }

		/// <summary>
		///   Gets or sets the effect technique that should be used for rendering.
		/// </summary>
		public EffectTechnique Effect { get; set; }

		/// <summary>
		///   Draws primitiveCount-many primitives of the given type, starting at the given offset
		///   into the currently bound vertex buffers.
		/// </summary>
		/// <param name="primitiveCount">The number of primitives that should be drawn.</param>
		/// <param name="offset">The offset into the vertex buffers.</param>
		public void Draw(int primitiveCount, int offset)
		{
			//Assert.NotDisposed(this);
			//NativeMethods.Draw(_device, primitiveCount, offset);
		}

		/// <summary>
		///   Draws indexed geometry.
		/// </summary>
		/// <param name="indexCount">The number of indices to draw.</param>
		/// <param name="indexOffset">The location of the first index read by the GPU from the index buffer.</param>
		/// <param name="vertexOffset">A value added to each index before reading a vertex from the vertex buffer.</param>
		public void DrawIndexed(int indexCount, int indexOffset = 0, int vertexOffset = 0)
		{
			//Assert.NotDisposed(this);
			//NativeMethods.DrawIndexed(_device, indexCount, indexOffset, vertexOffset);
		}
	}
}