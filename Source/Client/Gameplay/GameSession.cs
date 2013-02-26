using System;

namespace Lwar.Client.Gameplay
{
	using System.Net;
	using System.Threading.Tasks;
	using Network;
	using Network.Messages;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Network;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Assets;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Input;
	using Pegasus.Framework.Processes;
	using Pegasus.Framework.Rendering;
	using Pegasus.Framework.Rendering.UserInterface;
	using Pegasus.Framework.Scripting;
	using Rendering;

	/// <summary>
	///   Represents a game session.
	/// </summary>
	public class GameSession : DisposableObject
	{
		/// <summary>
		///   The amount of time in milliseconds that the session should wait after a full game state synchronization before
		///   entering the playing state.
		/// </summary>
		private const int EnterPlayingStateDelay = 250;

		/// <summary>
		///   The font that is used to draw the text of the loading screen.
		/// </summary>
		[Asset("Fonts/Liberation Mono 12")]
		public static Font LoadingFont;

		/// <summary>
		///   The debug camera that can be used to freely navigate the scene.
		/// </summary>
		private readonly DebugCamera _debugCamera;

		/// <summary>
		///   The process scheduler that schedules all draw processes of the game session.
		/// </summary>
		private readonly ProcessScheduler _drawScheduler;

		/// <summary>
		///   Manages the draw state of the game session.
		/// </summary>
		private readonly StateMachine _drawState;

		/// <summary>
		///   The sprite batch that is used to draw the connection state messages.
		/// </summary>
		private readonly SpriteBatch _spriteBatch;

		/// <summary>
		///   Manages the update state of the game session.
		/// </summary>
		private readonly StateMachine _updateState;

		/// <summary>
		///   Manages the input state and periodically sends updates to the server.
		/// </summary>
		private InputManager _inputManager;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="window">The window that displays the game session.</param>
		/// <param name="graphicsDevice">The graphics device that is used to draw the game session.</param>
		/// <param name="assets">The assets manager that manages all assets of the game session.</param>
		/// <param name="inputDevice">The logical input device that provides all the user input to the game session.</param>
		public GameSession(Window window, GraphicsDevice graphicsDevice, AssetsManager assets, LogicalInputDevice inputDevice)
		{
			Assert.ArgumentNotNull(window, () => window);
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(assets, () => assets);
			Assert.ArgumentNotNull(inputDevice, () => inputDevice);

			Window = window;
			GraphicsDevice = graphicsDevice;
			Assets = assets;
			InputDevice = inputDevice;
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			Scheduler = new ProcessScheduler();
			_updateState = StateMachine.Create(Scheduler);
			_drawScheduler = new ProcessScheduler();
			_drawState = StateMachine.Create(_drawScheduler);
			RenderContext = new RenderContext(graphicsDevice, assets);

			Camera = new Camera3D(graphicsDevice) { FieldOfView = MathUtils.DegToRad(20), Up = new Vector3(0, 0, 1) };
			_debugCamera = new DebugCamera(graphicsDevice, inputDevice, Scheduler);
			RenderContext.Camera = Camera;
			InputDevice.Modes = InputModes.Game;

			LwarCommands.Connect.Invoked += Connect;
			LwarCommands.Disconnect.Invoked += () => _updateState.ChangeState(Inactive);
			LwarCommands.Chat.Invoked += ChatMessageEntered;
			LwarCommands.ToggleDebugCamera.Invoked += ToggleDebugCamera;
			Cvars.PlayerName.Changed += PlayerNameChanged;
			Window.Resized += WindowResized;

			WindowResized(Window.Size);
			_updateState.ChangeState(Inactive);
		}

		/// <summary>
		///   The render context that is used to draw the game session.
		/// </summary>
		public RenderContext RenderContext { get; private set; }

		/// <summary>
		///   Gets the camera that is used to draw the game session.
		/// </summary>
		public Camera3D Camera { get; private set; }

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
		///   Gets the assets manager that manages all assets of the game session.
		/// </summary>
		public AssetsManager Assets { get; private set; }

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
		public PlayerList Players { get; private set; }

		/// <summary>
		///   Gets the player instance for the local player.
		/// </summary>
		public Player LocalPlayer { get; set; }

		/// <summary>
		///   Gets the proxy to the server that hosts the game session.
		/// </summary>
		public ServerProxy ServerProxy { get; private set; }

		/// <summary>
		///   Connects to a game session on the server identified by the given end point.
		/// </summary>
		/// <param name="endPoint">The end point of the server that hosts the game session.</param>
		private void Connect(IPEndPoint endPoint)
		{
			Assert.ArgumentNotNull(endPoint, () => endPoint);

			if (endPoint.Port == 0)
				endPoint.Port = Specification.DefaultServerPort;

			_updateState.ChangeState(ctx => Loading(ctx, endPoint));
		}

