namespace Lwar
{
	using System;
	using Assets;
	using Pegasus;
	using Pegasus.Platform.Logging;
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
		///     The allocator that is used to allocate game objects.
		/// </summary>
		private readonly PoolAllocator _allocator = new PoolAllocator();

		/// <summary>
		///     The root view model of the view model stacked used by the application.
		/// </summary>
		private readonly StackedViewModel _viewModelRoot = StackedViewModel.CreateRoot();

		/// <summary>
		///     The game server that is currently running.
		/// </summary>
		private Server _server;

		/// <summary>
		///     Gets a value indicating whether a server is currently running.
		/// </summary>
		private bool IsServerRunning
		{
			get { return _server != null; }
		}

		/// <summary>
		///     Gets a value indicating whether a client is currently running.
		/// </summary>
		private bool IsClientRunning
		{
			get { return !(_viewModelRoot.Child is MainMenuViewModel); }
		}

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

			Commands.Bind(Key.F1.WentDown(), "start_server");
			Commands.Bind(Key.F2.WentDown(), "stop_server");
			Commands.Bind(Key.F3.WentDown(), "connect 127.0.0.1");
			Commands.Bind(Key.F4.WentDown(), "disconnect");
			Commands.Bind(Key.F5.WentDown(), "reload_assets");
			Commands.Bind(Key.F6.WentDown(), "toggle use_debug_server");

			Commands.Bind(Key.C.WentDown(), "toggle_debug_camera");
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

			_viewModelRoot.ReplaceChild(new LoadingViewModel(_allocator, new IPEndPoint(address, port)));
		}

		/// <summary>
		///     Disconnects from the current game session.
		/// </summary>
		private void Disconnect()
		{
			if (!(_viewModelRoot.Child is LoadingViewModel || _viewModelRoot.Child is GameSessionViewModel))
				return;

			_viewModelRoot.ReplaceChild(new MainMenuViewModel());

			if (!IsServerRunning)
				_allocator.Free();
		}

		/// <summary>
		///     Starts a local game server.
		/// </summary>
		private void StartServer()
		{
			_server.SafeDispose();

			try
			{
				if (Cvars.UseDebugServer)
					_server = new DebugServer(_allocator);
				else
					_server = new NativeServer();
			}
			catch (NetworkException e)
			{
				Log.Error("Unable to start the server: {0}", e.Message);
				MessageBox.Show("Server Failure", String.Format("Unable to start the server: {0}", e.Message));

				_server.SafeDispose();
				_server = null;
			}
		}

		/// <summary>
		///     Stops the currently running local game server.
		/// </summary>
		private void StopServer()
		{
			_server.SafeDispose();
			_server = null;

			if (!IsClientRunning)
				_allocator.Free();
		}

		/// <summary>
		///     Invoked when the application should update the its state.
		/// </summary>
		protected override void Update()
		{
			if (IsServerRunning)
				_server.Update();

			_viewModelRoot.Update();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void Dispose()
		{
			_viewModelRoot.SafeDispose();
			_server.SafeDispose();
			_allocator.SafeDispose();

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