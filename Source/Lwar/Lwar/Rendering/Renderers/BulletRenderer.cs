namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets;
	using Gameplay.Client.Entities;
	using Pegasus.Math;
	using Pegasus.Platform.Graphics;
	using Pegasus.Rendering;

	/// <summary>
	///     Renders bullets into a 3D scene.
	/// </summary>
	internal class BulletRenderer : Renderer<BulletEntity>
	{
		private Texture2D _texture, _texture2;

		/// <summary>
		///     Initializes the renderer.
		/// </summary>
		/// <param name="renderContext">The render context that should be used for drawing.</param>
		/// <param name="assets">The asset bundle that provides access to Lwar assets.</param>
		public override void Initialize(RenderContext renderContext, GameBundle assets)
		{
			_texture = assets.Bullet;
			_texture2 = assets.BulletGlow;
		}

		/// <summary>
		///     Draws all registered 2D elements.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the 2D elements.</param>
		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.BlendState = spriteBatch.RenderContext.BlendStates.Additive;
			spriteBatch.DepthStencilState = spriteBatch.RenderContext.DepthStencilStates.DepthRead;

			foreach (var bullet in Elements)
			{
				var rectangle = new Rectangle(bullet.Position.X - _texture.Width / 2.0f,
					bullet.Position.Y - _texture.Height / 2.0f,
					_texture.Width, _texture.Height);
				spriteBatch.Draw(rectangle, _texture2, new Color(0, 255, 0, 255));
			}

			foreach (var bullet in Elements)
			{
				var rectangle = new Rectangle(bullet.Position.X - _texture.Width / 2.0f,
					bullet.Position.Y - _texture.Height / 2.0f,
					_texture.Width, _texture.Height);
				spriteBatch.Draw(rectangle, _texture, Colors.White);
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposingCore()
		{
			// Nothing to do here
		}
	}
}