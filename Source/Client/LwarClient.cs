using System;

namespace Lwar.Client
{
	using System.Net;
	using GameStates;
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Input;
	using Pegasus.Framework.Platform.Memory;
	using Pegasus.Framework.Rendering;
	using Scripting;

	/// <summary>
	///   Represents the lwar client.
	/// </summary>
	internal sealed class LwarClient : App
	{
		/// <summary>
		///   The local game server that can be used to hosts game sessions locally.
		/// </summary>
		private LocalServer _localServer;

		/// <summary>
		///   The state manager that manages the states of the application.
		/// </summary>
		private AppStateManager _stateManager;

		/// <summary>
		///   Invoked when the application is initializing.
		/// </summary>
		protected override void Initialize()
		{
			var commands = (CommandRegistry)Context.Commands;
			commands.OnConnect += Connect;
			commands.OnDisconnect += Disconnect;

			Context.LogicalInputDevice.Modes = InputModes.Game;
			Context.Window.Closing += Exit;
			Context.Window.Size = new Size(1280, 720);

			_localServer = new LocalServer(commands);
			_stateManager = new AppStateManager(Context);
			_stateManager.Add(new MainMenu());

			commands.Bind(Key.F1.WentDown(), "start_server");
			commands.Bind(Key.F2.WentDown(), "stop_server");
			commands.Bind(Key.F3.WentDown(), "connect 127.0.0.1");
			commands.Bind(Key.F4.WentDown(), "disconnect");
			commands.Bind(Key.F5.WentDown(), "reload_assets");
			commands.Bind(Key.C.WentDown(), "toggle_debug_camera");
			commands.Bind(Key.Escape.WentDown(), "exit");
		}

		/// <summary>
		///   Invoked when the application should update the its state.
		/// </summary>
		protected override void Update()
		{
			_localServer.Update();
			_stateManager.Update();
		}

		/// <summary>
		///   Invoked when the application should draw a frame.
		/// </summary>
		/// <param name="output">The output that the scene should be rendered to.</param>
		protected override void Draw(RenderOutput output)
		{
			output.ClearColor(new Color(0, 0, 0, 0));
			output.ClearDepth();

			_stateManager.Draw(output);
		}

		/// <summary>
		///   Invoked when the application should draw the user interface.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the user interface.</param>
		protected override void DrawUserInterface(SpriteBatch spriteBatch)
		{
			_stateManager.DrawUserInterface(spriteBatch);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void Dispose()
		{
			var commands = (CommandRegistry)Context.Commands;
			commands.OnConnect -= Connect;
			commands.OnDisconnect -= Disconnect;

			_stateManager.SafeDispose();
			_localServer.SafeDispose();
		}

		/// <summary>
		///   Connects to the server at the given end point and joins the game session.
		/// </summary>
		/// <param name="address">The IP address of the server.</param>
		/// <param name="port">The port of the server.</param>
		private void Connect(IPAddress address, ushort port)
		{
			Assert.ArgumentNotNull(address);

			Disconnect();
			_stateManager.Add(new Playing(new IPEndPoint(address, port)));
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