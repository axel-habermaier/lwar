using System;

namespace Lwar.Client.Rendering.Renderers
{
	using Assets.Effects;
	using Gameplay.Entities;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Memory;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Renders bullets into a 3D scene.
	/// </summary>
	public class BulletRenderer : Renderer<Bullet>
	{
		/// <summary>
		///   The effect that is used to draw the bullets.
		/// </summary>
		private TexturedQuadEffect _effect;

		/// <summary>
		///   The model that is used to draw the bullets.
		/// </summary>
		private Model _model;

		private Texture2D _texture, _texture2;

		/// <summary>
		///   Initializes the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		/// <param name="assets">The assets manager that should be used to load all required assets.</param>
		public override void Initialize(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice);
			Assert.ArgumentNotNull(assets);

			_texture = assets.LoadTexture2D("Textures/Bullet");
			_texture2 = assets.LoadTexture2D("Textures/BulletGlow");

			_model = Model.CreateQuad(graphicsDevice, _texture.Size);
			_effect = new TexturedQuadEffect(graphicsDevice, assets);
		}

		/// <summary>
		///   Draws all bullets.
		/// </summary>
		/// <param name="output">The output that the bullets should be rendered to.</param>
		public override void Draw(RenderOutput output)
		{
			BlendState.Additive.Bind();
			DepthStencilState.DepthDisabled.Bind();

			foreach (var bullet in Elements)
			{
				_effect.World = bullet.Transform.Matrix;
				_effect.Texture = new Texture2DView(_texture2, SamplerState.BilinearClampNoMipmaps);
				_effect.Color = new Vector4(0, 1, 0, 1);
				_model.Draw(output, _effect.ColoredTexturedQuad);

				_effect.Color = new Vector4(1, 1, 1, 1);
				_effect.Texture = new Texture2DView(_texture, SamplerState.BilinearClampNoMipmaps);
				_model.Draw(output, _effect.ColoredTexturedQuad);
			}
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