using System;

namespace Pegasus.Rendering
{
	using System.Runtime.InteropServices;
	using Math;
	using Platform.Graphics;

	/// <summary>
	///   Holds position, texture coordinates, and normal data for a vertex.
	/// </summary>
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct VertexPositionNormalTexture
	{
		/// <summary>
		///   The size in bytes of the structure.
		/// </summary>
		public const int Size = 36;

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
		public Vector3 Normal;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="position">The position of the vertex.</param>
		/// <param name="normal">The normal of the vertex.</param>
		/// <param name="textureCoordinates">The texture coordinates of the vertex.</param>
		public VertexPositionNormalTexture(Vector4 position, Vector3 normal, Vector2 textureCoordinates)
			: this()
		{
			Position = position;
			Normal = normal;
			TextureCoordinates = textureCoordinates;
		}

		/// <summary>
		///   Gets a vertex input layout for drawing VertexPositionNormalTexture vertices with an appropriate vertex buffer
		///   and vertex shader.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to construct the input layout.</param>
		/// <param name="vertexBuffer">The vertex buffer that holds the vertex data.</param>
		/// <param name="indexBuffer">The index buffer that holds the vertex indices.</param>
		public static VertexInputLayout GetInputLayout(GraphicsDevice graphicsDevice, VertexBuffer vertexBuffer,
													   IndexBuffer indexBuffer)
		{
			Assert.ArgumentNotNull(graphicsDevice);
			Assert.ArgumentNotNull(vertexBuffer);
			Assert.That(Marshal.SizeOf(typeof(VertexPositionNormalTexture)) == Size, "Unexpected unamanged size.");

			var inputElements = new[]
			{
				new VertexInputBinding(vertexBuffer, VertexDataFormat.Vector4, DataSemantics.Position, Size, 0),
				new VertexInputBinding(vertexBuffer, VertexDataFormat.Vector2, DataSemantics.TexCoords0, Size, 16),
				new VertexInputBinding(vertexBuffer, VertexDataFormat.Vector3, DataSemantics.Normal, Size, 24)
			};

			return new VertexInputLayout(graphicsDevice, indexBuffer, inputElements);
		}
	}
}