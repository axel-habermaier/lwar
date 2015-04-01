namespace Lwar.UserInterface.ViewModels
{
	using System;
	using System.Collections.Generic;
	using Gameplay.Client;
	using Network;
	using Network.Messages;
	using Pegasus;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;
	using Pegasus.Platform.Network;
	using Pegasus.Rendering;
	using Pegasus.UserInterface.Controls;
	using Pegasus.UserInterface.ViewModels;
	using Pegasus.Utilities;
	using Rendering;
	using Scripting;
	using Views;

	/// <summary>
	///     Displays a game session.
	/// </summary>
	internal class GameSessionViewModel : StackedViewModel
	{
		/// <summary>
		///     The game session that is played.
		/// </summary>
		private ClientGameSession _gameSession;

		/// <summary>
		///     Indicates whether the connection to the server is lagging.
		/// </summary>
		private bool _isLagging;

		/// <summary>
		///     Indicates whether the 3D scene should be rendered using the global app resolution.
		/// </summary>
		private ResolutionSource _resolutionSource;

		/// <summary>
		///     The remaining number of seconds before the connection of the server is dropped.
		/// </summary>
		private double _waitForServerTimeout;

		private int _weaponEnergy1;
		private int _weaponEnergy2;
		private int _weaponEnergy3;
		private int _weaponEnergy4;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session that should be displayed.</param>
		public GameSessionViewModel(ClientGameSession gameSession)
		{
			Assert.ArgumentNotNull(gameSession);

			_gameSession = gameSession;
			EventMessages = _gameSession.EventMessages.Messages;
			Scoreboard = new ScoreboardViewModel(_gameSession);
			Respawn = new RespawnViewModel(_gameSession);
			Chat = new ChatViewModel();
			View = new GameSessionView();

			_gameSession.InputDevice.UIElement = View;
			_gameSession.Connection.Send(PlayerLoadoutMessage.Create(_gameSession.Allocator, _gameSession.LocalPlayer.Identity,
				EntityType.Ship, new[] { EntityType.Gun, EntityType.Phaser, EntityType.RocketLauncher, EntityType.RocketLauncher }));
		}

		/// <summary>
		///     Gets the view model of the respawn view.
		/// </summary>
		public RespawnViewModel Respawn { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the 3D scene should be rendered using the global app resolution.
		/// </summary>
		public ResolutionSource ResolutionSource
		{
			get { return _resolutionSource; }
			set { ChangePropertyValue(ref _resolutionSource, value); }
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

		public int WeaponEnergy1
		{
			get { return _weaponEnergy1; }
			set { ChangePropertyValue(ref _weaponEnergy1, value); }
		}

		public int WeaponEnergy2
		{
			get { return _weaponEnergy2; }
			set { ChangePropertyValue(ref _weaponEnergy2, value); }
		}

		public int WeaponEnergy3
		{
			get { return _weaponEnergy3; }
			set { ChangePropertyValue(ref _weaponEnergy3, value); }
		}

		public int WeaponEnergy4
		{
			get { return _weaponEnergy4; }
			set { ChangePropertyValue(ref _weaponEnergy4, value); }
		}

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
			_gameSession.Renderer.Draw(renderOutput);
		}

		/// <summary>
		///     Toggles between the game and the debug camera.
		/// </summary>
		public void ToggleDebugCamera()
		{
			CameraManager.ToggleDebugCamera();
		}

		/// <summary>
		///     Activates the view model, presenting its content and view on the UI.
		/// </summary>
		protected override void OnActivated()
		{
			Cvars.WindowModeChanged += WindowModeChanged;
			ResolutionSource = Cvars.WindowMode == WindowMode.Fullscreen ? ResolutionSource.Application : ResolutionSource.Layout;
		}

		/// <summary>
		///     Ensures the 3D scene is rendered using the global app resolution if the window is in fullscreen mode.
		/// </summary>
		/// <param name="previousWindowMode">The previous window mode.</param>
		private void WindowModeChanged(WindowMode previousWindowMode)
		{
			ResolutionSource = Cvars.WindowMode == WindowMode.Fullscreen ? ResolutionSource.Application : ResolutionSource.Layout;
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
				IsLagging = _gameSession.Connection.IsLagging;
				WaitForServerTimeout = _gameSession.Connection.TimeToDrop / 1000;

				// Ensure that the scoreboard is hidden when the chat input or the console are visible
				if (Chat.IsVisible || Application.Current.IsConsoleOpen)
					Scoreboard.IsVisible = false;

				if (_gameSession.LocalPlayer != null && _gameSession.LocalPlayer.Ship != null)
				{
					var ship = _gameSession.LocalPlayer.Ship;
					WeaponEnergy1 = ship.Energy1;
					WeaponEnergy2 = ship.Energy2;
					WeaponEnergy3 = ship.Energy3;
					WeaponEnergy4 = ship.Energy4;
				}
			}
			catch (ConnectionDroppedException)
			{
				ShowErrorBox("Connection Lost", "The connection to the server has been lost.");
				Commands.Disconnect();
			}
			catch (ServerQuitException)
			{
				ShowErrorBox("Server Shutdown", "The server has ended the game session.");
				Commands.Disconnect();
			}
			catch (NetworkException e)
			{
				ShowErrorBox("Connection Error", String.Format("The game session has been aborted due to a network error: {0}", e.Message));
				Commands.Disconnect();
			}
		}
	}
}