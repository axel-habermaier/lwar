namespace Lwar.UserInterface.ViewModels
{
	using System;
	using System.Collections.Generic;
	using Gameplay;
	using Gameplay.Entities;
	using Network;
	using Network.Messages;
	using Pegasus;
	using Pegasus.Framework;
	using Pegasus.Framework.UserInterface;
	using Pegasus.Framework.UserInterface.Input;
	using Pegasus.Framework.UserInterface.ViewModels;
	using Pegasus.Platform;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;
	using Pegasus.Platform.Network;
	using Pegasus.Rendering;
	using Rendering;
	using Scripting;
	using Views;

	/// <summary>
	///     Displays a game session.
	/// </summary>
	public class GameSessionViewModel : StackedViewModel
	{
		/// <summary>
		///     The network session that synchronizes the game state between the client and the server.
		/// </summary>
		private readonly NetworkSession _networkSession;

		private readonly LogicalInput _respawn = new LogicalInput(MouseButton.Left.WentDown());
		private Camera2D _camera;

		/// <summary>
		///     The view model for the chat input.
		/// </summary>
		private ChatViewModel _chat;

		/// <summary>
		///     The game session that is played.
		/// </summary>
		private GameSession _gameSession;

		/// <summary>
		///     Manages the input provided by the user.
		/// </summary>
		private InputManager _inputManager;

		/// <summary>
		///     Indicates whether the connection to the server is lagging.
		/// </summary>
		private bool _isLagging;

		/// <summary>
		///     Indicates whether the game session is being loaded.
		/// </summary>
		private bool _isLoading = true;

		/// <summary>
		///     The message dispatcher that is used to dispatch messages received from the server.
		/// </summary>
		private MessageDispatcher _messageDispatcher;

		/// <summary>
		///     The render context that is used to render the game session.
		/// </summary>
		private RenderContext _renderContext;

		/// <summary>
		///     The view model for the scoreboard.
		/// </summary>
		private ScoreboardViewModel _scoreboard;

		/// <summary>
		///     Indicates whether the user input should be sent to the server during the next update cycle.
		/// </summary>
		private bool _sendInput;

		private SpriteBatch _spriteBatch;

		/// <summary>
		///     The timer that is used to send user input to the server.
		/// </summary>
		private Timer _timer = new Timer(1000.0 / Specification.InputUpdateFrequency);

		/// <summary>
		///     The remaining number of seconds before the connection of the server is dropped.
		/// </summary>
		private double _waitForServerTimeout;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="serverEndPoint">The remote end point of the server.</param>
		public GameSessionViewModel(IPEndPoint serverEndPoint)
		{
			Commands.ShowConsole(false);
			CameraManager = new CameraManager(Application.Current.Window, Application.Current.GraphicsDevice, Application.Current.Window.InputDevice);
			View = new GameSessionView();

			_networkSession = new NetworkSession(serverEndPoint);
			_timer.Timeout += SendInputTimeout;
			_spriteBatch = new SpriteBatch(Application.Current.GraphicsDevice, Application.Current.Assets)
			{
				BlendState = BlendState.Premultiplied,
				DepthStencilState = DepthStencilState.DepthDisabled,
				SamplerState = SamplerState.PointClampNoMipmaps
			};
		}

		/// <summary>
		///     Gets the view model for the scoreboard.
		/// </summary>
		public ScoreboardViewModel Scoreboard
		{
			get { return _scoreboard; }
			private set { ChangePropertyValue(ref _scoreboard, value); }
		}

		/// <summary>
		///     Gets the view model for the chat input.
		/// </summary>
		public ChatViewModel Chat
		{
			get { return _chat; }
			private set { ChangePropertyValue(ref _chat, value); }
		}

		/// <summary>
		///     Gets the event messages that should be displayed.
		/// </summary>
		public IEnumerable<EventMessage> EventMessages
		{
			get
			{
				if (_gameSession != null)
					return _gameSession.EventMessages.Messages;

				return null;
			}
		}

		/// <summary>
		///     Gets the camera manager that toggles between the game camera and the debug camera.
		/// </summary>
		public CameraManager CameraManager { get; private set; }

		/// <summary>
		///     Gets the endpoint of the server that hosts the game session.
		/// </summary>
		public IPEndPoint ServerEndPoint
		{
			get { return _networkSession.ServerEndPoint; }
		}

		/// <summary>
		///     Gets a value indicating whether the game session is being loaded.
		/// </summary>
		public bool IsLoading
		{
			get { return _isLoading; }
			private set { ChangePropertyValue(ref _isLoading, value); }
		}

		/// <summary>
		///     Gets a value indicating whether the connection to the server is lagging.
		/// </summary>
		public bool IsLagging
		{
			get { return _isLagging; }
			private set { ChangePropertyValue(ref _isLagging, value); }
		}

		/// <summary>
		///     Gets the remaining number of seconds before the connection of the server is dropped.
		/// </summary>
		public double WaitForServerTimeout
		{
			get { return _waitForServerTimeout; }
			private set
			{
				if (IsLagging)
					ChangePropertyValue(ref _waitForServerTimeout, Math.Round(value));
			}
		}

		/// <summary>
		///     Sends the input to the server, if required.
		/// </summary>
		private void SendInput()
		{
			if (!_sendInput || !_networkSession.IsConnected)
				return;

			var message = _inputManager.CreateInputMessage();
			message.Input.Player = _gameSession.Players.LocalPlayer.Identifier;

			var worldCoordinates = CameraManager.GameCamera.ToWorldCoordinates(message.Input.Target);
			if (_gameSession.Players.LocalPlayer.Ship != null)
				message.Input.Target = worldCoordinates - _gameSession.Players.LocalPlayer.Ship.Position;

			_networkSession.Send(message);
			_sendInput = false;
		}

		/// <summary>
		///     Finalizes the game state synchronization and starts the game session.
		/// </summary>
		private void OnConnected()
		{
			var localPlayer = _gameSession.Players.LocalPlayer;
			Assert.NotNull(localPlayer, "Game state synced but local player is unknown.");

			_networkSession.Send(SelectionMessage.Create(localPlayer, EntityType.Ship,
				EntityType.Gun, EntityType.Phaser,
				EntityType.Phaser, EntityType.Phaser));

			_gameSession.EventMessages.Enabled = true;
			IsLoading = false;

			Commands.OnSay += OnSay;
			Cvars.PlayerNameChanged += OnPlayerNameChanged;

			// Resend player name, as it might have been changed during the connection attempt
			OnPlayerNameChanged(Cvars.PlayerName);

			Log.Info("Game state synced. Now connected to game session hosted by {0}.", _networkSession.ServerEndPoint);
		}

		/// <summary>
		///     Ensures that the user input is sent to the server during the next input cycle.
		/// </summary>
		private void SendInputTimeout()
		{
			_sendInput = true;
		}

		/// <summary>
		///     Invoked when the local player changed his or her name.
		/// </summary>
		/// <param name="name">The previous name of the local player.</param>
		private void OnPlayerNameChanged(string name)
		{
			_networkSession.Send(NameMessage.Create(_gameSession.Players.LocalPlayer, Cvars.PlayerName));
		}

		/// <summary>
		///     Invoked when the local player entered a chat message.
		/// </summary>
		/// <param name="message">The message that the local player wants to send.</param>
		private void OnSay(string message)
		{
			_networkSession.Send(ChatMessage.Create(_gameSession.Players.LocalPlayer, message));
		}

		/// <summary>
		///     Draws the 3D scene to the given render output.
		/// </summary>
		/// <param name="renderOutput">The renderoutput the current scene should be drawn to.</param>
		public void OnDraw(RenderOutput renderOutput)
		{
			renderOutput.ClearColor(new Color(0, 0, 0, 255));
			renderOutput.ClearDepth();

			_renderContext.Draw(renderOutput);

			var camera = renderOutput.Camera;
			renderOutput.Camera = _camera;

			_spriteBatch.BlendState = BlendState.Premultiplied;
			_spriteBatch.DepthStencilState = DepthStencilState.DepthDisabled;
			_spriteBatch.SamplerState = SamplerState.PointClampNoMipmaps;
			_camera.Viewport = renderOutput.Viewport;
			_renderContext.DrawUserInterface(_spriteBatch, CameraManager.GameCamera);
			_spriteBatch.DrawBatch(renderOutput);

			renderOutput.Camera = camera;
		}

		/// <summary>
		///     Activates the view model, presenting its content and view on the UI.
		/// </summary>
		protected override void OnActivated()
		{
			EntityTemplates.Initialize(Application.Current.GraphicsDevice, Application.Current.Assets);

			_renderContext = new RenderContext(Application.Current.GraphicsDevice, Application.Current.Assets);
			_gameSession = new GameSession(_renderContext);
			_messageDispatcher = new MessageDispatcher(_gameSession);
			_inputManager = new InputManager(Application.Current.Window.InputDevice);
			_camera = new Camera2D(Application.Current.GraphicsDevice);

			Scoreboard = new ScoreboardViewModel(_gameSession);
			Chat = new ChatViewModel();
			Application.Current.Window.InputDevice.Add(_respawn);

			_networkSession.OnConnected += OnConnected;
			_networkSession.OnDropped += () => ShowErrorBox("Connection Lost", "The connection to the server has been lost.");
			_networkSession.OnFaulted += () => ShowErrorBox("Connection Error", "The game session has been aborted due to a network error.");
			_networkSession.OnRejected += OnConnectionRejected;

			OnPropertyChanged("EventMessages");
			OnPropertyChanged("Players");
		}

		/// <summary>
		///     Shows an error message box explaining why the connection was rejected.
		/// </summary>
		/// <param name="reason">The reason for the rejection.</param>
		private void OnConnectionRejected(RejectReason reason)
		{
			switch (reason)
			{
				case RejectReason.Full:
					ShowErrorBox("Connection Rejected", "The server is full.");
					break;
				case RejectReason.VersionMismatch:
					ShowErrorBox("Connection Rejected", "The server uses an incompatible version of the network protocol.");
					break;
				default:
					Assert.NotReached("Unknown reject reason.");
					break;
			}
		}

		/// <summary>
		///     Deactivates the view model, removing its content and view from the UI.
		/// </summary>
		protected override void OnDeactivated()
		{
			_timer.Timeout -= SendInputTimeout;

			CameraManager.SafeDispose();
			_inputManager.SafeDispose();
			_spriteBatch.SafeDispose();
			_gameSession.SafeDispose();
			_renderContext.SafeDispose();
			_networkSession.SafeDispose();
			EntityTemplates.Dispose();
			_camera.SafeDispose();
			Scoreboard.SafeDispose();

			Commands.OnSay -= OnSay;
			Cvars.PlayerNameChanged -= OnPlayerNameChanged;

			Application.Current.Window.InputDevice.Remove(_respawn);

			Log.Info("The game session has ended.");
		}

		/// <summary>
		///     Updates the view model's state.
		/// </summary>
		protected override void OnUpdate()
		{
			_networkSession.Update(_messageDispatcher);

			IsLagging = _networkSession.IsLagging;
			WaitForServerTimeout = _networkSession.TimeToDrop / 1000;

			if (!_networkSession.IsConnected)
				return;

			SendInput();

			_timer.Update();
			_gameSession.Update();

			if (CameraManager.GameCamera.IsActive)
				_inputManager.Update();

			CameraManager.GameCamera.Ship = _gameSession.Players.LocalPlayer.Ship;
			CameraManager.Update();

			if (!_networkSession.IsSyncing)
				_scoreboard.Update();

			var localPlayer = _gameSession.Players.LocalPlayer;
			if (localPlayer != null && localPlayer.Ship == null && _respawn.IsTriggered)
				_networkSession.Send(SelectionMessage.Create(localPlayer, EntityType.Ship,
					EntityType.Gun, EntityType.Phaser,
					EntityType.Phaser, EntityType.Phaser));
		}

		/// <summary>
		///     Opens the main menu and shows a message box with the given header and error message.
		/// </summary>
		/// <param name="header">The header of the message box.</param>
		/// <param name="message">The message that the message box should display.</param>
		private void ShowErrorBox(string header, string message)
		{
			Log.Error("{0}", message);
			MessageBox.Show(header, message);

			var mainMenu = new MainMenuViewModel();
			Root.ReplaceChild(mainMenu);
		}

		/// <summary>
		///     Shows the chat input.
		/// </summary>
		public void ShowChatInput()
		{
			_chat.IsVisible = true;
		}

		/// <summary>
		///     Shows the scoreboard.
		/// </summary>
		public void ShowScoreboard()
		{
			_scoreboard.IsVisible = true;
		}

		/// <summary>
		///     Hides the scoreboard.
		/// </summary>
		public void HideScoreboard()
		{
			_scoreboard.IsVisible = false;
		}
	}
}