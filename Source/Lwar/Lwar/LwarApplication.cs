namespace Lwar
{
	using System;
	using Assets;
	using Network.Server;
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
	internal sealed partial class LwarApplication
	{
		/// <summary>
		///     The root view model of the view model stacked used by the application.
		/// </summary>
		private readonly StackedViewModel _viewModelRoot = StackedViewModel.CreateRoot();

		/// <summary>
		///     The assets used by the Lwar menu.
		/// </summary>
		private MenuBundle _menuBundle;

		/// <summary>
		///     Invoked when the application is initializing.
		/// </summary>
		protected override void Initialize()
		{
			UseFixedTimeStep = true;
			TargetElapsedSeconds = 1 / 60.0;

			Commands.Resolve();
			Cvars.Resolve();

			Window.Closing += Exit;
			Commands.OnConnect += Connect;
			Commands.OnDisconnect += Disconnect;
			Commands.OnStartServer += StartServer;
			Commands.OnStopServer += StopServer;
			Cvars.UseNativeServerChanged += OnUseNativeServerChanged;

			Commands.Bind(Key.F1.WentDown(), "start_server TestServer");
			Commands.Bind(Key.F2.WentDown(), "stop_server");
			Commands.Bind(Key.F3.WentDown(), "connect ::1");
			Commands.Bind(Key.F4.WentDown(), "disconnect");
			Commands.Bind(Key.F5.WentDown(), "reload_assets");
			Commands.Bind(Key.F6.WentDown(), "toggle use_native_server");

			Commands.Bind(Key.Escape.WentDown() & Key.LeftShift.IsPressed(), "exit");
			Commands.Bind(Key.F9.WentDown(), "show_particle_effect_viewer");
			Commands.Bind(Key.F10.WentDown(), "toggle show_debug_overlay");

			_viewModelRoot.Child = new MainMenuViewModel();
			_viewModelRoot.Activate();

			_menuBundle = new MenuBundle(RenderContext);
			_menuBundle.Load();
		}

		/// <summary>
		///     Handles changes to the use native server cvar.
		/// </summary>
		/// <param name="useNative">The new value of the cvar.</param>
		private static void OnUseNativeServerChanged(bool useNative)
		{
			StopServer();
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
			LwarServer.TryStart(serverName, port);
		}

		/// <summary>
		///     Stops the currently running local game server.
		/// </summary>
		private static void StopServer()
		{
			LwarServer.Stop();
		}

		/// <summary>
		///     Invoked when the application should update the its state.
		/// </summary>
		protected override void Update()
		{
			LwarServer.CheckForErrors();
			_viewModelRoot.Update();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void Dispose()
		{
			_menuBundle.SafeDispose();
			_viewModelRoot.SafeDispose();
			LwarServer.Stop();

			Commands.OnConnect -= Connect;
			Commands.OnDisconnect -= Disconnect;
			Commands.OnStartServer -= StartServer;
			Commands.OnStopServer -= StopServer;
			Cvars.UseNativeServerChanged -= OnUseNativeServerChanged;

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

			Bootstrapper.Run<LwarApplication>(args, "Lwar");
		}
	}
}