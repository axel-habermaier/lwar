namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets;
	using Assets.Effects;
	using Gameplay.Client.Entities;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;

	/// <summary>
	///     Renders rockets into a 3D scene.
	/// </summary>
	internal class RocketRenderer : Renderer<RocketEntity>
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
		///     Initializes the renderer.
		/// </summary>
		/// <param name="renderContext">The render context that should be used for drawing.</param>
		/// <param name="assets">The asset bundle that provides access to Lwar assets.</param>
		public override void Initialize(RenderContext renderContext, GameBundle assets)
		{
			_texture = assets.Rocket;
			_effect = assets.TexturedQuadEffect;

			_model = Model.CreateQuad(renderContext.GraphicsDevice, _texture.Size);
		}

		/// <summary>
		///     Draws all rockets.
		/// </summary>
		/// <param name="output">The output that the rockets should be rendered to.</param>
		public override void Draw(RenderOutput output)
		{
			output.RenderContext.BlendStates.Premultiplied.Bind();
			output.RenderContext.DepthStencilStates.DepthDisabled.Bind();
			_effect.Texture = new Texture2DView(_texture, output.RenderContext.SamplerStates.TrilinearClamp);

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
			_model.SafeDispose();
		}
	}
}