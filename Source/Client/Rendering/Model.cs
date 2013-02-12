﻿using System;

namespace Lwar.Client.Rendering
{
	using System.Collections.Generic;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Represents a 3D model.
	/// </summary>
	public class Model : DisposableObject
	{
		/// <summary>
		///   The graphics device that is used to draw the model.
		/// </summary>
		private readonly GraphicsDevice _graphicsDevice;

		/// <summary>
		///   The index buffer containing the model's indices.
		/// </summary>
		private readonly IndexBuffer _indexBuffer;

		/// <summary>
		///   The number of indices in the index buffer.
		/// </summary>
		private readonly int _indexCount;

		/// <summary>
		///   The layout of the vertex buffer.
		/// </summary>
		private readonly VertexInputLayout _layout;

		/// <summary>
		///   The vertex buffer containing the vertex data of the model.
		/// </summary>
		private readonly VertexBuffer _vertexBuffer;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to draw the model.</param>
		/// <param name="vertexBuffer">The vertex buffer containing the vertex data of the model.</param>
		/// <param name="layout">The layout of the vertex buffer.</param>
		/// <param name="indexBuffer">The index buffer containing the model's indices.</param>
		/// <param name="indexCount">The number of indices in the index buffer.</param>
		public Model(GraphicsDevice graphicsDevice, VertexBuffer vertexBuffer, VertexInputLayout layout, IndexBuffer indexBuffer,
					 int indexCount)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(vertexBuffer, () => vertexBuffer);
			Assert.ArgumentNotNull(layout, () => layout);
			Assert.ArgumentNotNull(indexBuffer, () => indexBuffer);
			Assert.ArgumentInRange(indexCount, () => indexCount, 0, Int32.MaxValue);

			_graphicsDevice = graphicsDevice;
			_vertexBuffer = vertexBuffer;
			_indexBuffer = indexBuffer;
			_indexCount = indexCount;
			_layout = layout;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_indexBuffer.SafeDispose();
			_vertexBuffer.SafeDispose();
			_layout.SafeDispose();
		}

		/// <summary>
		///   Draws the model.
		/// </summary>
		public void Draw()
		{
			_layout.Bind();
			_graphicsDevice.SetPrimitiveType(PrimitiveType.Triangles);
			_graphicsDevice.DrawIndexed(_indexCount);
		}

		/// <summary>
		///   Creates a quad with the given width and height, lying in the y = 0 plane.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to draw the quad.</param>
		/// <param name="size">The size of the quad.</param>
		public static Model CreateQuad(GraphicsDevice graphicsDevice, Size size)
		{
			return CreateQuad(graphicsDevice, size.Width, size.Height);
		}

		/// <summary>
		///   Creates a quad with the given width and height, lying in the y = 0 plane.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to draw the quad.</param>
		/// <param name="width">The width of the quad.</param>
		/// <param name="height">The height of the quad.</param>
		public static Model CreateQuad(GraphicsDevice graphicsDevice, float width, float height)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentInRange(width, () => width, 0, Single.MaxValue);
			Assert.ArgumentInRange(height, () => height, 0, Single.MaxValue);

			var vertices = new VertexPositionNormalTexture[4];
			var indices = new ushort[6];

			width /= 2.0f;
			height /= 2.0f;
			vertices[0].Position = new Vector4(-width, 0, -width);
			vertices[1].Position = new Vector4(-width, 0, width);
			vertices[2].Position = new Vector4(width, 0, width);
			vertices[3].Position = new Vector4(width, 0, -width);

			vertices[0].Normal = new Vector3(0, 1, 0);
			vertices[1].Normal = new Vector3(0, 1, 0);
			vertices[2].Normal = new Vector3(0, 1, 0);
			vertices[3].Normal = new Vector3(0, 1, 0);

			var texture = new RectangleF(0, 0, 1, 1);
			vertices[0].TextureCoordinates = new Vector2(texture.Left, texture.Bottom);
			vertices[1].TextureCoordinates = new Vector2(texture.Left, texture.Top);
			vertices[2].TextureCoordinates = new Vector2(texture.Right, texture.Top);
			vertices[3].TextureCoordinates = new Vector2(texture.Right, texture.Bottom);

			indices[0] = 0;
			indices[1] = 2;
			indices[2] = 1;
			indices[3] = 0;
			indices[4] = 3;
			indices[5] = 2;

			var vertexBuffer = VertexBuffer.Create(graphicsDevice, vertices);
			var indexBuffer = IndexBuffer.Create(graphicsDevice, indices);
			var layout = VertexPositionNormalTexture.GetInputLayout(graphicsDevice, vertexBuffer, indexBuffer);

			return new Model(graphicsDevice, vertexBuffer, layout, indexBuffer, indices.Length);
		}

		/// <summary>
		///   Creates a model of a sphere with the given radius, using the given subdivision factor to determine the smoothness of
		///   the sphere.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to draw the sphere.</param>
		/// <param name="radius">The radius of the sphere.</param>
		/// <param name="subdivision">The subdivision factor; the higher the value, the smoother the sphere.</param>
		public static Model CreateSphere(GraphicsDevice graphicsDevice, float radius, int subdivision)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentInRange(subdivision, () => subdivision, 1, 100);

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

			return new Model(graphicsDevice, vertexBuffer, layout, indexBuffer, indices.Count);
		}
	}
}