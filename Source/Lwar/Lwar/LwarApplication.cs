namespace Lwar
{
	using System;
	using Assets;
	using Pegasus;
	using Pegasus.Platform.Memory;
	using Pegasus.Platform.Network;
	using Pegasus.UserInterface;
	using Pegasus.UserInterface.Input;
	using Pegasus.UserInterface.ViewModels;
	using Scripting;
	using UserInterface.ViewModels;

	/// <summary>
	///     Represents the Lwar application.
	/// </summary>
	public sealed partial class LwarApplication
	{
		/// <summary>
		///     The root view model of the view model stacked used by the application.
		/// </summary>
		private readonly StackedViewModel _viewModelRoot = StackedViewModel.CreateRoot();

		/// <summary>
		///     Invoked when the application is initializing.
		/// </summary>
		protected override void Initialize()
		{
			RegisterFontLoader(new FontLoader(Assets));
			Commands.Resolve();
			Cvars.Resolve();

			Window.Closing += Exit;
			Commands.OnConnect += Connect;
			Commands.OnDisconnect += Disconnect;
			Commands.OnStartServer += StartServer;
			Commands.OnStopServer += StopServer;
			Cvars.UseDebugServerChanged += v => StopServer();

			Commands.Bind(Key.F1.WentDown(), "start_server TestServer");
			Commands.Bind(Key.F2.WentDown(), "stop_server");
			Commands.Bind(Key.F3.WentDown(), "connect 127.0.0.1");
			Commands.Bind(Key.F4.WentDown(), "disconnect");
			Commands.Bind(Key.F5.WentDown(), "reload_assets");
			Commands.Bind(Key.F6.WentDown(), "toggle use_debug_server");

			Commands.Bind(Key.Escape.WentDown() & Key.LeftShift.IsPressed(), "exit");
			Commands.Bind(Key.F9.WentDown(), "show_particle_effect_viewer");
			Commands.Bind(Key.F10.WentDown(), "toggle show_debug_overlay");

			_viewModelRoot.Child = new MainMenuViewModel();
			_viewModelRoot.Activate();
		}

		/// <summary>
		///     Connects to the server at the given end point and joins the game session.
		/// </summary>
		/// <param name="address">The IP address of the server.</param>
		/// <param name="port">The port of the server.</param>
		private void Connect(IPAddress address, ushort port)
		{
			Commands.Disconnect();
			MessageBox.CloseAll();

			_viewModelRoot.ReplaceChild(new LoadingViewModel(new IPEndPoint(address, port)));
		}

		/// <summary>
		///     Disconnects from the current game session.
		/// </summary>
		private void Disconnect()
		{
			if (!(_viewModelRoot.Child is LoadingViewModel || _viewModelRoot.Child is GameSessionViewModel))
				return;

			_viewModelRoot.ReplaceChild(new MainMenuViewModel());
		}

		/// <summary>
		///     Starts a local game server.
		/// </summary>
		/// <param name="serverName">The name of the server that is displayed in the Join screen.</param>
		/// <param name="port">The port the server should use to communicate with the clients.</param>
		private static void StartServer(string serverName, ushort port)
		{
			Server.TryStart(serverName, port);
		}

		/// <summary>
		///     Stops the currently running local game server.
		/// </summary>
		private static void StopServer()
		{
			Server.Stop();
		}

		/// <summary>
		///     Invoked when the application should update the its state.
		/// </summary>
		protected override void Update()
		{
			Server.Update();
			_viewModelRoot.Update();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void Dispose()
		{
			_viewModelRoot.SafeDispose();
			Server.Stop();

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

			Bootstrapper.Run(new LwarApplication(), args, "Lwar");
		}
	}
}