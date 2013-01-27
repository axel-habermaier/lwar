using System;

namespace Lwar.Client
{
	using System.Collections.Generic;
	using System.Net;
	using Gameplay;
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Input;
	using Pegasus.Framework.Processes;
	using Pegasus.Framework.Scripting;

	/// <summary>
	///   Represents the lwar client.
	/// </summary>
	internal sealed class LwarApp : App
	{
		/// <summary>
		///   Starts up a new server instance.
		/// </summary>
		public static readonly Command StartServer = new Command("start", "Starts up a new server instance.");

		/// <summary>
		///   Shuts down the currently running server.
		/// </summary>
		public static readonly Command StopServer = new Command("stop", "Shuts down the currently running server.");

		/// <summary>
		///   Connects to a game session on a server.
		/// </summary>
		public static readonly Command<IPEndPoint> Connect = new Command<IPEndPoint>("connect",
																					 "Connects to a game session on a server.");

		/// <summary>
		///   Disconnects from the current game session.
		/// </summary>
		public static readonly Command Disconnect = new Command("disconnect", "Disconnects from the current game session.");

		private readonly ProcessScheduler _scheduler = new ProcessScheduler();

		/// <summary>
		///   The current game session.
		/// </summary>
		private readonly GameSession _session = new GameSession();

		private IProcess _connectProcess, _handleServerMessagesProcess;

		private ServerProxy _proxy;
		private IProcess _serverProcess;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public LwarApp()
		{
			StartServer.Invoked += () =>
				{
					_serverProcess.SafeDispose();
					_serverProcess = _scheduler.CreateProcess(new Server().Run);
				};

			StopServer.Invoked += () =>
				{
					if (_serverProcess == null)
						Log.Warn("The server is currently not running.");

					_serverProcess.SafeDispose();
					_serverProcess = null;
				};

			Connect.Invoked += serverEndPoint =>
				{
					_proxy.SafeDispose();
					_connectProcess.SafeDispose();
					_handleServerMessagesProcess.SafeDispose();

					_proxy = new ServerProxy(serverEndPoint);
					_handleServerMessagesProcess = _scheduler.CreateProcess(_proxy.HandleServerMessages);
					_connectProcess = _scheduler.CreateProcess(_proxy.Connect);
				};

			Disconnect.Invoked += () =>
				{
					if (_proxy == null)
						Log.Warn("Not connected to any server.");

					_proxy.SafeDispose();
					_connectProcess.SafeDispose();
					_handleServerMessagesProcess.SafeDispose();

					_proxy = null;
					_connectProcess = null;
					_handleServerMessagesProcess = null;
				};
		}

		/// <summary>
		///   Invoked when the application should update the game state.
		/// </summary>
		protected override void Update()
		{
			_scheduler.RunProcesses();
			_session.Update();
		}

		/// <summary>
		///   Invoked when the application should draw a frame.
		/// </summary>
		protected override void Draw()
		{
			SwapChain.BackBuffer.Bind();
			RasterizerState.CullNone.Bind();
			SwapChain.BackBuffer.Clear(ClearTargets.Color, Color.Black);

			_session.Draw();
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_handleServerMessagesProcess.SafeDispose();
			_connectProcess.SafeDispose();
			_proxy.SafeDispose();
			_serverProcess.SafeDispose();
			_session.SafeDispose();
			_scheduler.SafeDispose();

			base.OnDisposing();
		}

		/// <summary>
		///   Invoked when the application is initializing.
		/// </summary>
		protected override void Initialize()
		{
			UpdatesPerSecond = 60;

			Window.Closing += Exit;
			Window.Resized += s => GraphicsDevice.SetViewport(new Rectangle(0, 0, s.Width, s.Height));
			Window.Title = "lwar";
			Window.Size = new Size(1280, 720);

			AssetsLoader.Load(Assets);

			_session.Window = Window;
			_session.GraphicsDevice = GraphicsDevice;
			_session.InputDevice = LogicalInputDevice;
			_session.Players = new List<Player> { new Player() };
			_session.Initialize();

			Commands.Bind.Invoke(Key.F1.WentDown(), "start");
			Commands.Bind.Invoke(Key.F2.WentDown(), "stop");
			Commands.Bind.Invoke(Key.F3.WentDown(), "connect 127.0.0.1:" + ServerProxy.DefaultPort);
			Commands.Bind.Invoke(Key.F4.WentDown(), "disconnect");
		}

		/// <summary>
		///   Entry point of the application.
		/// </summary>
		/// <param name="args">The arguments the application was started with.</param>
		private static void Main(string[] args)
		{
			Cvars.AppName.Value = "lwar";

			using (var app = new LwarApp())
				app.Run();
		}
	}
}