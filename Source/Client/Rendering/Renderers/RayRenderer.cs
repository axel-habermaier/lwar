using System;

namespace Lwar.Client.Rendering.Renderers
{
	using Assets.Effects;
	using Gameplay.Entities;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Memory;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Renders rays into a 3D scene.
	/// </summary>
	public class RayRenderer : Renderer<Ray>
	{
		/// <summary>
		///   The effect that is used to draw the rays.
		/// </summary>
		private TexturedQuadEffect _effect;

		/// <summary>
		///   The model that is used to draw the rays.
		/// </summary>
		private Model _model;

		private Texture2D _texture, _texture2;

		/// <summary>
		///   Initializes the renderer.
		/// </summary>
		protected override void Initialize()
		{
			_texture = Assets.LoadTexture2D("Textures/Phaser");
			_texture2 = Assets.LoadTexture2D("Textures/PhaserGlow");

			_model = Model.CreateQuad(GraphicsDevice, _texture.Size, new Vector2(_texture.Size.Width / 2.0f, 0));
			_effect = new TexturedQuadEffect(GraphicsDevice, Assets);
		}

		/// <summary>
		///   Draws all rays.
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
				_effect.Color = new Vector4(1, 0, 0, 1);
				_model.Draw(output, _effect.ColoredTexturedQuad);

				_effect.Texture = new Texture2DView(_texture, SamplerState.BilinearClampNoMipmaps);
				_effect.Color = new Vector4(1, 1, 1, 1);
				_model.Draw(output, _effect.ColoredTexturedQuad);
			}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposingCore()
		{
			_effect.SafeDispose();
			_model.SafeDispose();
		}
	}
}