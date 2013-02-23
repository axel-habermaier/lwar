using System;

namespace Lwar.Client
{
	using System.Threading.Tasks;
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
		///   The local game server that can be used to hosts game sessions locally.
		/// </summary>
		private readonly Server _server = new Server();

		/// <summary>
		///   The game session.
		/// </summary>
		private GameSession _session;

		/// <summary>
		///   Invoked when the application should update the game state.
		/// </summary>
		protected override void Update()
		{
			_server.Update();
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
			_session.SafeDispose();
			_server.SafeDispose();

			base.OnDisposing();
		}

		/// <summary>
		///   Invoked when the application is initializing.
		/// </summary>
		protected override void Initialize()
		{
			LogicalInputDevice.Modes = InputModes.Game;
			Window.Closing += Exit;
			Window.Resized += s => GraphicsDevice.SetViewport(new Rectangle(0, 0, s.Width, s.Height));
			Window.Title = "lwar";
			Window.Size = new Size(1280, 720);

			AssetsLoader.Load(Assets);
			_session = new GameSession(Window, GraphicsDevice, Assets, LogicalInputDevice);

			Commands.Bind.Invoke(Key.F1.WentDown(), "start");
			Commands.Bind.Invoke(Key.F2.WentDown(), "stop");
			Commands.Bind.Invoke(Key.F3.WentDown(), "connect 127.0.0.1:" + ServerProxy.DefaultPort);
			Commands.Bind.Invoke(Key.F4.WentDown(), "disconnect");
			Commands.Bind.Invoke(Key.F5.WentDown(), "reload_assets");
			Commands.Bind.Invoke(Key.C.WentDown(), "toggle_debug_cam");
			Commands.Bind.Invoke(Key.Escape.WentDown(), "exit");
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