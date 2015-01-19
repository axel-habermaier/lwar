namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets;
	using Assets.Effects;
	using Pegasus.Math;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;

	/// <summary>
	///     Renders a starfield as a parallax scrolling effect.
	/// </summary>
	internal class StarfieldRenderer : DisposableObject, IRenderer
	{
		/// <summary>
		///     The number of stars that are rendered.
		/// </summary>
		private const int StarCount = 8192;

		/// <summary>
		///     The maximum allowed distance of the generated stars from the origin in world units.
		/// </summary>
		private const int Range = 16384;

		/// <summary>
		///     The number of different star textures in the texture atlas.
		/// </summary>
		private const int StarTypeCount = 8;

		/// <summary>
		///     The Y-coordinate of the stars.
		/// </summary>
		private const float StarLayer = -2000.0f;

		/// <summary>
		///     The allowed range of the star's relative distance to the viewer.
		/// </summary>
		private static readonly Vector2 DistanceRange = new Vector2(0.1f, 0.8f);

		/// <summary>
		///     The effect that is used to draw the parallax scrolling star field.
		/// </summary>
		private ParallaxEffect _effect;

		/// <summary>
		///     The model that stores the stars.
		/// </summary>
		private Model _model;

		/// <summary>
		///     The texture atlas containing the stars.
		/// </summary>
		private Texture2D _texture;

		/// <summary>
		///     Initializes the renderer.
		/// </summary>
		/// <param name="renderContext">The render context that should be used for drawing.</param>
		/// <param name="assets">The asset bundle that provides access to Lwar assets.</param>
		public void Initialize(RenderContext renderContext, GameBundle assets)
		{
			_effect = assets.ParallaxEffect;
			_texture = assets.Parallax;

			var random = new Random();
			var vertices = new VertexPositionNormal[StarCount * 4];
			var indices = new ushort[StarCount * 6];

			var width = _texture.Width / StarTypeCount;
			var scaledWidth = width / (float)_texture.Width;
			var height = _texture.Height;

			// Generate the texture coordinates
			var texCoords = new Rectangle[StarTypeCount];
			for (var i = 0; i < StarTypeCount; ++i)
				texCoords[i] = new Rectangle(i * scaledWidth, 0, scaledWidth, 1);

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
				var rectangle = new Rectangle(position, width * (1 - distance), height * (1 - distance));
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
			var vertexBuffer = VertexBuffer.Create(renderContext.GraphicsDevice, vertices);
			var indexBuffer = IndexBuffer.Create(renderContext.GraphicsDevice, indices);
			var inputLayout = VertexPositionNormal.GetInputLayout(renderContext.GraphicsDevice, vertexBuffer, indexBuffer);

			_model = new Model(vertexBuffer, inputLayout, indexBuffer, indices.Length);
		}

		/// <summary>
		///     Draws the skybox.
		/// </summary>
		/// <param name="output">The output that the bullets should be rendered to.</param>
		public void Draw(RenderOutput output)
		{
			var camera = output.Camera as GameCamera;
			if (camera == null)
				return;

			output.RenderContext.RasterizerStates.CullCounterClockwise.Bind();
			output.RenderContext.BlendStates.Additive.Bind();

			camera.ZoomMode = ZoomMode.Starfield;
			_effect.TextureAtlas = new Texture2DView(_texture, output.RenderContext.SamplerStates.BilinearWrapNoMipmaps);
			_model.Draw(output, _effect.Default);
			camera.ZoomMode = ZoomMode.Default;
		}

		/// <summary>
		///     Draws all registered 2D elements.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the 2D elements.</param>
		public void Draw(SpriteBatch spriteBatch)
		{
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_model.SafeDispose();
		}
	}
}