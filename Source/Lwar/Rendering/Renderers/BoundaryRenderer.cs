namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets.Effects;
	using Pegasus.Assets;
	using Pegasus.Math;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;

	/// <summary>
	///     Renders the world boundary.
	/// </summary>
	public class BoundaryRenderer : DisposableObject, IRenderer
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
		///     Loads the required assets of the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		/// <param name="assets">The assets manager that should be used to load all required assets.</param>
		public void Load(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			_effect = new SimpleVertexEffect(graphicsDevice, assets) { Color = new Vector4(0.3f, 0, 0, 0.3f) };
		}

		/// <summary>
		///     Initializes the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		public void Initialize(GraphicsDevice graphicsDevice)
		{
			const int thickness = 64;
			var outline = new CircleOutline();
			outline.Add(Int16.MaxValue + thickness / 2, 265, thickness);
			_model = outline.ToModel(graphicsDevice);
		}

		/// <summary>
		///     Draws the skybox.
		/// </summary>
		/// <param name="output">The output that the bullets should be rendered to.</param>
		public void Draw(RenderOutput output)
		{
			_model.Draw(output, _effect.Default);
		}

		/// <summary>
		///     Draws all registered 2D elements.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the 2D elements.</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			// TODO
		}

		/// <summary>
		///     Draws the user interface elements.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the user interface.</param>
		/// <param name="camera">The camera that is used to draw the scene.</param>
		public void DrawUserInterface(SpriteBatch spriteBatch, GameCamera camera)
		{
			// TODO
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_effect.SafeDispose();
			_model.SafeDispose();
		}
	}
}