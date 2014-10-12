namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets;
	using Assets.Effects;
	using Gameplay.Entities;
	using Pegasus.Assets;
	using Pegasus.Math;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;

	/// <summary>
	///     Renders rays into a 3D scene.
	/// </summary>
	public class RayRenderer : Renderer<Ray>
	{
		/// <summary>
		///     The effect that is used to draw the rays.
		/// </summary>
		private TexturedQuadEffect _effect;

		/// <summary>
		///     The model that is used to draw the rays.
		/// </summary>
		private Model _model;

		private Texture2D _texture, _texture2;

		/// <summary>
		///     Loads the required assets of the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		/// <param name="assets">The assets manager that should be used to load all required assets.</param>
		public override void Load(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			_texture = assets.Load(Textures.Phaser);
			_texture2 = assets.Load(Textures.PhaserGlow);
			_effect = new TexturedQuadEffect(graphicsDevice, assets);
		}

		/// <summary>
		///     Initializes the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		public override void Initialize(GraphicsDevice graphicsDevice)
		{
			_model = Model.CreateQuad(graphicsDevice, _texture.Size, new Vector2(_texture.Width / 2.0f, 0));
		}

		/// <summary>
		///     Draws all rays.
		/// </summary>
		/// <param name="output">The output that the rays should be rendered to.</param>
		public override void Draw(RenderOutput output)
		{
			BlendState.Additive.Bind();
			DepthStencilState.DepthDisabled.Bind();

			foreach (var ray in Elements)
			{
				_effect.Texture = new Texture2DView(_texture2, SamplerState.BilinearClampNoMipmaps);
				_effect.World = Matrix.CreateScale(ray.Length, 1, 1) * ray.Transform.Matrix;
				_effect.Color = new Vector4(1, 0, 0);
				_model.Draw(output, _effect.ColoredTexturedQuad);

				_effect.Texture = new Texture2DView(_texture, SamplerState.BilinearClampNoMipmaps);
				_effect.Color = new Vector4(1, 1, 1);
				_model.Draw(output, _effect.ColoredTexturedQuad);
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposingCore()
		{
			_effect.SafeDispose();
			_model.SafeDispose();
		}
	}
}