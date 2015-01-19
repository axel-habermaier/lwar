namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets;
	using Pegasus.Rendering;

	/// <summary>
	///     Represents a renderer that renders an object into a 3D scene.
	/// </summary>
	internal interface IRenderer : IDisposable
	{
		/// <summary>
		///     Initializes the renderer.
		/// </summary>
		/// <param name="renderContext">The render context that should be used for drawing.</param>
		/// <param name="assets">The asset bundle that provides access to Lwar assets.</param>
		void Initialize(RenderContext renderContext, GameBundle assets);

		/// <summary>
		///     Draws the 3D content of the renderer.
		/// </summary>
		/// <param name="output">The output that the elements should be rendered to.</param>
		void Draw(RenderOutput output);

		/// <summary>
		///     Draws the 2D content of the renderer.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the 2D elements.</param>
		void Draw(SpriteBatch spriteBatch);
	}
}