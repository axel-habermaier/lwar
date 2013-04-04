using System;

namespace Lwar.Client.Rendering
{
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Rendering;
	using Scripting;

	/// <summary>
	///   Represents the context in which rendering operations are performed.
	/// </summary>
	public class RenderContext : DisposableObject
	{
		/// <summary>
		///   The cvar registry that handles the application cvars.
		/// </summary>
		private readonly CvarRegistry _cvars;

		/// <summary>
		///   The renderer that is used to draw the skybox.
		/// </summary>
		private readonly SkyboxRenderer _skyboxRenderer;

		/// <summary>
		///   The rasterizer state that is used to draw in wireframe mode.
		/// </summary>
		private readonly RasterizerState _wireframe;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that is used to draw the game session.</param>
		/// <param name="assets">The assets manager that manages all assets of the game session.</param>
		/// <param name="cvars"> The cvar registry that handles the application cvars.</param>
		public RenderContext(GraphicsDevice graphicsDevice, AssetsManager assets, CvarRegistry cvars)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(assets, () => assets);
			Assert.ArgumentNotNull(cvars, () => cvars);

			_cvars = cvars;
			_wireframe = new RasterizerState(graphicsDevice) { CullMode = CullMode.Back, FillMode = FillMode.Wireframe };
			_skyboxRenderer = new SkyboxRenderer(graphicsDevice, assets);

			SunRenderer = new SunRenderer(graphicsDevice, assets);
			PlanetRenderer = new PlanetRenderer(graphicsDevice, assets);
			ShipRenderer = new ShipRenderer(graphicsDevice, assets);
			BulletRenderer = new BulletRenderer(graphicsDevice, assets);
		}

		/// <summary>
		///   Gets the renderer that is used to draw planets.
		/// </summary>
		public PlanetRenderer PlanetRenderer { get; private set; }

		/// <summary>
		///   Gets the renderer that is used to draw ships.
		/// </summary>
		public ShipRenderer ShipRenderer { get; private set; }

		/// <summary>
		///   Gets the renderer that is used to draw bullets.
		/// </summary>
		public BulletRenderer BulletRenderer { get; private set; }

		/// <summary>
		///   Gets the renderer that is used to draw suns.
		/// </summary>
		public SunRenderer SunRenderer { get; private set; }

		/// <summary>
		///   Renders a frame.
		/// </summary>
		/// <param name="output">The output that the render context should render to.</param>
		public void Draw(RenderOutput output)
		{
			Assert.ArgumentNotNull(output, () => output);

			if (_cvars.DrawWireframe)
				_wireframe.Bind();
			else
				RasterizerState.CullCounterClockwise.Bind();

			_skyboxRenderer.Draw(output);

			DepthStencilState.DepthEnabled.Bind();

			SunRenderer.Draw(output);
			PlanetRenderer.Draw(output);

			DepthStencilState.DepthDisabled.Bind();

			ShipRenderer.Draw(output);
			BulletRenderer.Draw(output);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			SunRenderer.SafeDispose();
			PlanetRenderer.SafeDispose();
			BulletRenderer.SafeDispose();
			ShipRenderer.SafeDispose();

			_skyboxRenderer.SafeDispose();
			_wireframe.SafeDispose();
		}
	}
}