		/// <summary>
		///   Toggles between the game and the debug camera.
		/// </summary>
		private void ToggleDebugCamera()
		{
			if (RenderContext.Camera == _debugCamera)
			{
				RenderContext.Camera = Camera;
				InputDevice.Modes = InputModes.Game;
				Window.MouseCaptured = false;
			}
			else
			{
				RenderContext.Camera = _debugCamera;
				InputDevice.Modes = InputModes.Debug;
				Window.MouseCaptured = true;
				_debugCamera.Reset();
			}
		}

		/// <summary>
		///   Invoked when the player changed his or her name.
		/// </summary>
		/// <param name="name">The new name of the player.</param>
		private void PlayerNameChanged(string name)
		{
			if (ServerProxy != null && (ServerProxy.IsConnected || ServerProxy.IsSyncing))
				ServerProxy.Send(NameMessage.Create(LocalPlayer.Id, name));
		}

		/// <summary>
		///   Invoked when a chat message should be sent to the server.
		/// </summary>
		/// <param name="message">The message that should be sent.</param>
		private void ChatMessageEntered(string message)
		{
			if (ServerProxy != null && ServerProxy.IsConnected)
				ServerProxy.Send(ChatMessage.Create(LocalPlayer.Id, message));
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			RenderContext.SafeDispose();
			Camera.SafeDispose();
			_debugCamera.SafeDispose();
			Entities.SafeDispose();
			_spriteBatch.SafeDispose();
			_inputManager.SafeDispose();
			_drawState.SafeDispose();
			_updateState.SafeDispose();
			ServerProxy.SafeDispose();
			Scheduler.SafeDispose();
			_drawScheduler.SafeDispose();

			if (Window != null)
				Window.Resized -= WindowResized;
		}

		/// <summary>
		///   Updates the state of the session.
		/// </summary>
		public void Update()
		{
			Scheduler.RunProcesses();
		}

		/// <summary>
		///   Processes the given message.
		/// </summary>
		/// <param name="message">The message that should be processed.</param>
		private void ProcessMessage(IMessage message)
		{
			Assert.ArgumentNotNull(message, () => message);
			message.Process(this);
		}

		/// <summary>
		///   Draws the session state to the screen.
		/// </summary>
		public void Draw()
		{
			_drawScheduler.RunProcesses();
			_spriteBatch.DrawBatch();
		}

		/// <summary>
		///   Updates the projection matrix such that one pixel corresponds to one floating point unit.
		/// </summary>
		/// <param name="windowSize">The new size of the window.</param>
		private void WindowResized(Size windowSize)
		{
			var viewport = new Rectangle(0, 0, Window.Size.Width, Window.Size.Height);

			Camera.Viewport = viewport;
			_debugCamera.Viewport = viewport;

			_spriteBatch.ProjectionMatrix = Matrix.CreateOrthographic(0, windowSize.Width, windowSize.Height, 0, 0, 1);
		}

		/// <summary>
		///   Cleans up all session-specific state.
		/// </summary>
		private void Cleanup()
		{
			Entities.SafeDispose();
			ServerProxy.SafeDispose();
			_inputManager.SafeDispose();

			Entities = null;
			Players = null;
			LocalPlayer = null;
			ServerProxy = null;
			_inputManager = null;
		}

		/// <summary>
		///   Active when the game session is inactive.
		/// </summary>
		/// <param name="context">The context in which the state function should be executed.</param>
		private async Task Inactive(ProcessContext context)
		{
			if (ServerProxy != null)
				Log.Info("The game session has ended.");

			Cleanup();

			_drawState.ChangeStateDelayed(async ctx => await ctx.NextFrame());
			await context.NextFrame();
		}

