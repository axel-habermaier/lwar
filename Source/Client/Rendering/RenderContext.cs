using System;

namespace Lwar.Client.Rendering
{
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Assets;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Represents the context in which rendering operations are performed.
	/// </summary>
	public class RenderContext : DisposableObject
	{
		private readonly RenderTarget _backBuffer;
		private readonly Texture2D _depth;
		private readonly FullscreenQuad _fsQuad;
		private readonly FragmentShader _quadShader;
		private readonly RenderTarget _rt;
		private readonly Texture2D _screen;

		/// <summary>
		///   The renderer that is used to draw the skybox.
		/// </summary>
		private readonly SkyBoxRenderer _skyBoxRenderer;

		/// <summary>
		///   The rasterizer state that is used to draw in wireframe mode.
		/// </summary>
		private readonly RasterizerState _wireframe;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="window">The window that displays the game session.</param>
		/// <param name="graphicsDevice">The graphics device that is used to draw the game session.</param>
		/// <param name="renderTarget">The render target the render context should draw into.</param>
		/// <param name="assets">The assets manager that manages all assets of the game session.</param>
		public RenderContext(Window window, GraphicsDevice graphicsDevice, RenderTarget renderTarget, AssetsManager assets)
		{
			Assert.ArgumentNotNull(window, () => window);
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(renderTarget, () => renderTarget);
			Assert.ArgumentNotNull(assets, () => assets);

			_wireframe = new RasterizerState(graphicsDevice) { CullMode = CullMode.Back, FillMode = FillMode.Wireframe };
			_skyBoxRenderer = new SkyBoxRenderer(graphicsDevice, assets);
			_backBuffer = renderTarget;

			_fsQuad = new FullscreenQuad(graphicsDevice, assets);
			_quadShader = assets.LoadFragmentShader("Shaders/QuadFS");
			_screen = new Texture2D(graphicsDevice, 1280, 720, SurfaceFormat.Rgba16F, TextureFlags.RenderTarget);
			_depth = new Texture2D(graphicsDevice, 1280, 720, SurfaceFormat.Depth24Stencil8, TextureFlags.DepthStencil);
			_rt = new RenderTarget(graphicsDevice, new[] { _screen }, _depth);

			SunRenderer = new SunRenderer(graphicsDevice, _rt, assets);
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
		/// <param name="camera">The camera that should be used to render the frame.</param>
		public void Draw(Camera camera)
		{
			Assert.ArgumentNotNull(camera, () => camera);

			DepthStencilState.Default.Bind();
			camera.Bind();

			if (LwarCvars.DrawWireframe.Value)
				_wireframe.Bind();
			else
				RasterizerState.CullCounterClockwise.Bind();

			_skyBoxRenderer.Draw();

			_rt.Bind();
			_rt.Clear(new Color(0, 0, 0, 0));
			_rt.ClearDepth();

			SunRenderer.Draw();
			PlanetRenderer.Draw();
			ShipRenderer.Draw();
			BulletRenderer.Draw();

			DepthStencilState.DepthDisabled.Bind();
			_backBuffer.Bind();
			_screen.Bind(0);
			SamplerState.PointClampNoMipmaps.Bind(0);
			_quadShader.Bind();
			_fsQuad.Draw();
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_fsQuad.SafeDispose();
			_screen.SafeDispose();
			_depth.SafeDispose();
			_rt.SafeDispose();

			SunRenderer.SafeDispose();
			PlanetRenderer.SafeDispose();
			BulletRenderer.SafeDispose();
			ShipRenderer.SafeDispose();

			_skyBoxRenderer.SafeDispose();
			_wireframe.SafeDispose();
		}
	}
}