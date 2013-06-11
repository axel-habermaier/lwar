using System;

namespace Lwar.Client.Rendering.Renderers
{
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Represents a renderer that renders an object into a 3D scene.
	/// </summary>
	public interface IRenderer : IDisposable
	{
		/// <summary>
		///   Initializes the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		/// <param name="assets">The assets manager that should be used to load all required assets.</param>
		void Initialize(GraphicsDevice graphicsDevice, AssetsManager assets);

		/// <summary>
		///   Draws all registered 3D elements.
		/// </summary>
		/// <param name="output">The output that the bullets should be rendered to.</param>
		void Draw(RenderOutput output);

		/// <summary>
		///   Draws all registered 2D elements.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the 2D elements.</param>
		void Draw(SpriteBatch spriteBatch);
	}
}