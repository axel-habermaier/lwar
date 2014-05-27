namespace Lwar.UserInterface
{
	using System;
	using Gameplay;
	using Gameplay.Entities;
	using Network;
	using Network.Messages;
	using Pegasus.Framework;
	using Pegasus.Platform;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Input;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;
	using Pegasus.Platform.Network;
	using Pegasus.Rendering;
	using Rendering;
	using Scripting;

	/// <summary>
	///     Displays a game session.
	/// </summary>
	public class InGameViewModel : LwarViewModel<InGameView>
	{
		/// <summary>
		///     The network session that synchronizes the game state between the client and the server.
		/// </summary>
		private readonly NetworkSession _networkSession;

		private readonly LogicalInput _respawn = new LogicalInput(MouseButton.Left.WentDown(), InputLayers.Game);

		private SpriteBatch _spriteBatch;

		/// <summary>
		///     The timer that is used to send user input to the server.
		/// </summary>
		private Timer _timer = new Timer(1000.0 / Specification.InputUpdateFrequency);

		/// <summary>
		///     The game session that is played.
		/// </summary>
		private GameSession _gameSession;

		/// <summary>
		///     Manages the input provided by the user.
		/// </summary>
		private InputManager _inputManager;

		/// <summary>
		///     The message dispatcher that is used to dispatch messages received from the server.
		/// </summary>
		private MessageDispatcher _messageDispatcher;

		/// <summary>
		///     The render context that is used to render the game session.
		/// </summary>
		private RenderContext _renderContext;

		/// <summary>
		///     Indicates whether the user input should be sent to the server during the next update cycle.
		/// </summary>
		private bool _sendInput;

		private Camera2D _camera;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="serverEndPoint">The remote end point of the server.</param>
		public InGameViewModel(IPEndPoint serverEndPoint)
		{
			OnDraw = Draw;
			CameraManager = new CameraManager(Application.Current.Window, Application.Current.GraphicsDevice, Application.Current.Window.InputDevice);
			View = new InGameView();

			_networkSession = new NetworkSession(serverEndPoint);
			_timer.Timeout += SendInputTimeout;
			_spriteBatch = new SpriteBatch(Application.Current.GraphicsDevice, Application.Current.Assets)
			{
				BlendState = BlendState.Premultiplied,
				DepthStencilState = DepthStencilState.DepthDisabled,
				SamplerState = SamplerState.PointClampNoMipmaps
			};
			
			Log.Info("Connecting to {0}...", serverEndPoint);
		}

		/// <summary>
		///     Gets the camera manager that toggles between the game camera and the debug camera.
		/// </summary>
		public CameraManager CameraManager { get; private set; }

		/// <summary>
		///     Gets the callback that should be used to redraw the 3D scene.
		/// </summary>
		public Action<RenderOutput> OnDraw { get; private set; }

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
		private void Draw(RenderOutput renderOutput)
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
			//_eventMessage.Draw(spriteBatch);
			//_scoreboard.Draw(spriteBatch);
			//_chatInput.Draw(spriteBatch);
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
			//_eventMessage = new EventMessageDisplay(Assets);

			Commands.OnSay += OnSay;
			Cvars.PlayerNameChanged += OnPlayerNameChanged;

			//ScreenManager.Add(new Loading(_gameSession, _networkSession));

			//_scoreboard = new Scoreboard(InputDevice, Assets, _gameSession);
			//_chatInput = new ChatInput(InputDevice, Assets);
			Application.Current.Window.InputDevice.Add(_respawn);

			Child = new LoadingViewModel(_gameSession, _networkSession);
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
			//_scoreboard.SafeDispose();
			//_chatInput.SafeDispose();
			_networkSession.SafeDispose();
			//_eventMessage.SafeDispose();
			EntityTemplates.Dispose();
			_camera.SafeDispose();

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
			SendInput();
			_timer.Update();
			_networkSession.Update(_messageDispatcher);

			if (_networkSession.IsConnected)
			{
				_gameSession.Update();
				_inputManager.Update();
				//_eventMessage.Update(_gameSession.EventMessages, Window.Size);

				CameraManager.GameCamera.Ship = _gameSession.Players.LocalPlayer.Ship;
				CameraManager.Update();
			}

			if (_networkSession.IsDropped)
			{
				//MessageBox.Show(this, LogType.Error, "The connection to the server has been lost.", true);
				return;
			}

			if (_networkSession.IsFaulted)
			{
				//MessageBox.Show(this, LogType.Error, "The game session has been aborted due to a network error.", true);
				return;
			}

			if (_networkSession.ServerIsFull || _networkSession.VersionMismatch)
			{
				//ScreenManager.Remove(this);
				return;
			}

			//if (_networkSession.IsLagging && topmost)
				//ScreenManager.Add(new WaitingForServer(_networkSession));

			if (!_networkSession.IsSyncing)
			{
				//_scoreboard.Update(Window.Size);
				//_chatInput.Update(Window.Size);
			}

			var localPlayer = _gameSession.Players.LocalPlayer;
			if (localPlayer != null && localPlayer.Ship == null && _respawn.IsTriggered)
				_networkSession.Send(SelectionMessage.Create(localPlayer, EntityType.Ship,
															 EntityType.Gun, EntityType.Phaser,
															 EntityType.Phaser, EntityType.Phaser));
		}
	}
}