using System;

namespace Lwar.Rendering.Renderers
{
	using Assets.Effects;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Memory;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Renders a parallax scrolling effect.
	/// </summary>
	public class ParallaxRenderer : DisposableObject
	{
		/// <summary>
		///   The number of stars that are rendered.
		/// </summary>
		private const int StarCount = 8192;

		/// <summary>
		///   The maximum allowed distance of the generated stars from the origin in world units.
		/// </summary>
		private const int Range = 16384;

		/// <summary>
		///   The number of different star textures in the texture atlas.
		/// </summary>
		private const int StarTypeCount = 8;

		/// <summary>
		///   The Y-coordinate of the stars.
		/// </summary>
		private const float StarLayer = -2000.0f;

		/// <summary>
		///   The allowed range of the star's relative distance to the viewer.
		/// </summary>
		private static readonly Vector2 DistanceRange = new Vector2(0.1f, 0.8f);

		/// <summary>
		///   The effect that is used to draw the parallax scrolling star field.
		/// </summary>
		private readonly ParallaxEffect _effect;

		/// <summary>
		///   The model that stores the stars.
		/// </summary>
		private readonly Model _model;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that is used to draw the game session.</param>
		/// <param name="assets">The assets manager that manages all assets of the game session.</param>
		public ParallaxRenderer(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice);
			Assert.ArgumentNotNull(assets);

			_effect = new ParallaxEffect(graphicsDevice, assets)
			{
				TextureAtlas = new Texture2DView(assets.LoadTexture2D("Textures/Parallax"), SamplerState.BilinearWrapNoMipmaps)
			};

			var random = new Random();
			var vertices = new VertexPositionNormal[StarCount * 4];
			var indices = new ushort[StarCount * 6];

			var width = _effect.TextureAtlas.Texture.Width / StarTypeCount;
			var scaledWidth = width / (float)_effect.TextureAtlas.Texture.Width;
			var height = _effect.TextureAtlas.Texture.Height;

			// Generate the texture coordinates
			var texCoords = new RectangleF[StarTypeCount];
			for (var i = 0; i < StarTypeCount; ++i)
				texCoords[i] = new RectangleF(i * scaledWidth, 0, scaledWidth, 1);

			// Generate the stars
			var idx = 0;
			ushort vertexIndex = 0;
			for (var i = 0; i < StarCount; ++i)
			{
				// Randomly place a star, selecting a random distance to the viewer and a random star type
				var position = new Vector2(random.Next(-Range, Range), random.Next(-Range, Range));
				var distance = MathUtils.Clamp((float)random.NextDouble(), DistanceRange.X, DistanceRange.Y);
				var textureIdx = random.Next(0, StarTypeCount);

				// Store the positions of the vertices of the star
				var rectangle = new RectangleF(position, width * (1 - distance), height * (1 - distance));
				vertices[vertexIndex + 0].Position = new Vector4(rectangle.Left, StarLayer, rectangle.Bottom);
				vertices[vertexIndex + 1].Position = new Vector4(rectangle.Right, StarLayer, rectangle.Bottom);
				vertices[vertexIndex + 2].Position = new Vector4(rectangle.Left, StarLayer, rectangle.Top);
				vertices[vertexIndex + 3].Position = new Vector4(rectangle.Right, StarLayer, rectangle.Top);

				// Store the stars texture coordinates
				var texture = texCoords[textureIdx];
				vertices[vertexIndex + 0].Normal = new Vector3(texture.Left, texture.Top, distance);
				vertices[vertexIndex + 1].Normal = new Vector3(texture.Right, texture.Top, distance);
				vertices[vertexIndex + 2].Normal = new Vector3(texture.Left, texture.Bottom, distance);
				vertices[vertexIndex + 3].Normal = new Vector3(texture.Right, texture.Bottom, distance);

				// Indices for the first triangle of the star
				indices[idx++] = (ushort)(vertexIndex + 0);
				indices[idx++] = (ushort)(vertexIndex + 2);
				indices[idx++] = (ushort)(vertexIndex + 1);

				// Indices for the second triangle of the star
				indices[idx++] = (ushort)(vertexIndex + 3);
				indices[idx++] = (ushort)(vertexIndex + 1);
				indices[idx++] = (ushort)(vertexIndex + 2);

				vertexIndex += 4;
			}

			// Generate the model
			var vertexBuffer = VertexBuffer.Create(graphicsDevice, vertices);
			var indexBuffer = IndexBuffer.Create(graphicsDevice, indices);
			var inputLayout = VertexPositionNormal.GetInputLayout(graphicsDevice, vertexBuffer, indexBuffer);

			_model = new Model(vertexBuffer, inputLayout, indexBuffer, indices.Length);
		}

		/// <summary>
		///   Draws the skybox.
		/// </summary>
		/// <param name="output">The output that the bullets should be rendered to.</param>
		public void Draw(RenderOutput output)
		{
			var camera = output.Camera as GameCamera;
			if (camera == null)
				return;

			BlendState.Additive.Bind();
			camera.ZoomMode = ZoomMode.Starfield;
			_model.Draw(output, _effect.Default);
			camera.ZoomMode = ZoomMode.Default;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_effect.SafeDispose();
			_model.SafeDispose();
		}
	}
}