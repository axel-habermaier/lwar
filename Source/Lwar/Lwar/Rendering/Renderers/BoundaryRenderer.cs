namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets;
	using Assets.Effects;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;

	/// <summary>
	///     Renders the world boundary.
	/// </summary>
	internal class BoundaryRenderer : DisposableObject, IRenderer
	{
		/// <summary>
		///     The effect that is used to draw the level boundary.
		/// </summary>
		private SimpleVertexEffect _effect;

		/// <summary>
		///     The model representing the boundary of the level.
		/// </summary>
		private Model _model;

		/// <summary>
		///     Initializes the renderer.
		/// </summary>
		/// <param name="renderContext">The render context that should be used for drawing.</param>
		/// <param name="assets">The asset bundle that provides access to Lwar assets.</param>
		public void Initialize(RenderContext renderContext, GameBundle assets)
		{
			const int thickness = 64;
			var outline = new CircleOutline();
			outline.Add(Int16.MaxValue + thickness / 2, 256, thickness);
			_model = outline.ToModel(renderContext.GraphicsDevice);

			_effect = assets.SimpleVertexEffect;
		}

		/// <summary>
		///     Draws the skybox.
		/// </summary>
		/// <param name="output">The output that the bullets should be rendered to.</param>
		public void Draw(RenderOutput output)
		{
			_effect.Color = new Vector4(0.3f, 0, 0, 0.3f);
			_effect.World = Matrix.Identity;
			_model.Draw(output, _effect.Default);
		}

		/// <summary>
		///     Draws all registered 2D elements.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the 2D elements.</param>
		public void Draw(SpriteBatch spriteBatch)
		{
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_model.SafeDispose();
		}
	}
}