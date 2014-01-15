namespace Pegasus.Rendering
{
	using System;
	using System.Runtime.InteropServices;
	using Math;
	using Platform.Graphics;

	/// <summary>
	///     Represents a rectangle with possibly non-axis aligned edges.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct Quad
	{
		/// <summary>
		///     Holds position, texture coordinates, and color data for a vertex.
		/// </summary>
		[StructLayout(LayoutKind.Explicit, Pack = 1)]
		private struct Vertex
		{
			/// <summary>
			///     The size in bytes of the structure.
			/// </summary>
			public const int Size = 20;

			/// <summary>
			///     Gets or sets the vertex' position.
			/// </summary>
			[FieldOffset(0)]
			public Vector2 Position;

			/// <summary>
			///     Gets or sets the vertex' texture coordinates.
			/// </summary>
			[FieldOffset(8)]
			public Vector2 TextureCoordinates;

			/// <summary>
			///     Gets or sets the vertex' color.
			/// </summary>
			[FieldOffset(16)]
			public Color Color;
		}

		/// <summary>
		///     Gets a vertex input layout for drawing quads with an appropriate vertex buffer and vertex shader.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to construct the input layout.</param>
		/// <param name="vertexBuffer">The vertex buffer that holds the vertex data.</param>
		/// <param name="indexBuffer">The index buffer that holds the vertex indices.</param>
		public static VertexInputLayout GetInputLayout(GraphicsDevice graphicsDevice, VertexBuffer vertexBuffer,
													   IndexBuffer indexBuffer)
		{
			Assert.ArgumentNotNull(graphicsDevice);
			Assert.ArgumentNotNull(vertexBuffer);
			Assert.That(Marshal.SizeOf(typeof(Vertex)) == Vertex.Size, "Unexpected unamanged size.");

			var inputElements = new[]
			{
				new VertexInputBinding(vertexBuffer, VertexDataFormat.Vector2, DataSemantics.Position, Vertex.Size, 0),
				new VertexInputBinding(vertexBuffer, VertexDataFormat.Vector2, DataSemantics.TexCoords0, Vertex.Size, 8),
				new VertexInputBinding(vertexBuffer, VertexDataFormat.Color, DataSemantics.Color0, Vertex.Size, 16)
			};

			return new VertexInputLayout(graphicsDevice, indexBuffer, inputElements);
		}

		/// <summary>
		///     Creates a dynamic vertex buffer that holds the given number of quads.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to construct the dynamic vertex buffer.</param>
		/// <param name="quadCount">The number of quads that the dynamic vertex buffer should be able to hold.</param>
		/// <param name="chunkCount">The number of chunks that the dynamic vertex buffer should allocate.</param>
		/// <param name="requiresSynchronization">
		///     If true, mapping the vertex buffer and all drawing operations involving the vertex buffer require CPU/GPU
		///     synchronization.
		/// </param>
		public static DynamicVertexBuffer CreateDynamicVertexBuffer(GraphicsDevice graphicsDevice, int quadCount, int chunkCount,
																	bool requiresSynchronization)
		{
			return DynamicVertexBuffer.Create<Vertex>(graphicsDevice, quadCount * 4, chunkCount, requiresSynchronization);
		}

		/// <summary>
		///     The vertex that conceptually represents the bottom left corner of the quad.
		/// </summary>
		private Vertex _bottomLeft;

		/// <summary>
		///     The vertex that conceptually represents the bottom right corner of the quad.
		/// </summary>
		private Vertex _bottomRight;

		/// <summary>
		///     The vertex that conceptually represents the top left corner of the quad.
		/// </summary>
		private Vertex _topLeft;

		/// <summary>
		///     The vertex that conceptually represents the top right corner of the quad.
		/// </summary>
		private Vertex _topRight;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="rectangle">The position and size of the rectangular quad.</param>
		/// <param name="color">The color of the quad.</param>
		/// <param name="textureArea">
		///     The area of the texture that contains the quad's image data. If not given, the whole texture
		///     is placed onto the rectangle.
		/// </param>
		public Quad(RectangleF rectangle, Color color, RectangleF? textureArea = null)
			: this()
		{
			_bottomLeft.Position = new Vector2(rectangle.Left, rectangle.Bottom);
			_bottomRight.Position = new Vector2(rectangle.Right, rectangle.Bottom);
			_topLeft.Position = new Vector2(rectangle.Left, rectangle.Top);
			_topRight.Position = new Vector2(rectangle.Right, rectangle.Top);

			_bottomLeft.Color = color;
			_bottomRight.Color = color;
			_topLeft.Color = color;
			_topRight.Color = color;

			var texture = textureArea ?? new RectangleF(0, 0, 1, 1);
			_bottomLeft.TextureCoordinates = new Vector2(texture.Left, texture.Top);
			_bottomRight.TextureCoordinates = new Vector2(texture.Right, texture.Top);
			_topLeft.TextureCoordinates = new Vector2(texture.Left, texture.Bottom);
			_topRight.TextureCoordinates = new Vector2(texture.Right, texture.Bottom);
		}

		/// <summary>
		///     Changes the color of the quad.
		/// </summary>
		/// <param name="color">The new color of the quad.</param>
		public void SetColor(Color color)
		{
			_bottomLeft.Color = color;
			_bottomRight.Color = color;
			_topLeft.Color = color;
			_topRight.Color = color;
		}

		/// <summary>
		///     Applies the given transformation matrix to the quad's vertices.
		/// </summary>
		/// <param name="quad">The quad that should be transformed.</param>
		/// <param name="matrix">The transformation matrix that should be applied.</param>
		public static void Transform(ref Quad quad, ref Matrix matrix)
		{
			quad._bottomLeft.Position = Vector2.Transform(ref quad._bottomLeft.Position, ref matrix);
			quad._bottomRight.Position = Vector2.Transform(ref quad._bottomRight.Position, ref matrix);
			quad._topLeft.Position = Vector2.Transform(ref quad._topLeft.Position, ref matrix);
			quad._topRight.Position = Vector2.Transform(ref quad._topRight.Position, ref matrix);
		}

		/// <summary>
		///     Applies the given position offset to the quad's vertices.
		/// </summary>
		/// <param name="quad">The quad that should be transformed.</param>
		/// <param name="positionOffset">The position offset that should be applied.</param>
		public static void Offset(ref Quad quad, ref Vector2 positionOffset)
		{
			quad._bottomLeft.Position = quad._bottomLeft.Position + positionOffset;
			quad._bottomRight.Position = quad._bottomRight.Position + positionOffset;
			quad._topLeft.Position = quad._topLeft.Position + positionOffset;
			quad._topRight.Position = quad._topRight.Position + positionOffset;
		}
	}
}