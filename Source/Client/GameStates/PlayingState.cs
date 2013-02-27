using System;

namespace Lwar.Client.GameStates
{
	using System.Net;
	using Gameplay;
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Rendering;

	/// <summary>
	///   Displays a game session.
	/// </summary>
	public class PlayingState : GameState
	{
		/// <summary>
		///   The game session that is played.
		/// </summary>
		private readonly GameSession _gameSession;

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
		///   Manages the input provided by the user.
		/// </summary>
		private InputManager _inputManager;

		/// <summary>
		///   The render context that is used to render the game session.
		/// </summary>
		private RenderContext _renderContext;

		/// <summary>
		///   Indicates whether the user input should be sent to the server during the next update cycle.
		/// </summary>
		private bool _sendInput;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="serverEndPoint">The remote end point of the server.</param>
		public PlayingState(IPEndPoint serverEndPoint)
		{
			Assert.ArgumentNotNull(serverEndPoint, () => serverEndPoint);

			_networkSession = new NetworkSession(serverEndPoint);
			_gameSession = new GameSession();

			IsOpaque = true;
			_timer.Timeout += SendInput;

			Log.Info("Connecting to {0}...", serverEndPoint);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_timer.Timeout -= SendInput;
			_timer.SafeDispose();

			_cameraManager.SafeDispose();
			_inputManager.SafeDispose();

			_networkSession.SafeDispose();
			_gameSession.SafeDispose();
			_renderContext.SafeDispose();

			Log.Info("The game session has ended.");
		}

		/// <summary>
		///   Initializes the game state.
		/// </summary>
		public override void Initialize()
		{
			_renderContext = new RenderContext(Window, GraphicsDevice, Assets);
			_cameraManager = new CameraManager(Window, GraphicsDevice, InputDevice);
			_inputManager = new InputManager(InputDevice);

			StateManager.Add(new LoadingState(_gameSession, _networkSession));
		}

		/// <summary>
		///   Updates the game state.
		/// </summary>
		/// <param name="topmost">Indicates whether the game state is the topmost one.</param>
		public override void Update(bool topmost)
		{
			if (_sendInput && _networkSession.IsConnected)
			{
				var message = _inputManager.CreateInputMessage(_gameSession.LocalPlayer, _cameraManager.GameCamera, Window.Size);
				_networkSession.Send(message);
				_sendInput = false;
			}

			_timer.Update();
			_networkSession.Update(_gameSession, _renderContext);

			if (_networkSession.IsConnected)
			{
				_gameSession.Update();
				_inputManager.Update();
			}

			if (_networkSession.IsDropped)
				ShowMessageBox("The connection to the server has been lost.", LogType.Error, true);

			if (_networkSession.IsFaulted)
				ShowMessageBox("The game session has been aborted due to a network error.", LogType.Error, true);

			if (_networkSession.IsLagging && topmost)
				StateManager.Add(new WaitingForServerState(_networkSession));
		}

		/// <summary>
		///   Ensures that the user input is sent to the server during the next input cycle.
		/// </summary>
		private void SendInput()
		{
			_sendInput = true;
		}

		/// <summary>
		///   Draws the game state.
		/// </summary>
		public override void Draw()
		{
			_renderContext.Draw(_cameraManager.ActiveCamera);
		}
	}
}