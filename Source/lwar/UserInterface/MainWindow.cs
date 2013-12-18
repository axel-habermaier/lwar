namespace Lwar.UserInterface
{
	using System;
	using Pegasus;
	using Pegasus.Math;
	using Pegasus.Platform.Graphics;
	using Pegasus.Rendering;

	public partial class MainWindow
	{
		/// <summary>
		///     The graphics device that is used to initialize the render output panel.
		/// </summary>
		private GraphicsDevice _graphicsDevice;

		/// <summary>
		///     Initializes the window.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to initialize the render output panel.</param>
		public void Initialize(GraphicsDevice graphicsDevice)
		{
			Assert.ArgumentNotNull(graphicsDevice);

			_graphicsDevice = graphicsDevice;
			RenderOutputPanel.InitializeRenderOutput += InitializeRenderOutputPanel;
		}

		/// <summary>
		///     Initializes the render output panel's graphics objects.
		/// </summary>
		/// <param name="panelSize">The size of the render output panel.</param>
		private void InitializeRenderOutputPanel(Size panelSize)
		{
			// Initialize the color buffer of the render target
			var colorBuffer = new Texture2D(_graphicsDevice, panelSize, SurfaceFormat.Rgba8, TextureFlags.RenderTarget);
			colorBuffer.SetName("MainWindow.RenderOutputPanel.ColorBuffer");

			// Initialize the depth stencil buffer of the render target
			var depthStencil = new Texture2D(_graphicsDevice, panelSize, SurfaceFormat.Depth24Stencil8, TextureFlags.DepthStencil);
			depthStencil.SetName("MainWindow.RenderOutputPanel.DepthStencil");

			// Initialize the render output panel's graphics properties
			RenderOutputPanel.OutputTexture = colorBuffer;
			RenderOutputPanel.RenderOutput = new RenderOutput(_graphicsDevice)
			{
				Camera = new Camera3D(_graphicsDevice),
				RenderTarget = new RenderTarget(_graphicsDevice, depthStencil, colorBuffer),
				Viewport = new Rectangle(0, 0, panelSize)
			};

			RenderOutputPanel.RenderOutput.ClearColor(new Color(0xFF7F3FCD));
		}
	}
}