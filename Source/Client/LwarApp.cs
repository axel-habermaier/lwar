using System;

namespace Lwar.Client
{
	using System.Net;
	using GameStates;
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Input;
	using Pegasus.Framework.Scripting;

	/// <summary>
	///   Represents the lwar client.
	/// </summary>
	internal sealed class LwarApp : App
	{
		/// <summary>
		///   The local game server that can be used to hosts game sessions locally.
		/// </summary>
		private readonly LocalServer _localServer = new LocalServer();

		/// <summary>
		///   The state manager that manages the states of the application.
		/// </summary>
		private StateManager _stateManager;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public LwarApp()
		{
			LwarCommands.Connect.Invoked += Connect;
			LwarCommands.Disconnect.Invoked += Disconnect;
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

			_stateManager = new StateManager(Window, GraphicsDevice, Assets, LogicalInputDevice);
			_stateManager.Add(new MainMenu());

			Commands.Bind.Invoke(Key.F1.WentDown(), "start");
			Commands.Bind.Invoke(Key.F2.WentDown(), "stop");
			Commands.Bind.Invoke(Key.F3.WentDown(), "connect 127.0.0.1");
			Commands.Bind.Invoke(Key.F4.WentDown(), "disconnect");
			Commands.Bind.Invoke(Key.F5.WentDown(), "reload_assets");
			Commands.Bind.Invoke(Key.C.WentDown(), "toggle_debug_cam");
			Commands.Bind.Invoke(Key.Escape.WentDown(), "exit");
		}

		/// <summary>
		///   Invoked when the application should update the game state.
		/// </summary>
		protected override void Update()
		{
			_localServer.Update();
			_stateManager.Update();
		}

		/// <summary>
		///   Invoked when the application should draw a frame.
		/// </summary>
		protected override void Draw()
		{
			SwapChain.BackBuffer.Bind();
			RasterizerState.CullNone.Bind();
			SwapChain.BackBuffer.Clear(Color.Black);

			_stateManager.Draw();
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			LwarCommands.Connect.Invoked -= Connect;
			LwarCommands.Disconnect.Invoked -= Disconnect;

			_stateManager.SafeDispose();
			_localServer.SafeDispose();

			base.OnDisposing();
		}

		/// <summary>
		///   Connects to the server at the given end point and joins the game session.
		/// </summary>
		/// <param name="serverEndPoint">The remote end point of the server that hosts the game session.</param>
		private void Connect(IPEndPoint serverEndPoint)
		{
			Assert.ArgumentNotNull(serverEndPoint, () => serverEndPoint);

			if (serverEndPoint.Port == 0)
				serverEndPoint.Port = Specification.DefaultServerPort;

			Disconnect();
			_stateManager.Add(new Playing(serverEndPoint));
		}

		/// <summary>
		///   Disconnects from the game session the client is currently connected to.
		/// </summary>
		private void Disconnect()
		{
			_stateManager.Clear();
			_stateManager.Add(new MainMenu());
		}
	}
}