namespace Lwar.UserInterface
{
	using System;
	using Pegasus;
	using Pegasus.Math;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;

	public partial class MainWindow
	{
		/// <summary>
		///     The graphics device that is used to initialize the render output panel.
		/// </summary>
		private GraphicsDevice _graphicsDevice;

		/// <summary>
		///     Gets the render output that should be used for 3D rendering.
		/// </summary>
		public RenderOutput Output3D
		{
			get
			{
				Assert.NotNull(RenderOutputPanel.RenderOutput, "The render output panel has not yet been initialized.");
				return RenderOutputPanel.RenderOutput;
			}
		}

		/// <summary>
		///     Gets the render output that should be used for 2D rendering.
		/// </summary>
		public RenderOutput Output2D { get; private set; }

		/// <summary>
		///     Initializes the window.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to initialize the render output panel.</param>
		public void Initialize(GraphicsDevice graphicsDevice)
		{
			Assert.ArgumentNotNull(graphicsDevice);

			_graphicsDevice = graphicsDevice;
			RenderOutputPanel.InitializeRenderOutput += InitializeRenderOutputPanel;
			RenderOutputPanel.DisposeRenderOutput += DisposeRenderOutputPanel;
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
				RenderTarget = new RenderTarget(_graphicsDevice, depthStencil, colorBuffer),
				Viewport = new Rectangle(0, 0, panelSize)
			};

			// TODO: Remove this initialization of the 2D output and remove the 2D output altogether
			if (Output2D != null)
				Output2D.Camera.SafeDispose();

			Output2D.SafeDispose();

			var camera2D = new Camera2D(_graphicsDevice);
			Output2D = new RenderOutput(_graphicsDevice)
			{
				Camera = camera2D,
				RenderTarget = RenderOutputPanel.RenderOutput.RenderTarget,
				Viewport = new Rectangle(0, 0, panelSize)
			};
		}

		/// <summary>
		///     Disposes the render output panel's graphics objects.
		/// </summary>
		private void DisposeRenderOutputPanel()
		{
			if (RenderOutputPanel.RenderOutput != null)
				RenderOutputPanel.RenderOutput.RenderTarget.SafeDispose();

			RenderOutputPanel.OutputTexture.SafeDispose();
			RenderOutputPanel.RenderOutput.SafeDispose();

			RenderOutputPanel.OutputTexture = null;
			RenderOutputPanel.RenderOutput = null;
		}

		/// <summary>
		///     Invoked when the window is being closed.
		/// </summary>
		protected override void OnClosing()
		{
			DisposeRenderOutputPanel();

			if (Output2D != null)
				Output2D.Camera.SafeDispose();

			Output2D.SafeDispose();
			base.OnClosing();
		}
	}
}