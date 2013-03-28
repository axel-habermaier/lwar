using System;

namespace Pegasus.Framework.Rendering
{
	using System.Runtime.InteropServices;
	using Math;
	using Platform.Graphics;

	/// <summary>
	///   Holds position, texture coordinates, and color data for a vertex.
	/// </summary>
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct VertexPositionColorTexture
	{
		/// <summary>
		///   The size in bytes of the structure.
		/// </summary>
		public const int Size = 28;

		/// <summary>
		///   Gets or sets the vertex' position.
		/// </summary>
		[FieldOffset(0)]
		public Vector4 Position;

		/// <summary>
		///   Gets or sets the vertex' texture coordinates.
		/// </summary>
		[FieldOffset(16)]
		public Vector2 TextureCoordinates;

		/// <summary>
		///   Gets or sets the vertex' color.
		/// </summary>
		[FieldOffset(24)]
		public Color Color;

		/// <summary>
		///   Gets a vertex input layout for drawing VertexPositionColorTexture vertices with an appropriate vertex buffer
		///   and vertex shader.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to construct the input layout.</param>
		/// <param name="vertexBuffer">The vertex buffer that holds the vertex data.</param>
		/// <param name="indexBuffer">The index buffer that holds the vertex indices.</param>
		public static VertexInputLayout GetInputLayout(GraphicsDevice graphicsDevice, VertexBuffer vertexBuffer,
													   IndexBuffer indexBuffer)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(vertexBuffer, () => vertexBuffer);
			Assert.That(Marshal.SizeOf(typeof(VertexPositionColorTexture)) == Size, "Unexpected unamanged size.");

			var inputElements = new[]
			{
				new VertexInputBinding(vertexBuffer, VertexDataFormat.Vector4, DataSemantics.Position, Size, 0),
				new VertexInputBinding(vertexBuffer, VertexDataFormat.Vector2, DataSemantics.TexCoords0, Size, 16),
				new VertexInputBinding(vertexBuffer, VertexDataFormat.Color, DataSemantics.Color0, Size, 24)
			};

			return new VertexInputLayout(graphicsDevice, indexBuffer, inputElements);
		}
	}
}