using System;

namespace Client
{
	using System.Collections.Generic;
	using Gameplay;
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Processes;
	using Pegasus.Framework.Scripting;

	internal sealed class LwarApp : App
	{
		private readonly ProcessScheduler _scheduler = new ProcessScheduler();
		private readonly Server _server = new Server();

		/// <summary>
		///   The current game session.
		/// </summary>
		private readonly GameSession _session = new GameSession();

		private IProcess _serverProcess;

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

			_serverProcess = _scheduler.CreateProcess(_server.Run);
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