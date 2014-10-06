namespace Lwar.Rendering
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using Pegasus;
	using Pegasus.Math;
	using Pegasus.Platform.Graphics;

	/// <summary>
	///     Builds up a model containing circle outlines.
	/// </summary>
	public struct CircleOutline
	{
		/// <summary>
		///     Stores the indices of the generated model.
		/// </summary>
		private List<ushort> _indices;

		/// <summary>
		///     Stores the vertices of the generated model.
		/// </summary>
		private List<Vector3> _vertices;

		/// <summary>
		///     Gets a vertex input layout for drawing planet trajectories with an appropriate vertex buffer and vertex shader.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to construct the input layout.</param>
		/// <param name="vertexBuffer">The vertex buffer that holds the vertex data.</param>
		/// <param name="indexBuffer">The index buffer that holds the vertex indices.</param>
		private static VertexInputLayout GetInputLayout(GraphicsDevice graphicsDevice, VertexBuffer vertexBuffer, IndexBuffer indexBuffer)
		{
			const int stride = 12;
			Assert.ArgumentNotNull(graphicsDevice);
			Assert.ArgumentNotNull(vertexBuffer);
			Assert.That(Marshal.SizeOf(typeof(Vector3)) == stride, "Unexpected unmanaged size.");

			var inputElements = new[]
			{
				new VertexInputBinding(vertexBuffer, VertexDataFormat.Vector3, DataSemantics.Position, stride, 0)
			};

			return new VertexInputLayout(graphicsDevice, indexBuffer, inputElements);
		}

		/// <summary>
		///     Generates the model for the circle outlines that have been added.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to create the model.</param>
		public Model ToModel(GraphicsDevice graphicsDevice)
		{
			Assert.ArgumentNotNull(graphicsDevice);
			Assert.That(_vertices != null, "No circle outlines have been added.");

			var vertexBuffer = VertexBuffer.Create(graphicsDevice, _vertices.ToArray());
			var indexBuffer = IndexBuffer.Create(graphicsDevice, _indices.ToArray());
			var inputLayout = GetInputLayout(graphicsDevice, vertexBuffer, indexBuffer);

			return new Model(vertexBuffer, inputLayout, indexBuffer, _indices.Count);
		}

		/// <summary>
		///     Adds a circle outline to the resulting model. The circle's origin lies in the XZ plane, centered at the origin.
		/// </summary>
		/// <param name="radius">The radius of the circle.</param>
		/// <param name="precision">
		///     The precision that should be used to draw the circle. The given number of individual line elements
		///     are used to draw the outline.
		/// </param>
		/// <param name="width">The width of the circle outline.</param>
		public void Add(float radius, int precision, int width)
		{
			if (_vertices == null)
			{
				_vertices = new List<Vector3>();
				_indices = new List<ushort>();
			}

			var theta = MathUtils.TwoPi / precision;
			var cosine = MathUtils.Cos(theta);
			var sine = MathUtils.Sin(theta);

			var current = new Vector2(radius, 0);
			var startOffset = _vertices.Count;

			for (var i = 0; i < precision; i++)
			{
				var start = current;

				// Calculate the next point
				var t = current.X;
				current.X = cosine * current.X - sine * current.Y;
				current.Y = sine * t + cosine * current.Y;

				var end = current;

				var length = (end - start).Length;
				var topLeft = new Vector3(0, 0, -width);
				var bottomLeft = new Vector3(0, 0, width);
				var topRight = new Vector3(length, 0, -width);
				var bottomRight = new Vector3(length, 0, width);

				// The rotation is computed relative to the unit vector in X direction.
				var rotation = MathUtils.ComputeAngle(start, end, new Vector2(1, 0));

				// Construct the transformation matrix and transform the vertices
				var transformMatrix = Matrix.CreateRotationY(rotation) * Matrix.CreateTranslation(start.X, 0, start.Y + width);
				Vector3.Transform(ref topLeft, ref transformMatrix, out topLeft);
				Vector3.Transform(ref bottomLeft, ref transformMatrix, out bottomLeft);
				Vector3.Transform(ref topRight, ref transformMatrix, out topRight);
				Vector3.Transform(ref bottomRight, ref transformMatrix, out bottomRight);

				// Add the vertices
				var isFirst = i == 0;
				var isLast = i == precision - 1;

				// Determine the indices
				var vertexCount = _vertices.Count;
				var rightOffset = isFirst ? 2 : 0;
				var indexTopLeft = isFirst ? startOffset : vertexCount - 2;
				var indexBottomLeft = isFirst ? startOffset + 1 : vertexCount - 1;
				var indexTopRight = isLast ? startOffset : vertexCount + rightOffset;
				var indexBottomRight = isLast ? startOffset + 1 : vertexCount + 1 + rightOffset;

				// Indices for the first triangle
				_indices.Add((ushort)(indexBottomLeft));
				_indices.Add((ushort)(indexTopLeft));
				_indices.Add((ushort)(indexTopRight));

				// Indices for the second triangle
				_indices.Add((ushort)(indexBottomLeft));
				_indices.Add((ushort)(indexTopRight));
				_indices.Add((ushort)(indexBottomRight));

				// Reuse the vertices defining the right edge of the previous element
				if (isFirst)
				{
					_vertices.Add(topLeft);
					_vertices.Add(bottomLeft);
				}

				// Reuse the vertices defining the left edge of the first element
				if (isLast)
					continue;

				_vertices.Add(topRight);
				_vertices.Add(bottomRight);
			}
		}
	}
}