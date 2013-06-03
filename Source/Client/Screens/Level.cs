using System;

namespace Lwar.Client.Screens
{
	using System.Net;
	using Gameplay;
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Logging;
	using Pegasus.Framework.Platform.Memory;
	using Pegasus.Framework.Rendering;
	using Rendering;
	using Scripting;

	/// <summary>
	///   Displays the entities of a game session.
	/// </summary>
	public class Level : Screen
	{
		/// <summary>
		///   The network session that synchronizes the game state between the client and the server.
		/// </summary>
		private readonly NetworkSession _networkSession;

		/// <summary>
		///   The timer that is used to send user input to the server.
		/// </summary>
		private readonly Timer _timer = Timer.Create(1000.0 / Specification.InputUpdateFrequency);

		/// <summary>
		///   Manages the game and debug cameras.
		/// </summary>
		private CameraManager _cameraManager;

		/// <summary>
		///   The game session that is played.
		/// </summary>
		private GameSession _gameSession;

		/// <summary>
		///   Manages the input provided by the user.
		/// </summary>
		private InputManager _inputManager;

		/// <summary>
		///   The message dispatcher that is used to dispatch messages received from the server.
		/// </summary>
		private MessageDispatcher _messageDispatcher;

		/// <summary>
		///   The render context that is used to render the game session.
		/// </summary>
		private RenderContext _renderContext;

		/// <summary>
		///   The scoreboard that is currently being shown, or null if the scoreboard is invisible.
		/// </summary>
		private Scoreboard _scoreboard;

		/// <summary>
		///   Indicates whether the user input should be sent to the server during the next update cycle.
		/// </summary>
		private bool _sendInput;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="serverEndPoint">The remote end point of the server.</param>
		public Level(IPEndPoint serverEndPoint)
		{
			Assert.ArgumentNotNull(serverEndPoint);

			_networkSession = new NetworkSession(serverEndPoint);
			IsOpaque = true;
			_timer.Timeout += SendInputTimeout;

			Log.Info(LogCategory.Client, "Connecting to {0}...", serverEndPoint);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			if (_scoreboard != null)
				ScreenManager.Remove(_scoreboard);

			_timer.Timeout -= SendInputTimeout;
			_timer.SafeDispose();

			_cameraManager.SafeDispose();
			_inputManager.SafeDispose();

			_networkSession.SafeDispose();
			_gameSession.SafeDispose();
			_renderContext.SafeDispose();

			Commands.OnShowScoreboard -= OnShowScoreboard;
			Commands.OnSay -= OnSay;
			Cvars.PlayerNameChanged -= OnPlayerNameChanged;

			Log.Info(LogCategory.Client, "The game session has ended.");
		}

		/// <summary>
		///   Initializes the screen.
		/// </summary>
		public override void Initialize()
		{
			_renderContext = new RenderContext(GraphicsDevice, Assets);
			_gameSession = new GameSession(_renderContext);
			_messageDispatcher = new MessageDispatcher(_gameSession);
			_cameraManager = new CameraManager(Window, GraphicsDevice, InputDevice);
			_inputManager = new InputManager(InputDevice);

			Commands.OnShowScoreboard += OnShowScoreboard;
			Commands.OnSay += OnSay;
			Cvars.PlayerNameChanged += OnPlayerNameChanged;

			ScreenManager.Add(new Loading(_gameSession, _networkSession));
		}

		/// <summary>
		///   Updates the screen.
		/// </summary>
		/// <param name="topmost">Indicates whether the app screen is the topmost one.</param>
		public override void Update(bool topmost)
		{
			SendInput();
			_timer.Update();
			_networkSession.Update(_messageDispatcher);

			if (_networkSession.IsConnected)
			{
				_gameSession.Update();
				_inputManager.Update();

				_cameraManager.GameCamera.Ship = _gameSession.Players.LocalPlayer.Ship;
				_cameraManager.Update();
			}

			if (_networkSession.IsDropped)
				MessageBox.Show(this, LogCategory.Client, LogType.Error, "The connection to the server has been lost.", true);

			if (_networkSession.IsFaulted)
				MessageBox.Show(this, LogCategory.Client, LogType.Error, "The game session has been aborted due to a network error.", true);

			if (_networkSession.IsLagging && topmost)
				ScreenManager.Add(new WaitingForServer(_networkSession));
		}

		/// <summary>
		///   Draws the screen.
		/// </summary>
		/// <param name="output">The output that the screen should render to.</param>
		public override void Draw(RenderOutput output)
		{
			Assert.ArgumentNotNull(output);

			output.Camera = _cameraManager.ActiveCamera;
			output.Camera.Viewport = output.Viewport;

			_renderContext.Draw(output);
		}

		/// <summary>
		///   Sends the input to the server, if required.
		/// </summary>
		private void SendInput()
		{
			if (!_sendInput || !_networkSession.IsConnected)
				return;

			var message = _inputManager.CreateInputMessage();
			message.Input.Player = _gameSession.Players.LocalPlayer.Id;
			message.Input.Target = _cameraManager.GameCamera.ToWorldCoordinates(message.Input.Target);

			_networkSession.Send(message);
			_sendInput = false;
		}

		/// <summary>
		///   Ensures that the user input is sent to the server during the next input cycle.
		/// </summary>
		private void SendInputTimeout()
		{
			_sendInput = true;
		}

		/// <summary>
		///   Shows or hides the scoreboard.
		/// </summary>
		/// <param name="show">If true, the scoreboard is shown.</param>
		private void OnShowScoreboard(bool show)
		{
			if (show)
			{
				if (_scoreboard != null)
					return;

				_scoreboard = new Scoreboard(_gameSession);
				ScreenManager.Add(_scoreboard);
			}
			else
			{
				if (_scoreboard == null)
					return;

				ScreenManager.Remove(_scoreboard);
				_scoreboard = null;
			}
		}

		/// <summary>
		///   Invoked when the local player changed his or her name.
		/// </summary>
		/// <param name="name">The previous name of the local player.</param>
		private void OnPlayerNameChanged(string name)
		{
			_networkSession.Send(Message.ChangePlayerName(_gameSession.Players.LocalPlayer, Cvars.PlayerName));
		}

		/// <summary>
		///   Invoked when the local player entered a chat message.
		/// </summary>
		/// <param name="message">The message that the local player wants to send.</param>
		private void OnSay(string message)
		{
			_networkSession.Send(Message.Say(_gameSession.Players.LocalPlayer, message));
		}
	}
}