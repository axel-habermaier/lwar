namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets;
	using Gameplay.Client.Entities;
	using Pegasus.Platform.Graphics;
	using Pegasus.Rendering;

	/// <summary>
	///     Renders ships into a 3D scene.
	/// </summary>
	internal class ShipRenderer : Renderer<ShipEntity>
	{
		/// <summary>
		///     The texture that is used to draw the ship.
		/// </summary>
		private Texture2D _texture;

		/// <summary>
		///     Initializes the renderer.
		/// </summary>
		/// <param name="renderContext">The render context that should be used for drawing.</param>
		/// <param name="assets">The asset bundle that provides access to Lwar assets.</param>
		public override void Initialize(RenderContext renderContext, GameBundle assets)
		{
			_texture = assets.Ship;
		}

		/// <summary>
		///     Draws all registered 2D elements.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the 2D elements.</param>
		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.BlendState = spriteBatch.RenderContext.BlendStates.Premultiplied;
			spriteBatch.DepthStencilState = spriteBatch.RenderContext.DepthStencilStates.DepthDisabled;

			foreach (var ship in Elements)
				spriteBatch.Draw(ship.Position, _texture.Size, _texture, Colors.White, -ship.Orientation);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposingCore()
		{
		}
	}
}