namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Math;
	using Platform.Graphics;
	using Platform.Memory;
	using Rendering;

	/// <summary>
	///     Represents the default window of an application.
	/// </summary>
	partial class AppWindow
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="title">The title of the window.</param>
		/// <param name="position">The screen position of the window's top left corner.</param>
		/// <param name="size">The size of the window's rendering area.</param>
		/// <param name="mode">Indicates the window mode.</param>
		internal AppWindow(string title, Vector2i position, Size size, WindowMode mode)
			: base(title, position, size, mode)
		{
			InitializeComponents();

			RenderOutputPanel.InitializeRenderOutput += InitializeRenderOutputPanel;
			RenderOutputPanel.DisposeRenderOutput += DisposeRenderOutputPanel;
		}

		/// <summary>
		///     Gets the render output that should be used for 3D rendering.
		/// </summary>
		public RenderOutput RenderOutput
		{
			get
			{
				Assert.NotNull(RenderOutputPanel.RenderOutput, "The render output panel has not yet been initialized.");
				return RenderOutputPanel.RenderOutput;
			}
		}

		/// <summary>
		///     Initializes the render output panel's graphics objects.
		/// </summary>
		/// <param name="panelSize">The size of the render output panel.</param>
		private void InitializeRenderOutputPanel(Size panelSize)
		{
			var graphicsDevice = Application.Current.GraphicsDevice;
			Assert.NotNull(graphicsDevice);

			// Initialize the color buffer of the render target
			var colorBuffer = new Texture2D(graphicsDevice, panelSize, SurfaceFormat.Rgba8, TextureFlags.RenderTarget);
			colorBuffer.SetName("MainWindow.RenderOutputPanel.ColorBuffer");

			// Initialize the depth stencil buffer of the render target
			var depthStencil = new Texture2D(graphicsDevice, panelSize, SurfaceFormat.Depth24Stencil8, TextureFlags.DepthStencil);
			depthStencil.SetName("MainWindow.RenderOutputPanel.DepthStencil");

			// Initialize the render output panel's graphics properties
			RenderOutputPanel.OutputTexture = colorBuffer;
			RenderOutputPanel.RenderOutput = new RenderOutput(graphicsDevice)
			{
				RenderTarget = new RenderTarget(graphicsDevice, depthStencil, colorBuffer),
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
			base.OnClosing();
		}
	}
}