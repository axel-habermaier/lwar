namespace Lwar
{
	using System;
	using Assets;
	using Network;
	using Pegasus.Framework;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Input;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;
	using Screens;
	using Scripting;
	using UserInterface;

	/// <summary>
	///     Represents the Lwar application.
	/// </summary>
	public sealed partial class LwarApplication
	{
		/// <summary>
		///     The local game server that can be used to hosts game sessions locally.
		/// </summary>
		private readonly LocalServer _localServer = new LocalServer();

		private readonly ScreenManager _screenManager = new ScreenManager();
		private readonly RootViewModel _viewModel = new RootViewModel();
		private Camera2D _camera;
		private SpriteBatch _spriteBatch;

		/// <summary>
		///     Invoked when the application is initializing.
		/// </summary>
		protected override void Initialize()
		{
			RegisterFontLoader(new FontLoader(Assets));
			Commands.Resolve();
			Cvars.Resolve();

			Window.InputDevice.ActivateLayer(InputLayers.Game);
			Window.Closing += Exit;

			_screenManager.Add(new MainMenu());

			Commands.Bind(Key.F1.WentDown(), "start_server");
			Commands.Bind(Key.F2.WentDown(), "stop_server");
			Commands.Bind(Key.F3.WentDown(), "connect 127.0.0.1");
			Commands.Bind(Key.F4.WentDown(), "disconnect");
			Commands.Bind(Key.F5.WentDown(), "reload_assets");

			Commands.Bind(Key.C.WentDown(), "toggle_debug_camera");
			Commands.Bind(Key.Escape.WentDown(), "exit");
			Commands.Bind(Key.F10.WentDown(), "toggle show_debug_overlay");

			_camera = new Camera2D(GraphicsDevice);
			_spriteBatch = new SpriteBatch(GraphicsDevice, Assets)
			{
				BlendState = BlendState.Premultiplied,
				DepthStencilState = DepthStencilState.DepthDisabled,
				SamplerState = SamplerState.PointClampNoMipmaps
			};

			//var uc1 = new UserControl1();
			//Window.LayoutRoot.Children.Add(uc1);

			_viewModel.Activate();
		}

		/// <summary>
		///     Invoked when the application should update the its state.
		/// </summary>
		protected override void Update()
		{
			_localServer.Update();
			_screenManager.Update();
			_viewModel.Update();
		}

		/// <summary>
		///     Invoked when the application should draw a frame.
		/// </summary>
		/// <param name="output">The render output that should be used to draw the frame.</param>
		protected override void Draw(RenderOutput output)
		{
			output.ClearColor(new Color(0, 0, 0, 255));
			output.ClearDepth();

			_screenManager.Draw(output);

			var camera = output.Camera;
			output.Camera = _camera;

			_camera.Viewport = output.Viewport;
			_screenManager.DrawUserInterface(_spriteBatch);
			_spriteBatch.DrawBatch(output);

			output.Camera = camera;
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void Dispose()
		{
			_viewModel.SafeDispose();
			_camera.SafeDispose();
			_spriteBatch.SafeDispose();
			_screenManager.SafeDispose();
			_localServer.SafeDispose();

			base.Dispose();
		}

		/// <summary>
		///     The entry point of the application.
		/// </summary>
		/// <param name="args">The command line arguments passed to the application.</param>
		public static void Main(string[] args)
		{
			Commands.Initialize();
			Cvars.Initialize();

			Bootstrapper<LwarApplication>.Run(args, "Lwar");
		}
	}
}