using System;

namespace Lwar.Client.Gameplay
{
	using System.Collections.Generic;
	using System.Net;
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Input;
	using Pegasus.Framework.Processes;
	using Pegasus.Framework.Rendering;
	using Pegasus.Framework.Rendering.UserInterface;
	using Pegasus.Framework.Scripting;

	/// <summary>
	///   Represents a game session.
	/// </summary>
	public class GameSession : DisposableObject
	{
		/// <summary>
		///   The font that is used to draw the text of the loading screen.
		/// </summary>
		[Asset("Fonts/Liberation Mono 12")]
		public static Font LoadingFont;

		/// <summary>
		///   The process scheduler that schedules all draw processes of the game session.
		/// </summary>
		private readonly ProcessScheduler _drawScheduler;

		/// <summary>
		///   Manages the draw state of the game session.
		/// </summary>
		private readonly StateMachine _drawState;

		/// <summary>
		///   Manages the update state of the game session.
		/// </summary>
		private readonly StateMachine _updateState;

		/// <summary>
		///   The process that handles incoming server messages.
		/// </summary>
		private IProcess _handleServerMessagesProcess;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public GameSession(Window window, GraphicsDevice graphicsDevice, LogicalInputDevice inputDevice)
		{
			Assert.ArgumentNotNull(window, () => window);
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(inputDevice, () => inputDevice);

			Window = window;
			GraphicsDevice = graphicsDevice;
			InputDevice = inputDevice;
			SpriteBatch = new SpriteBatch(GraphicsDevice);
			Scheduler = new ProcessScheduler();
			_updateState = StateMachine.Create(Scheduler);
			_drawScheduler = new ProcessScheduler();
			_drawState = StateMachine.Create(_drawScheduler);
			Window.Resized += UpdateProjectionMatrix;
			UpdateProjectionMatrix(Window.Size);

			LwarCommands.Connect.Invoked += LoadGame;
			LwarCommands.Disconnect.Invoked += Inactive;
			Inactive();
		}

		/// <summary>
		///   Gets or sets the window that displays the game session.
		/// </summary>
		public Window Window { get; private set; }

		/// <summary>
		///   Gets the logical input device that provides all the user input to the game session.
		/// </summary>
		public LogicalInputDevice InputDevice { get; private set; }

		/// <summary>
		///   Gets the graphics device that is used to draw the game session.
		/// </summary>
		public GraphicsDevice GraphicsDevice { get; private set; }

		/// <summary>
		///   Gets the sprite batch that is used to draw all sprites.
		/// </summary>
		public SpriteBatch SpriteBatch { get; private set; }

		/// <summary>
		///   Gets the process scheduler that schedules all update processes of the game session.
		/// </summary>
		public ProcessScheduler Scheduler { get; private set; }

		/// <summary>
		///   Gets the entities of the game session.
		/// </summary>
		public EntityList Entities { get; private set; }

		/// <summary>
		///   Gets the list of players that participate in the game session.
		/// </summary>
		public List<Player> Players { get; private set; }

		/// <summary>
		///   Gets the proxy to the server that hosts the game session.
		/// </summary>
		public ServerProxy ServerProxy { get; private set; }

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Entities.SafeDispose();
			SpriteBatch.SafeDispose();
			_drawState.SafeDispose();
			_updateState.SafeDispose();
			_handleServerMessagesProcess.SafeDispose();
			ServerProxy.SafeDispose();
			Scheduler.SafeDispose();
			_drawScheduler.SafeDispose();

			if (Window != null)
				Window.Resized -= UpdateProjectionMatrix;
		}

		/// <summary>
		///   Updates the state of the session.
		/// </summary>
		public void Update()
		{
			Scheduler.RunProcesses();
		}

		/// <summary>
		///   Draws the session state to the screen.
		/// </summary>
		public void Draw()
		{
			_drawScheduler.RunProcesses();
			SpriteBatch.DrawBatch();
		}

		/// <summary>
		///   Updates the projection matrix such that one pixel corresponds to one floating point unit.
		/// </summary>
		/// <param name="windowSize">The new size of the window.</param>
		private void UpdateProjectionMatrix(Size windowSize)
		{
			SpriteBatch.ProjectionMatrix = Matrix.OrthographicProjection(0, windowSize.Width, windowSize.Height, 0, 0, 1);
		}

		/// <summary>
		///   Cleans up all session-specific state.
		/// </summary>
		private void Cleanup()
		{
			Entities.SafeDispose();
			_handleServerMessagesProcess.SafeDispose();
			ServerProxy.SafeDispose();

			Entities = null;
			_handleServerMessagesProcess = null;
			ServerProxy = null;
		}

		/// <summary>
		///   Sets the game session into its inactive state.
		/// </summary>
		private void Inactive()
		{
			Cleanup();

			// TODO: Implement menu, server selection, etc.
			_updateState.ChangeStateDelayed(async ctx => await ctx.NextFrame());
			_drawState.ChangeStateDelayed(async ctx => await ctx.NextFrame());
		}

		/// <summary>
		///   Active when a connection to a server is established and the game is loaded.
		/// </summary>
		/// <param name="serverEndPoint">The end point of the server that hosts the game session.</param>
		private void LoadGame(IPEndPoint serverEndPoint)
		{
			Commands.ShowConsole.Invoke(false);
			Cleanup();

			Entities = new EntityList(this);
			ServerProxy = new ServerProxy(serverEndPoint);
			_handleServerMessagesProcess = Scheduler.CreateProcess(ServerProxy.HandleServerMessages);

			var label = new Label(LoadingFont) { Alignment = TextAlignment.Centered | TextAlignment.Middle };

			_updateState.ChangeStateDelayed(async ctx =>
				{
					label.Text = String.Format("Connecting to {0}...", serverEndPoint);
					await ServerProxy.Connect(ctx);

					if (!ServerProxy.IsConnected)
					{
						Commands.ShowConsole.Invoke(true);
						Inactive();
						return;
					}

					Play();
				});

			_drawState.ChangeStateDelayed(async ctx =>
				{
					while (!ctx.IsCanceled)
					{
						label.Area = new Rectangle(Vector2i.Zero, Window.Size);
						label.Draw(SpriteBatch);
						await ctx.NextFrame();
					}
				});
		}

		/// <summary>
		///   Active when a game session is running.
		/// </summary>
		private void Play()
		{
			_updateState.ChangeStateDelayed(async ctx =>
				{
					while (!ctx.IsCanceled)
					{
						Entities.Update();
						await ctx.NextFrame();
					}
				});

			_drawState.ChangeStateDelayed(async ctx =>
				{
					while (!ctx.IsCanceled)
					{
						Entities.Draw();
						await ctx.NextFrame();
					}
				});
		}
	}
}