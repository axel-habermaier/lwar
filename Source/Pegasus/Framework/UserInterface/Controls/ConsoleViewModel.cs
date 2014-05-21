namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Assets;
	using Platform.Graphics;
	using Platform.Input;
	using Platform.Memory;
	using Rendering;
	using Console = Rendering.UserInterface.Console;

	/// <summary>
	///     Displays the in-game console.
	/// </summary>
	internal class ConsoleViewModel : ViewModel
	{
		/// <summary>
		///     The console view.
		/// </summary>
		//private readonly ConsoleView _view;

		/// <summary>
		///     The window in which the console is shown.
		/// </summary>
		//private AppWindow _window;

		//private Camera2D Camera;
		//private Console Console;
		//private SpriteBatch SpriteBatch;
		//Texture2D 

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="window">The window in which the console should be shown.</param>
		/// <param name="inputDevice">The logical input device that should be used to listen for user input.</param>
		public ConsoleViewModel(AppWindow window, LogicalInputDevice inputDevice)
		{
			Assert.ArgumentNotNull(window);
			Assert.ArgumentNotNull(inputDevice);

			var font = Application.Current.Assets.Load(Fonts.LiberationMono11);
			//Console = new Console(inputDevice, font);
			//Console.Update(window.Size);

			//_view = new ConsoleView(Console);
			//_window = window;
			//window.LayoutRoot.AddTopmost(_view);

			//var colorBuffer = new Texture2D(Application.Current.GraphicsDevice, panelSize, SurfaceFormat.Rgba8, TextureFlags.RenderTarget);
			//colorBuffer.SetName("RenderOutputPanel.ColorBuffer");

			//renderOutput = new RenderOutput(Application.Current.GraphicsDevice)
			//{
			//	RenderTarget = new RenderTarget(Application.Current.GraphicsDevice, depthStencil, colorBuffer),
			//	Viewport = new Rectangle(0, 0, panelSize)
			//};

			//_consolePanel.Camera = new Camera2D(Application.Current.GraphicsDevice);
			//_consolePanel.SpriteBatch = new SpriteBatch(Application.Current.GraphicsDevice, Application.Current.Assets)
			//{
			//	BlendState = BlendState.Premultiplied,
			//	DepthStencilState = DepthStencilState.DepthDisabled,
			//	SamplerState = SamplerState.PointClampNoMipmaps
			//};

			//renderOutput.Camera = Camera;

			//renderOutput.ClearColor(new Color());

			//Camera.Viewport = renderOutput.Viewport;
			//Console.Update(renderOutput.Viewport.Size);
			//Console.Draw(SpriteBatch);

			//SpriteBatch.DrawBatch(renderOutput);
		}

		/// <summary>
		///     Gets the in-game console.
		/// </summary>
		public Console Console { get; private set; }

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Console.SafeDispose();
			//_window.LayoutRoot.Remove(_view);
		}
	}
}