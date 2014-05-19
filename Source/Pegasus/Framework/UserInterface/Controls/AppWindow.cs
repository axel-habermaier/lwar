namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Assets;
	using Math;
	using Platform.Graphics;
	using Platform.Memory;
	using Rendering;
	using Rendering.UserInterface;
	using Console = Rendering.UserInterface.Console;

	/// <summary>
	///     Represents the default window of an application.
	/// </summary>
	partial class AppWindow
	{
		/// <summary>
		///     The camera that is used to draw the console and the debug overlay.
		/// </summary>
		private readonly Camera _camera;

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

			_renderOutputPanel.InitializeRenderOutput += InitializeRenderOutputPanel;
			_renderOutputPanel.DisposeRenderOutput += DisposeRenderOutputPanel;

			_camera = new Camera2D(Application.Current.GraphicsDevice);

			var font = Application.Current.Assets.Load(Fonts.LiberationMono11);
			DebugOverlay = new DebugOverlay(font);
			Console = new Console(InputDevice, font);

			// Ensure that the console and the statistics are properly initialized
			DebugOverlay.Update(Size);
			Console.Update(Size);
		}

		/// <summary>
		///     Gets the layout root of the application window.
		/// </summary>
		public LayoutRoot LayoutRoot
		{
			get { return _layoutRoot; }
		}

		/// <summary>
		///     Gets or sets the console that should be drawn on top of the window's backbuffer.
		/// </summary>
		internal Console Console { get; set; }

		/// <summary>
		///     Gets or sets the debug overlay that should be drawn on top of the window's backbuffer.
		/// </summary>
		internal DebugOverlay DebugOverlay { get; set; }

		/// <summary>
		///     Gets the render output that should be used for 3D rendering.
		/// </summary>
		public RenderOutput RenderOutput
		{
			get
			{
				Assert.NotNull(_renderOutputPanel.RenderOutput, "The render output panel has not yet been initialized.");
				return _renderOutputPanel.RenderOutput;
			}
		}

		/// <summary>
		///     Initializes the render output panel's graphics objects.
		/// </summary>
		/// <param name="panelSize">The size of the render output panel.</param>
		private void InitializeRenderOutputPanel(Size panelSize)
		{
			// Initialize the color buffer of the render target
			var colorBuffer = new Texture2D(Application.Current.GraphicsDevice, panelSize, SurfaceFormat.Rgba8, TextureFlags.RenderTarget);
			colorBuffer.SetName("AppWindow.RenderOutputPanel.ColorBuffer");

			// Initialize the depth stencil buffer of the render target
			var depthStencil = new Texture2D(Application.Current.GraphicsDevice, panelSize, SurfaceFormat.Depth24Stencil8, TextureFlags.DepthStencil);
			depthStencil.SetName("AppWindow.RenderOutputPanel.DepthStencil");

			// Initialize the render output panel's graphics properties
			_renderOutputPanel.OutputTexture = colorBuffer;
			_renderOutputPanel.RenderOutput = new RenderOutput(Application.Current.GraphicsDevice)
			{
				RenderTarget = new RenderTarget(Application.Current.GraphicsDevice, depthStencil, colorBuffer),
				Viewport = new Rectangle(0, 0, panelSize)
			};
		}

		/// <summary>
		///     Disposes the render output panel's graphics objects.
		/// </summary>
		private void DisposeRenderOutputPanel()
		{
			if (_renderOutputPanel.RenderOutput != null)
				_renderOutputPanel.RenderOutput.RenderTarget.SafeDispose();

			_renderOutputPanel.OutputTexture.SafeDispose();
			_renderOutputPanel.RenderOutput.SafeDispose();

			_renderOutputPanel.OutputTexture = null;
			_renderOutputPanel.RenderOutput = null;
		}

		/// <summary>
		///     Invoked when the window is being closed.
		/// </summary>
		protected override void OnClosing()
		{
			DisposeRenderOutputPanel();

			_camera.SafeDispose();
			Console.SafeDispose();
			DebugOverlay.SafeDispose();

			base.OnClosing();
		}

		/// <summary>
		///     Invoked after the window has been drawn.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used for drawing.</param>
		protected override void OnWindowDrawn(SpriteBatch spriteBatch)
		{
			var camera = _renderOutputPanel.RenderOutput.Camera;
			_renderOutputPanel.RenderOutput.Camera = _camera;

			spriteBatch.WorldMatrix = Matrix.Identity;
			spriteBatch.UseScissorTest = false;

			if (Console != null)
				Console.Draw(spriteBatch);

			if (DebugOverlay != null)
				DebugOverlay.Draw(spriteBatch);

			_renderOutputPanel.RenderOutput.Camera = camera;
			spriteBatch.Layer = 0;
		}
	}
}