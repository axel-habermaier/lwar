namespace Lwar
{
	using System;
	using System.Net;
	using Assets;
	using Network;
	using Pegasus;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Input;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;
	using Screens;
	using Scripting;
	using UserInterface;
	using MainMenu = Screens.MainMenu;

	/// <summary>
	///     Represents the lwar application.
	/// </summary>
	public sealed partial class App
	{
		/// <summary>
		///     The local game server that can be used to hosts game sessions locally.
		/// </summary>
		private LocalServer _localServer;

		private ScreenManager _screenManager;

		private SpriteBatch _spriteBatch;
		private MainWindow _window;

		/// <summary>
		///     Invoked when the application is initializing.
		/// </summary>
		protected override void Initialize()
		{
			RegisterFontLoader(new FontLoader(Assets));

			Commands.Resolve();
			Cvars.Resolve();

			Commands.OnConnect += Connect;
			Commands.OnDisconnect += Disconnect;

			InputDevice.ActivateLayer(InputLayers.Game);
			Window.Closing += Exit;

			_localServer = new LocalServer();
			_screenManager = new ScreenManager(new AppContext(GraphicsDevice, Window, Assets, InputDevice));
			_screenManager.Add(new MainMenu());

			Commands.Bind(Key.F1.WentDown(), "start_server");
			Commands.Bind(Key.F2.WentDown(), "stop_server");
			Commands.Bind(Key.F3.WentDown(), "connect 127.0.0.1");
			Commands.Bind(Key.F4.WentDown(), "disconnect");
			Commands.Bind(Key.F5.WentDown(), "reload_assets");
			Commands.Bind(Key.C.WentDown(), "toggle_debug_camera");
			Commands.Bind(Key.Escape.WentDown(), "exit");
			Commands.Bind(Key.F9.WentDown(), "toggle show_platform_info");
			Commands.Bind(Key.F10.WentDown(), "toggle show_frame_stats");

			_window = new MainWindow();
			_window.Initialize(GraphicsDevice);

			_spriteBatch = new SpriteBatch(GraphicsDevice, Assets)
			{
				BlendState = BlendState.Premultiplied,
				DepthStencilState = DepthStencilState.DepthDisabled,
				SamplerState = SamplerState.PointClampNoMipmaps
			};
		}

		/// <summary>
		///     Invoked when the application should update the its state.
		/// </summary>
		protected override void Update()
		{
			_localServer.Update();
			_screenManager.Update();
		}

		/// <summary>
		///     Invoked when the application should draw a frame.
		/// </summary>
		protected override void Draw()
		{
			_window.Output3D.ClearColor(new Color(0, 0, 0, 255));
			_window.Output3D.ClearDepth();

			_screenManager.Draw(_window.Output3D);

			_screenManager.DrawUserInterface(_spriteBatch);
			_spriteBatch.DrawBatch(_window.Output2D);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void Dispose()
		{
			Commands.OnConnect -= Connect;
			Commands.OnDisconnect -= Disconnect;

			_spriteBatch.SafeDispose();
			_screenManager.SafeDispose();
			_localServer.SafeDispose();
			_window.SafeDispose();

			base.Dispose();
		}

		/// <summary>
		///     Connects to the server at the given end point and joins the game session.
		/// </summary>
		/// <param name="address">The IP address of the server.</param>
		/// <param name="port">The port of the server.</param>
		private void Connect(IPAddress address, ushort port)
		{
			Assert.ArgumentNotNull(address);

			Disconnect();
			_screenManager.Add(new Level(new IPEndPoint(address, port)));
		}

		/// <summary>
		///     Disconnects from the game session the client is currently connected to.
		/// </summary>
		private void Disconnect()
		{
			_screenManager.Clear();
			_screenManager.Add(new MainMenu());
		}
	}
}