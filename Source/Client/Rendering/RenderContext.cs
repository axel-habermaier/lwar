using System;

namespace Lwar.Client.Rendering
{
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform.Assets;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Represents the context in which rendering operations are performed.
	/// </summary>
	public class RenderContext : DisposableObject
	{
		/// <summary>
		///   The output the render context renders to.
		/// </summary>
		private readonly RenderOutput _renderOutput;

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
		/// <param name="renderTarget">The render target the render context should draw into.</param>
		public RenderContext(GraphicsDevice graphicsDevice, AssetsManager assets, RenderTarget renderTarget)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(assets, () => assets);
			Assert.ArgumentNotNull(renderTarget, () => renderTarget);

			_wireframe = new RasterizerState(graphicsDevice) { CullMode = CullMode.Back, FillMode = FillMode.Wireframe };
			_skyboxRenderer = new SkyboxRenderer(graphicsDevice, assets);

			SunRenderer = new SunRenderer(graphicsDevice, renderTarget, assets);
			PlanetRenderer = new PlanetRenderer(graphicsDevice, assets);
			ShipRenderer = new ShipRenderer(graphicsDevice, assets);
			BulletRenderer = new BulletRenderer(graphicsDevice, assets);

			_renderOutput = new RenderOutput(graphicsDevice) { RenderTarget = renderTarget };
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
		/// <param name="camera">The camera that should be used to render the frame.</param>
		public void Draw(Camera camera)
		{
			Assert.ArgumentNotNull(camera, () => camera);

			if (LwarCvars.DrawWireframe.Value)
				_wireframe.Bind();
			else
				RasterizerState.CullCounterClockwise.Bind();

			_renderOutput.Camera = camera;
			_skyboxRenderer.Draw(_renderOutput);

			DepthStencilState.DepthEnabled.Bind();

			SunRenderer.Draw(_renderOutput);
			PlanetRenderer.Draw(_renderOutput);

			DepthStencilState.DepthDisabled.Bind();

			ShipRenderer.Draw(_renderOutput);
			BulletRenderer.Draw(_renderOutput);
		}

		/// <summary>
		///   Resizes the viewport of the rendering output.
		/// </summary>
		/// <param name="size">The new size.</param>
		public void Resize(Size size)
		{
			_renderOutput.Viewport = new Rectangle(Vector2i.Zero, size);
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
			_renderOutput.SafeDispose();
		}
	}
}