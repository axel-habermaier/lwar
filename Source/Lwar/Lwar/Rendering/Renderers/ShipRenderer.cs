namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets;
	using Gameplay.Entities;
	using Pegasus.Assets;
	using Pegasus.Platform.Graphics;
	using Pegasus.Rendering;

	/// <summary>
	///     Renders ships into a 3D scene.
	/// </summary>
	public class ShipRenderer : Renderer<Ship>
	{
		/// <summary>
		///     The texture that is used to draw the ship.
		/// </summary>
		private Texture2D _texture;

		/// <summary>
		///     Loads the required assets of the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		/// <param name="assets">The assets manager that should be used to load all required assets.</param>
		public override void Load(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			_texture = assets.Load(Textures.Ship);
		}

		/// <summary>
		///     Draws all registered 2D elements.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the 2D elements.</param>
		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.BlendState = BlendState.Premultiplied;
			spriteBatch.DepthStencilState = DepthStencilState.DepthDisabled;

			foreach (var ship in Elements)
				spriteBatch.Draw(ship.Position, _texture.Size, _texture, Colors.White, -ship.Rotation);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposingCore()
		{
		}
	}
}