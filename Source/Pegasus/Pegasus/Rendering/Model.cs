﻿namespace Pegasus.Rendering
{
	using System;
	using System.Collections.Generic;
	using Math;
	using Platform.Graphics;
	using Platform.Memory;
	using Scripting;
	using Utilities;

	/// <summary>
	///     Represents a 3D model.
	/// </summary>
	public class Model : DisposableObject
	{
		/// <summary>
		///     The number of indices in the index buffer or the number of primitives of the model, if no index buffer is used.
		/// </summary>
		private readonly int _count;

		/// <summary>
		///     The index buffer containing the model's indices.
		/// </summary>
		private readonly IndexBuffer _indexBuffer;

		/// <summary>
		///     The layout of the vertex buffer.
		/// </summary>
		private readonly VertexLayout _layout;

		/// <summary>
		///     The vertex buffer containing the vertex data of the model.
		/// </summary>
		private readonly VertexBuffer _vertexBuffer;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="vertexBuffer">The vertex buffer containing the vertex data of the model.</param>
		/// <param name="layout">The layout of the vertex buffer.</param>
		/// <param name="indexBuffer">The index buffer containing the model's indices.</param>
		/// <param name="indexCount">The number of indices in the index buffer.</param>
		public Model(VertexBuffer vertexBuffer, VertexLayout layout, IndexBuffer indexBuffer,
					 int indexCount)
		{
			Assert.ArgumentNotNull(vertexBuffer);
			Assert.ArgumentNotNull(layout);
			Assert.ArgumentNotNull(indexBuffer);
			Assert.ArgumentInRange(indexCount, 1, Int32.MaxValue);

			_vertexBuffer = vertexBuffer;
			_indexBuffer = indexBuffer;
			_count = indexCount;
			_layout = layout;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="vertexBuffer">The vertex buffer containing the vertex data of the model.</param>
		/// <param name="layout">The layout of the vertex buffer.</param>
		/// <param name="primitiveCount">The number of primivites of the model.</param>
		public Model(VertexBuffer vertexBuffer, VertexLayout layout, int primitiveCount)
		{
			Assert.ArgumentNotNull(vertexBuffer);
			Assert.ArgumentNotNull(layout);
			Assert.ArgumentInRange(primitiveCount, 1, Int32.MaxValue);

			_vertexBuffer = vertexBuffer;
			_count = primitiveCount;
			_layout = layout;
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_indexBuffer.SafeDispose();
			_vertexBuffer.SafeDispose();
			_layout.SafeDispose();
		}

		/// <summary>
		///     Draws the model.
		/// </summary>
		/// <param name="output">The output the model should be rendered to.</param>
		/// <param name="effect">The effect technique that should be used for rendering.</param>
		public void Draw(RenderOutput output, EffectTechnique effect)
		{
			Assert.ArgumentNotNull(output);

			_layout.Bind();

			if (_indexBuffer == null)
				output.Draw(effect, _count, PrimitiveType.TriangleStrip);
			else
				output.DrawIndexed(effect, _count);
		}

		/// <summary>
		///     Creates a quad with the given width and height, lying in the y = 0 plane.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to draw the quad.</param>
		/// <param name="size">The size of the quad.</param>
		/// <param name="offset">
		///     The offset that should be applied to the generated quad. By default, the center of the quad lies in the origin.
		/// </param>
		public static Model CreateQuad(GraphicsDevice graphicsDevice, Size size, Vector2 offset = default(Vector2))
		{
			return CreateQuad(graphicsDevice, size.Width, size.Height, offset);
		}

		/// <summary>
		///     Creates a quad with the given width and height, lying in the y = 0 plane.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to draw the quad.</param>
		/// <param name="width">The width of the quad.</param>
		/// <param name="height">The height of the quad.</param>
		/// <param name="offset">
		///     The offset that should be applied to the generated quad. By default, the center of the quad lies in the origin.
		/// </param>
		public static Model CreateQuad(GraphicsDevice graphicsDevice, float width, float height, Vector2 offset = default(Vector2))
		{
			Assert.ArgumentNotNull(graphicsDevice);

			var rectangle = new Rectangle(-width / 2.0f + offset.X, -height / 2.0f + offset.Y, width, height);
			var texture = new Rectangle(0, 0, 1, 1);

			var vertices = new[]
			{
				new VertexPositionNormalTexture
				{
					Position = new Vector4(rectangle.Left, 0, rectangle.Bottom),
					Normal = new Vector3(0, 1, 0),
					TextureCoordinates = new Vector2(texture.Left, texture.Top),
				},
				new VertexPositionNormalTexture
				{
					Position = new Vector4(rectangle.Left, 0, rectangle.Top),
					Normal = new Vector3(0, 1, 0),
					TextureCoordinates = new Vector2(texture.Left, texture.Bottom)
				},
				new VertexPositionNormalTexture
				{
					Position = new Vector4(rectangle.Right, 0, rectangle.Bottom),
					Normal = new Vector3(0, 1, 0),
					TextureCoordinates = new Vector2(texture.Right, texture.Top)
				},
				new VertexPositionNormalTexture
				{
					Position = new Vector4(rectangle.Right, 0, rectangle.Top),
					Normal = new Vector3(0, 1, 0),
					TextureCoordinates = new Vector2(texture.Right, texture.Bottom)
				}
			};

			var vertexBuffer = VertexBuffer.Create(graphicsDevice, vertices);
			var layout = VertexPositionNormalTexture.GetInputLayout(graphicsDevice, vertexBuffer);

			return new Model(vertexBuffer, layout, 2);
		}

		/// <summary>
		///     Creates a full-screen quad.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to draw the quad.</param>
		public static Model CreateFullScreenQuad(GraphicsDevice graphicsDevice)
		{
			Assert.ArgumentNotNull(graphicsDevice);

			ushort[] indices;
			int flip;

			// For OpenGL, we have to flip the quad upside-down and change its winding, because OpenGL's window
			// coordinate origins are at the bottom left corner... annoying
			switch (graphicsDevice.GraphicsApi)
			{
				case GraphicsApi.Direct3D11:
					indices = new ushort[] { 0, 1, 2, 0, 2, 3 };
					flip = 1;
					break;
				case GraphicsApi.OpenGL3:
					indices = new ushort[] { 0, 2, 1, 0, 3, 2 };
					flip = -1;
					break;
				default:
					throw new InvalidOperationException("Unsupported graphics API.");
			}

			var texture = new Rectangle(0, 0, 1, 1);
			var vertices = new[]
			{
				new VertexPositionNormalTexture
				{
					Position = new Vector4(-1, -1 * flip, 1),
					Normal = new Vector3(0, 1, 0),
					TextureCoordinates = new Vector2(texture.Left, texture.Bottom)
				},
				new VertexPositionNormalTexture
				{
					Position = new Vector4(-1, 1 * flip, 1),
					Normal = new Vector3(0, 1, 0),
					TextureCoordinates = new Vector2(texture.Left, texture.Top),
				},
				new VertexPositionNormalTexture
				{
					Position = new Vector4(1, 1 * flip, 1),
					Normal = new Vector3(0, 1, 0),
					TextureCoordinates = new Vector2(texture.Right, texture.Top)
				},
				new VertexPositionNormalTexture
				{
					Position = new Vector4(1, -1 * flip, 1),
					Normal = new Vector3(0, 1, 0),
					TextureCoordinates = new Vector2(texture.Right, texture.Bottom)
				}
			};

			var vertexBuffer = VertexBuffer.Create(graphicsDevice, vertices);
			var indexBuffer = IndexBuffer.Create(graphicsDevice, indices);
			var layout = VertexPositionNormalTexture.GetInputLayout(graphicsDevice, vertexBuffer, indexBuffer);

			return new Model(vertexBuffer, layout, indexBuffer, indices.Length);
		}

		/// <summary>
		///     Creates a skybox cube.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to draw the skybox.</param>
		public static unsafe Model CreateSkybox(GraphicsDevice graphicsDevice)
		{
			Assert.ArgumentNotNull(graphicsDevice);
			Assert.That(sizeof(Vector4) == 4 * sizeof(float), "Vector4 has an unexpected unmanaged size.");

			var vertices = new[]
			{
				new Vector4(-1.0f, -1.0f, -1.0f),
				new Vector4(-1.0f, 1.0f, -1.0f),
				new Vector4(1.0f, 1.0f, -1.0f),
				new Vector4(1.0f, -1.0f, -1.0f),
				new Vector4(-1.0f, -1.0f, 1.0f),
				new Vector4(1.0f, -1.0f, 1.0f),
				new Vector4(1.0f, 1.0f, 1.0f),
				new Vector4(-1.0f, 1.0f, 1.0f)
			};

			var indices = new ushort[]
			{
				0, 1, 2, 2, 3, 0, 4, 5, 6,
				6, 7, 4, 0, 3, 5, 5, 4, 0,
				3, 2, 6, 6, 5, 3, 2, 1, 7,
				7, 6, 2, 1, 0, 4, 4, 7, 1
			};

			var vertexBuffer = VertexBuffer.Create(graphicsDevice, vertices);
			var indexBuffer = IndexBuffer.Create(graphicsDevice, indices);

			var inputElements = new[]
			{
				new VertexBinding(vertexBuffer, VertexDataFormat.Vector4, DataSemantics.Position, sizeof(Vector4), 0)
			};

			var layout = new VertexLayout(graphicsDevice, indexBuffer, inputElements);

			return new Model(vertexBuffer, layout, indexBuffer, indices.Length);
		}

		/// <summary>
		///     Creates a model of a sphere with the given radius, using the given subdivision factor to determine the smoothness of
		///     the sphere.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to draw the sphere.</param>
		/// <param name="radius">The radius of the sphere.</param>
		/// <param name="subdivision">The subdivision factor; the higher the value, the smoother the sphere.</param>
		public static Model CreateSphere(GraphicsDevice graphicsDevice, float radius, int subdivision)
		{
			Assert.ArgumentNotNull(graphicsDevice);
			Assert.ArgumentInRange(subdivision, 1, 100);

			// Create a cube and project it into a sphere
			var topLeftFront = new Vector3(-radius, radius, radius);
			var topRightFront = new Vector3(radius, radius, radius);
			var topLeftBack = new Vector3(-radius, radius, -radius);
			var topRightBack = new Vector3(radius, radius, -radius);

			var bottomLeftFront = new Vector3(-radius, -radius, radius);
			var bottomRightFront = new Vector3(radius, -radius, radius);
			var bottomLeftBack = new Vector3(-radius, -radius, -radius);
			var bottomRightBack = new Vector3(radius, -radius, -radius);

			var vertices = new List<VertexPositionNormal>(4069);
			var indices = new List<ushort>(4069);

			// Generates a face of the cube
			Action<Vector3, Vector3, Vector3> generateFace = (topLeft, topRight, bottomLeft) =>
			{
				Assert.That(vertices.Count < UInt16.MaxValue, "Too many vertices.");

				var right = topRight - topLeft;
				var down = bottomLeft - topLeft;
				var delta = right.Length / subdivision;
				var indexOffset = (ushort)vertices.Count;

				right = right.Normalize() * delta;
				down = down.Normalize() * delta;

				// Create the vertices
				for (var i = 0; i <= subdivision; ++i)
				{
					for (var j = 0; j <= subdivision; ++j)
					{
						// Compute the position of the vertex and project it onto the sphere
						var position = topLeft + i * right + j * down;
						var normal = position.Normalize();
						vertices.Add(new VertexPositionNormal(new Vector4(normal * radius), normal));
					}
				}

				// Create the indices
				Assert.That(vertices.Count < UInt16.MaxValue, "Too many vertices.");
				for (var i = 0; i < subdivision; ++i)
				{
					var row = i * (subdivision + 1);
					var nextRow = (i + 1) * (subdivision + 1);

					for (var j = 0; j < subdivision; ++j)
					{
						indices.Add((ushort)(indexOffset + row + j));
						indices.Add((ushort)(indexOffset + nextRow + j));
						indices.Add((ushort)(indexOffset + nextRow + j + 1));
						indices.Add((ushort)(indexOffset + row + j));
						indices.Add((ushort)(indexOffset + nextRow + j + 1));
						indices.Add((ushort)(indexOffset + row + j + 1));
					}
				}
			};

			// Generate the cube faces
			generateFace(topLeftFront, topRightFront, bottomLeftFront); // Front
			generateFace(topRightBack, topLeftBack, bottomRightBack); // Back
			generateFace(topLeftBack, topRightBack, topLeftFront); // Top
			generateFace(bottomLeftFront, bottomRightFront, bottomLeftBack); // Bottom
			generateFace(topLeftBack, topLeftFront, bottomLeftBack); // Left
			generateFace(topRightFront, topRightBack, bottomRightFront); // Right

			// Construct and return the model
			var vertexBuffer = VertexBuffer.Create(graphicsDevice, vertices.ToArray());
			var indexBuffer = IndexBuffer.Create(graphicsDevice, indices.ToArray());
			var layout = VertexPositionNormal.GetInputLayout(graphicsDevice, vertexBuffer, indexBuffer);

			return new Model(vertexBuffer, layout, indexBuffer, indices.Count);
		}
	}
}