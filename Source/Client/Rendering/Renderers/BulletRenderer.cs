using System;

namespace Lwar.Client.Rendering.Renderers
{
	using Assets.Effects;
	using Gameplay.Entities;
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
		protected override void Initialize()
		{
			_texture = Assets.LoadTexture2D("Textures/Bullet");
			_texture2 = Assets.LoadTexture2D("Textures/BulletGlow");

			_model = Model.CreateQuad(GraphicsDevice, _texture.Size);
			_effect = new TexturedQuadEffect(GraphicsDevice, Assets);
		}

		/// <summary>
		///   Draws all registered 2D elements.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the 2D elements.</param>
		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.BlendState = BlendState.Additive;
			spriteBatch.DepthStencilState = DepthStencilState.DepthRead;

			foreach (var bullet in Elements)
			{
				// TODO: Remove this hack
				if (!bullet.IsValid)
					return;

				spriteBatch.Draw(bullet.Position, _texture.Size, _texture2, new Color(0, 255, 0, 255), -bullet.Rotation);
			}

			foreach (var bullet in Elements)
			{
				// TODO: Remove this hack
				if (!bullet.IsValid)
					return;

				spriteBatch.Draw(bullet.Position, _texture.Size, _texture, Color.White, -bullet.Rotation);
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