namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets;
	using Assets.Effects;
	using Pegasus.Assets;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;

	/// <summary>
	///     Renders a skybox.
	/// </summary>
	public class SkyboxRenderer : DisposableObject, IRenderer
	{
		/// <summary>
		///     The skybox cube map.
		/// </summary>
		private CubeMap _cubeMap;

		/// <summary>
		///     The effect that is used to draw the skybox.
		/// </summary>
		private SkyboxEffect _effect;

		/// <summary>
		///     The skybox model.
		/// </summary>
		private Model _model;

		/// <summary>
		///     Loads the required assets of the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		/// <param name="assets">The assets manager that should be used to load all required assets.</param>
		public void Load(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			_cubeMap = assets.Load(Textures.SpaceCubemap);
			_effect = new SkyboxEffect(graphicsDevice, assets);
		}

		/// <summary>
		///     Initializes the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		public void Initialize(GraphicsDevice graphicsDevice)
		{
			_model = Model.CreateSkybox(graphicsDevice);
			_effect.Skybox = new CubeMapView(_cubeMap, SamplerState.BilinearClampNoMipmaps);
		}

		/// <summary>
		///     Draws the skybox.
		/// </summary>
		/// <param name="output">The output that the bullets should be rendered to.</param>
		public void Draw(RenderOutput output)
		{
			RasterizerState.CullCounterClockwise.Bind();
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
			_effect.SafeDispose();
			_model.SafeDispose();
		}
	}
}