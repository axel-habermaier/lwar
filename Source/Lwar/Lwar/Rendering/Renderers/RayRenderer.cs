namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets;
	using Assets.Effects;
	using Gameplay.Client.Entities;
	using Pegasus.Math;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;

	/// <summary>
	///     Renders rays into a 3D scene.
	/// </summary>
	internal class RayRenderer : Renderer<RayEntity>
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
		///     Initializes the renderer.
		/// </summary>
		/// <param name="renderContext">The render context that should be used for drawing.</param>
		/// <param name="assets">The asset bundle that provides access to Lwar assets.</param>
		public override void Initialize(RenderContext renderContext, GameBundle assets)
		{
			_texture = assets.Phaser;
			_texture2 = assets.PhaserGlow;
			_effect = assets.TexturedQuadEffect;
			_model = Model.CreateQuad(renderContext.GraphicsDevice, _texture.Size, new Vector2(_texture.Width / 2.0f, 0));
		}

		/// <summary>
		///     Draws all rays.
		/// </summary>
		/// <param name="output">The output that the rays should be rendered to.</param>
		public override void Draw(RenderOutput output)
		{
			output.RenderContext.BlendStates.Additive.Bind();
			output.RenderContext.DepthStencilStates.DepthDisabled.Bind();

			foreach (var ray in Elements)
			{
				_effect.Texture = new Texture2DView(_texture2, output.RenderContext.SamplerStates.BilinearClampNoMipmaps);
				_effect.World = Matrix.CreateScale(ray.Length, 1, 1) * ray.Transform.Matrix;
				_effect.Color = new Vector4(1, 0, 0);
				_model.Draw(output, _effect.ColoredTexturedQuad);

				_effect.Texture = new Texture2DView(_texture, output.RenderContext.SamplerStates.BilinearClampNoMipmaps);
				_effect.Color = new Vector4(1, 1, 1);
				_model.Draw(output, _effect.ColoredTexturedQuad);
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