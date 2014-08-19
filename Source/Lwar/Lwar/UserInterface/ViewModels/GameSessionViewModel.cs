namespace Lwar.UserInterface.ViewModels
{
	using System;
	using System.Collections.Generic;
	using Gameplay;
	using Gameplay.Entities;
	using Network.Messages;
	using Pegasus;
	using Pegasus.Framework;
	using Pegasus.Framework.UserInterface.Controls;
	using Pegasus.Framework.UserInterface.ViewModels;
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
		///     The game session that is played.
		/// </summary>
		private GameSession _gameSession;

		/// <summary>
		///     Indicates whether the connection to the server is lagging.
		/// </summary>
		private bool _isLagging;

		/// <summary>
		///     Indicates whether the 3D scene should be rendered using the global app resolution.
		/// </summary>
		private bool _useAppResolution;

		/// <summary>
		///     The remaining number of seconds before the connection of the server is dropped.
		/// </summary>
		private double _waitForServerTimeout;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session that should be displayed.</param>
		public GameSessionViewModel(GameSession gameSession)
		{
			Assert.ArgumentNotNull(gameSession);

			_gameSession = gameSession;
			EventMessages = _gameSession.EventMessages.Messages;
			Scoreboard = new ScoreboardViewModel(_gameSession);
			Respawn = new RespawnViewModel(_gameSession);
			Chat = new ChatViewModel();
			View = new GameSessionView();

			// TODO: Selection via UI
			_gameSession.NetworkSession.Send(SelectionMessage.Create(_gameSession.LocalPlayer,
				EntityType.Ship, EntityType.Gun, EntityType.Phaser, EntityType.Phaser, EntityType.Phaser));
		}

		/// <summary>
		///     Gets the view model of the respawn view.
		/// </summary>
		public RespawnViewModel Respawn { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the 3D scene should be rendered using the global app resolution.
		/// </summary>
		public bool UseAppResolution
		{
			get { return _useAppResolution; }
			set { ChangePropertyValue(ref _useAppResolution, value); }
		}

		/// <summary>
		///     Gets the view model for the scoreboard.
		/// </summary>
		public ScoreboardViewModel Scoreboard { get; private set; }

		/// <summary>
		///     Gets the view model for the chat input.
		/// </summary>
		public ChatViewModel Chat { get; private set; }

		/// <summary>
		///     Gets the event messages that should be displayed.
		/// </summary>
		public IEnumerable<EventMessage> EventMessages { get; private set; }

		/// <summary>
		///     Gets the camera manager that toggles between the game camera and the debug camera.
		/// </summary>
		public CameraManager CameraManager
		{
			get { return _gameSession.CameraManager; }
		}

		/// <summary>
		///     Gets the list of players of the game session.
		/// </summary>
		public PlayerList Players
		{
			get { return _gameSession.Players; }
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
		///     Shows the chat input.
		/// </summary>
		public void ShowChatInput()
		{
			Chat.IsVisible = true;
		}

		/// <summary>
		///     Shows the scoreboard.
		/// </summary>
		public void ShowScoreboard()
		{
			Scoreboard.IsVisible = true;
		}

		/// <summary>
		///     Hides the scoreboard.
		/// </summary>
		public void HideScoreboard()
		{
			Scoreboard.IsVisible = false;
		}

		/// <summary>
		///     Shows the in-game menu.
		/// </summary>
		public void ShowInGameMenu()
		{
			Child = new InGameMenuViewModel();
		}

		/// <summary>
		///     Draws the 3D scene to the given render output.
		/// </summary>
		/// <param name="renderOutput">The render output the current scene should be drawn to.</param>
		public void OnDraw(RenderOutput renderOutput)
		{
			_gameSession.RenderContext.Draw(renderOutput);
		}

		/// <summary>
		///     Activates the view model, presenting its content and view on the UI.
		/// </summary>
		protected override void OnActivated()
		{
			Cvars.WindowModeChanged += WindowModeChanged;
			UseAppResolution = Cvars.WindowMode == WindowMode.Fullscreen;
		}

		/// <summary>
		///     Ensures the 3D scene is rendered using the global app resolution if the window is in fullscreen mode.
		/// </summary>
		/// <param name="previousWindowMode">The previous window mode.</param>
		private void WindowModeChanged(WindowMode previousWindowMode)
		{
			UseAppResolution = Cvars.WindowMode == WindowMode.Fullscreen;
		}

		/// <summary>
		///     Deactivates the view model, removing its content and view from the UI.
		/// </summary>
		protected override void OnDeactivated()
		{
			_gameSession.SafeDispose();
			Scoreboard.SafeDispose();
			Respawn.SafeDispose();
			Cvars.WindowModeChanged -= WindowModeChanged;

			Log.Info("The game session has ended.");
		}

		/// <summary>
		///     Updates the view model's state.
		/// </summary>
		protected override void OnUpdate()
		{
			try
			{
				// Let the game session and the child view models update their states
				_gameSession.Update(Child is InGameMenuViewModel);
				Scoreboard.Update();
				Respawn.Update();

				// Check if we're lagging or waiting for the server
				IsLagging = _gameSession.NetworkSession.IsLagging;
				WaitForServerTimeout = _gameSession.NetworkSession.TimeToDrop / 1000;

				// Ensure that the scoreboard is hidden when the chat input or the console are visible
				if (Chat.IsVisible || Application.Current.IsConsoleOpen)
					Scoreboard.IsVisible = false;
			}
			catch (ConnectionDroppedException)
			{
				ShowErrorBox("Connection Lost", "The connection to the server has been lost.", new MainMenuViewModel());
			}
			catch (NetworkException e)
			{
				ShowErrorBox("Connection Error",
					String.Format("The game session has been aborted due to a network error: {0}", e.Message),
					new MainMenuViewModel());
			}
		}
	}
}