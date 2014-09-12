namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets;
	using Assets.Effects;
	using Gameplay.Entities;
	using Pegasus.Assets;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;

	/// <summary>
	///     Renders rockets into a 3D scene.
	/// </summary>
	public class RocketRenderer : Renderer<Rocket>
	{
		/// <summary>
		///     The effect that is used to draw the rockets.
		/// </summary>
		private TexturedQuadEffect _effect;

		/// <summary>
		///     The model that is used to draw the rockets.
		/// </summary>
		private Model _model;

		private Texture2D _texture;

		/// <summary>
		///     Loads the required assets of the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		/// <param name="assets">The assets manager that should be used to load all required assets.</param>
		public override void Load(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			_texture = assets.Load(Textures.Rocket);
			_effect = new TexturedQuadEffect(graphicsDevice, assets);
		}

		/// <summary>
		///     Initializes the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		public override void Initialize(GraphicsDevice graphicsDevice)
		{
			_model = Model.CreateQuad(graphicsDevice, _texture.Size);
			_effect.Texture = new Texture2DView(_texture, SamplerState.TrilinearClamp);
		}

		/// <summary>
		///     Draws all rockets.
		/// </summary>
		/// <param name="output">The output that the rockets should be rendered to.</param>
		public override void Draw(RenderOutput output)
		{
			BlendState.Premultiplied.Bind();
			DepthStencilState.DepthDisabled.Bind();

			foreach (var rocket in Elements)
			{
				_effect.World = rocket.Transform.Matrix;
				_model.Draw(output, _effect.TexturedQuad);
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