		/// <summary>
		///   Active when a connection to a server is established and the game is loaded.
		/// </summary>
		/// <param name="context">The context in which the state function should be executed.</param>
		/// <param name="serverEndPoint">The IP end point of the server that hosts the game session.</param>
		private async Task Loading(ProcessContext context, IPEndPoint serverEndPoint)
		{
			Assert.ArgumentNotNull(serverEndPoint, () => serverEndPoint);

			Commands.ShowConsole.Invoke(false);
			Cleanup();

			Entities = new EntityList(this);
			Players = new PlayerList();
			ServerProxy = new ServerProxy(new LwarPacketFactory(), serverEndPoint, Scheduler);
			ServerProxy.MessageReceived += ProcessMessage;

			var label = new Label(LoadingFont)
			{
				Alignment = TextAlignment.Centered | TextAlignment.Middle,
				Text = String.Format("Connecting to {0}...", serverEndPoint)
			};

			_drawState.ChangeState(ctx => LoadingDraw(ctx, label));
			await ServerProxy.Connect(context);

			if (!ServerProxy.IsSyncing && !ServerProxy.IsConnected)
			{
				if (ServerProxy.ServerIsFull)
					NetworkLog.ClientError("Unable to connect to {0}: The server is full.", serverEndPoint);

				Commands.ShowConsole.Invoke(true);
				_updateState.ChangeStateDelayed(Inactive);
				return;
			}

			ServerProxy.Send(NameMessage.Create(LocalPlayer.Id, Cvars.PlayerName.Value));
			ServerProxy.Send(SelectionMessage.Create(LocalPlayer.Id, EntityTemplate.Ship, EntityTemplate.Gun, 0, 0, 0));

			label.Text = "Awaiting game state...";
			await context.WaitFor(() => ServerProxy.IsConnected || ServerProxy.IsDropped);

			if (!ServerProxy.IsConnected)
			{
				NetworkLog.ClientError(ServerProxy.IsFaulted
										   ? "An error occurred during the synchronization of the game state."
										   : "Game state synchronization failed: The server did not respond.");

				Commands.ShowConsole.Invoke(true);
				_updateState.ChangeStateDelayed(Inactive);
				return;
			}

			label.Text = "Synchronizing...";
			await context.Delay(EnterPlayingStateDelay);

			_inputManager = new InputManager(this);
			_updateState.ChangeStateDelayed(Playing);
		}

		/// <summary>
		///   Draws the loading screen.
		/// </summary>
		/// <param name="context">The context in which the state function should be executed.</param>
		/// <param name="label">The label describing the current loading state.</param>
		private async Task LoadingDraw(ProcessContext context, Label label)
		{
			while (!context.IsCanceled)
			{
				label.Area = new Rectangle(Vector2i.Zero, Window.Size);
				label.Draw(_spriteBatch);
				await context.NextFrame();
			}
		}

		/// <summary>
		///   Active when the game session is running.
		/// </summary>
		/// <param name="context">The context in which the state function should be executed.</param>
		private async Task Playing(ProcessContext context)
		{
			_drawState.ChangeState(PlayingDraw);

			while (!context.IsCanceled)
			{
				Entities.Update();

				if (LocalPlayer.Ship != null)
				{
					var position = LocalPlayer.Ship.Position;
					Camera.Position = new Vector3(position.X, 1500, position.Y);
					Camera.Target = new Vector3(position.X, 0, position.Y);
				}

				if (ServerProxy.IsLagging)
					_updateState.ChangeStateDelayed(WaitingForServer);

				await context.NextFrame();
			}
		}

		/// <summary>
		///   Draws the game session.
		/// </summary>
		/// <param name="context">The context in which the state function should be executed.</param>
		private async Task PlayingDraw(ProcessContext context)
		{
			while (!context.IsCanceled)
			{
				DrawEntities();
				await context.NextFrame();
			}
		}

		/// <summary>
		///   Draws the entities with the current camera.
		/// </summary>
		private void DrawEntities()
		{
			RenderContext.BeginFrame();
			Entities.Draw();
		}

		/// <summary>
		///   Pauses the game session if the connection to the server is lagging.
		/// </summary>
		/// <param name="context">The context in which the state function should be executed.</param>
		private async Task WaitingForServer(ProcessContext context)
		{
			var label = new Label(LoadingFont) { Alignment = TextAlignment.Centered | TextAlignment.Middle };
			_drawState.ChangeState(ctx => WaitingForServerDraw(ctx, label));

			while (!context.IsCanceled && ServerProxy.IsLagging && !ServerProxy.IsDropped && !ServerProxy.IsFaulted)
			{
				label.Text = String.Format("Waiting for server ({0} seconds)...", (int)(ServerProxy.TimeToDrop / 1000) + 1);
				await context.NextFrame();
			}

			if (ServerProxy.IsDropped || ServerProxy.IsFaulted)
			{
				if (ServerProxy.IsDropped)
					NetworkLog.ClientError("The connection to the server has been dropped.");

				Commands.ShowConsole.Invoke(true);
				_updateState.ChangeStateDelayed(Inactive);
			}
			else if (ServerProxy.IsConnected)
				_updateState.ChangeStateDelayed(Playing);
			else
				Assert.That(false, "Unexpected connection state.");
		}

		/// <summary>
		///   Draws the game session if the connection to the server is lagging.
		/// </summary>
		/// <param name="context">The context in which the state function should be executed.</param>
		/// <param name="label">The label describing the current waiting state.</param>
		private async Task WaitingForServerDraw(ProcessContext context, Label label)
		{
			while (!context.IsCanceled)
			{
				DrawEntities();

				label.Area = new Rectangle(Vector2i.Zero, Window.Size);
				label.Draw(_spriteBatch);

				await context.NextFrame();
			}
		}
	}
}