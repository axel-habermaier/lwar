using System;

namespace Pegasus.Framework.Rendering
{
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using Math;
	using Platform;
	using Platform.Assets;
	using Platform.Graphics;

	/// <summary>
	///   Represents a renderable, three-dimensional sphere.
	/// </summary>
	public class Sphere : DisposableObject
	{
		private readonly FragmentShader _fragmentShader;
		private readonly IndexBuffer _indexBuffer;

		private readonly List<uint> _indices = new List<uint>();

		private readonly VertexInputLayout _layout;

		private readonly ConstantBuffer _projection;
		private readonly RasterizerState _rasterizerState;
		private readonly VertexBuffer _vertexBuffer;
		private readonly VertexShader _vertexShader;
		private readonly List<VertexPositionNormalTexture> _vertices = new List<VertexPositionNormalTexture>();
		private readonly ConstantBuffer _world;

		private float r;

		public Sphere(GraphicsDevice device, AssetsManager assets, float radius)
		{
			var topLeftFront = new Vector3(-radius, radius, radius);
			var topRightFront = new Vector3(radius, radius, radius);
			var topLeftBack = new Vector3(-radius, radius, -radius);
			var topRightBack = new Vector3(radius, radius, -radius);

			var bottomLeftFront = new Vector3(-radius, -radius, radius);
			var bottomRightFront = new Vector3(radius, -radius, radius);
			var bottomLeftBack = new Vector3(-radius, -radius, -radius);
			var bottomRightBack = new Vector3(radius, -radius, -radius);

			var subdivide = 200;

			// Front
			AddFace(topLeftFront, topRightFront, bottomLeftFront, bottomRightFront, subdivide);

			// Back
			AddFace(topRightBack, topLeftBack, bottomRightBack, bottomLeftBack, subdivide);

			// Top
			AddFace(topLeftBack, topRightBack, topLeftFront, topRightFront, subdivide);

			// Bottom
			AddFace(bottomLeftFront, bottomRightFront, bottomLeftBack, bottomRightBack, subdivide);

			// Left
			AddFace(topLeftBack, topLeftFront, bottomLeftBack, bottomLeftFront, subdivide);

			// Right
			AddFace(topRightFront, topRightBack, bottomRightFront, bottomRightBack, subdivide);

			for (var i = 0; i < _vertices.Count; ++i)
			{
				var pos = _vertices[i].Position;
				var pos3 = new Vector3(pos.X, pos.Y, pos.Z);
				_vertices[i] = new VertexPositionNormalTexture { Position = new Vector4(pos3.Normalize() * radius)};
			}

			_vertexBuffer = VertexBuffer.Create(device, _vertices.ToArray());
			_indexBuffer = IndexBuffer.Create(device, _indices.ToArray());

			_rasterizerState = new RasterizerState(device) { CullMode = CullMode.None, FillMode = FillMode.Wireframe };
			_vertexShader = assets.LoadVertexShader("Shaders/SphereVS");
			_fragmentShader = assets.LoadFragmentShader("Shaders/SphereFS");

			_layout = VertexPositionNormalTexture.GetInputLayout(device, _vertexBuffer, _indexBuffer);
			_projection = ConstantBuffer.Create(device, Matrix.Identity);
			_world = ConstantBuffer.Create(device, Matrix.Identity);
		}

		public void Draw(GraphicsDevice device)
		{
			_layout.Bind();
			_rasterizerState.Bind();
			_fragmentShader.Bind();
			_vertexShader.Bind();

			var m = Matrix.CreatePerspective(-640, 640, 360, -360, 1, -1000);
			m.UsePointer(p => _projection.SetData(p, Marshal.SizeOf(m)));

			m = Matrix.CreateRotationX(r += 0.0001f);
			m *= Matrix.CreateTranslation(0, 0, -102);
			m.UsePointer(p => _world.SetData(p, Marshal.SizeOf(m)));

			_projection.Bind(0);
			_world.Bind(1);

			device.DrawIndexed(_indices.Count);
		}

		private void AddFace(Vector3 topLeft, Vector3 topRight, Vector3 bottomLeft, Vector3 bottomRight, int subdivide)
		{
			var x = topRight - topLeft;
			var y = bottomLeft - topLeft;
			var delta = x.Length / subdivide;

			x = x.Normalize() * delta;
			y = y.Normalize() * delta;

			for (var i = 0; i < subdivide; ++i)
			{
				for (var j = 0; j < subdivide; ++j)
				{
					AddQuad(topLeft + i * x + j * y, topLeft + (i + 1) * x + j * y,
							topLeft + i * x + (j + 1) * y, topLeft + (i + 1) * x + (j + 1) * y);
				}
			}
		}

		private void AddQuad(Vector3 topLeft, Vector3 topRight, Vector3 bottomLeft, Vector3 bottomRight)
		{
			var index = _vertices.Count;
			_vertices.Add(new VertexPositionNormalTexture(new Vector4(topLeft), Vector3.Zero, Vector2.Zero));
			_vertices.Add(new VertexPositionNormalTexture(new Vector4(topRight), Vector3.Zero, Vector2.Zero));
			_vertices.Add(new VertexPositionNormalTexture(new Vector4(bottomLeft), Vector3.Zero, Vector2.Zero));
			_vertices.Add(new VertexPositionNormalTexture(new Vector4(bottomRight), Vector3.Zero, Vector2.Zero));

			_indices.Add((uint)index);
			_indices.Add((uint)(index + 1));
			_indices.Add((uint)(index + 2));
			_indices.Add((uint)(index + 1));
			_indices.Add((uint)(index + 3));
			_indices.Add((uint)(index + 2));
		}

		protected override void OnDisposing()
		{
			_indexBuffer.SafeDispose();
			_vertexBuffer.SafeDispose();
			_vertexShader.SafeDispose();
			_fragmentShader.SafeDispose();
			_rasterizerState.SafeDispose();
			_layout.SafeDispose();
			_projection.SafeDispose();
			_world.SafeDispose();
		}
	}
}