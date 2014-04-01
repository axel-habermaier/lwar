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

	/// <summary>
	///     Represents the lwar application.
	/// </summary>
	public sealed partial class App
	{
		private Camera2D _camera;

		/// <summary>
		///     The local game server that can be used to hosts game sessions locally.
		/// </summary>
		private LocalServer _localServer;

		private ScreenManager _screenManager;

		private SpriteBatch _spriteBatch;

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

			Window.InputDevice.ActivateLayer(InputLayers.Game);
			Window.Closing += Exit;

			_localServer = new LocalServer();
			_screenManager = new ScreenManager();
			_screenManager.Add(new MainMenu());

			Commands.Bind(Key.F1.WentDown(), "start_server");
			Commands.Bind(Key.F2.WentDown(), "stop_server");
			Commands.Bind(Key.F3.WentDown(), "connect 127.0.0.1");
			Commands.Bind(Key.F4.WentDown(), "disconnect");
			Commands.Bind(Key.F5.WentDown(), "reload_assets \"../../Source/lwar Assets/lwar Assets.csproj\"");
			Commands.Bind(Key.C.WentDown(), "toggle_debug_camera");
			Commands.Bind(Key.Escape.WentDown(), "exit");
			Commands.Bind(Key.F9.WentDown(), "toggle show_platform_info");
			Commands.Bind(Key.F10.WentDown(), "toggle show_frame_stats");

			_camera = new Camera2D(GraphicsDevice);
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
			Commands.OnConnect -= Connect;
			Commands.OnDisconnect -= Disconnect;

			_camera.SafeDispose();
			_spriteBatch.SafeDispose();
			_screenManager.SafeDispose();
			_localServer.SafeDispose();

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