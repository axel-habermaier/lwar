namespace Lwar
{
	using System;
	using System.Net;
	using Network;
	using Pegasus;
	using Pegasus.Platform.Input;
	using Pegasus.Platform.Memory;
	using Scripting;
	using UserInterface;

	/// <summary>
	///     Represents the lwar application.
	/// </summary>
	public sealed partial class App
	{
		/// <summary>
		///     The local game server that can be used to hosts game sessions locally.
		/// </summary>
		private LocalServer _localServer;

		/// <summary>
		///     Invoked when the application is initializing.
		/// </summary>
		protected override void Initialize()
		{
			Commands.Resolve();
			Cvars.Resolve();

			Commands.OnConnect += Connect;
			Commands.OnDisconnect += Disconnect;

			//Context.InputDevice.ActivateLayer(InputLayers.Game);
			//Context.Window.Closing += Exit;

			_localServer = new LocalServer();
			//_stateManager = new ScreenManager(Context);
			//_stateManager.Add(new MainMenu());

			Commands.Bind(Key.F1.WentDown(), "start_server");
			Commands.Bind(Key.F2.WentDown(), "stop_server");
			Commands.Bind(Key.F3.WentDown(), "connect 127.0.0.1");
			Commands.Bind(Key.F4.WentDown(), "disconnect");
			Commands.Bind(Key.F5.WentDown(), "reload_assets");
			Commands.Bind(Key.C.WentDown(), "toggle_debug_camera");
			Commands.Bind(Key.Escape.WentDown(), "exit");
			Commands.Bind(Key.F9.WentDown(), "toggle show_platform_info");
			Commands.Bind(Key.F10.WentDown(), "toggle show_frame_stats");

			//var uc1 = new UserControl1();
			////Add(uc1);
			ShowWindow(new MainWindow());
		}

		/// <summary>
		///     Invoked when the application should update the its state.
		/// </summary>
		protected override void Update()
		{
			_localServer.Update();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Commands.OnConnect -= Connect;
			Commands.OnDisconnect -= Disconnect;

			//_stateManager.SafeDispose();
			_localServer.SafeDispose();

			base.OnDisposing();
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
		}

		/// <summary>
		///     Disconnects from the game session the client is currently connected to.
		/// </summary>
		private void Disconnect()
		{
		}
	}
}