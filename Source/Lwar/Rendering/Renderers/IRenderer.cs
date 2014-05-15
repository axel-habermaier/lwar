﻿namespace Lwar.Rendering.Renderers
{
	using System;
	using Pegasus.Assets;
	using Pegasus.Platform.Graphics;
	using Pegasus.Rendering;

	/// <summary>
	///     Represents a renderer that renders an object into a 3D scene.
	/// </summary>
	public interface IRenderer : IDisposable
	{
		/// <summary>
		///     Initializes the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		/// <param name="assets">The assets manager that should be used to load all required assets.</param>
		void Initialize(GraphicsDevice graphicsDevice, AssetsManager assets);

		/// <summary>
		///     Draws all registered 3D elements.
		/// </summary>
		/// <param name="output">The output that the elements should be rendered to.</param>
		void Draw(RenderOutput output);

		/// <summary>
		///     Draws all registered 2D elements.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the 2D elements.</param>
		void Draw(SpriteBatch spriteBatch);

		/// <summary>
		///     Draws the user interface elements.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the user interface.</param>
		/// <param name="camera">The camera that is used to draw the scene.</param>
		void DrawUserInterface(SpriteBatch spriteBatch, GameCamera camera);
	}
}