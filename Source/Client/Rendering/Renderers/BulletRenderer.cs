using System;

namespace Lwar.Client.Rendering.Renderers
{
	using Assets.Effects;
	using Gameplay.Entities;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
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

		/// <summary>
		///   Initializes the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		/// <param name="assets">The assets manager that should be used to load all required assets.</param>
		public override void Initialize(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(assets, () => assets);

			var texture = assets.LoadTexture2D("Textures/Bullet");

			_model = Model.CreateQuad(graphicsDevice, texture.Size);
			_effect = new TexturedQuadEffect(graphicsDevice, assets) { Texture = new Texture2DView(texture, SamplerState.TrilinearClamp) };
		}

		/// <summary>
		///   Draws all bullets.
		/// </summary>
		/// <param name="output">The output that the bullets should be rendered to.</param>
		public override void Draw(RenderOutput output)
		{
			foreach (var bullet in Elements)
			{
				_effect.World = bullet.Transform.Matrix;
				_model.Draw(output, _effect.TexturedQuad);
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