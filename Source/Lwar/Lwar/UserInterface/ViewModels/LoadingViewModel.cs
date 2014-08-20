﻿namespace Lwar.UserInterface.ViewModels
{
	using System;
	using Gameplay;
	using Pegasus.Framework.UserInterface.ViewModels;
	using Pegasus.Platform;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;
	using Pegasus.Platform.Network;
	using Scripting;
	using Views;

	/// <summary>
	///     Loads a game session.
	/// </summary>
	public class LoadingViewModel : StackedViewModel
	{
		/// <summary>
		///     The clock that is used for time measurements.
		/// </summary>
		private Clock _clock = new Clock();

		/// <summary>
		///     Indicates how long the client has been trying to establish a connection to the server.
		/// </summary>
		private double _elapsedTime;

		/// <summary>
		///     The game session that is being loaded.
		/// </summary>
		private GameSession _gameSession;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="serverEndPoint">The remote end point of the server.</param>
		public LoadingViewModel(IPEndPoint serverEndPoint)
		{
			Commands.ShowConsole(false);
			View = new LoadingView();

			_gameSession = new GameSession(serverEndPoint);
		}

		/// <summary>
		///     Gets the endpoint of the server that hosts the game session.
		/// </summary>
		public IPEndPoint ServerEndPoint
		{
			get { return _gameSession.NetworkSession.ServerEndPoint; }
		}

		/// <summary>
		///     Gets a value indicating how long the client has been trying to establish a connection to the server.
		/// </summary>
		public double ElapsedTime
		{
			get { return _elapsedTime; }
			private set { ChangePropertyValue(ref _elapsedTime, Math.Round(value)); }
		}

		/// <summary>
		///     Updates the view model's state.
		/// </summary>
		protected override void OnUpdate()
		{
			try
			{
				ElapsedTime = _clock.Seconds;

				if (!_gameSession.Load())
					return;

				Log.Info("Loading completed and game state synced. Now connected to game session hosted by {0}.", ServerEndPoint);

				var gameSession = _gameSession;
				_gameSession = null;

				Root.ReplaceChild(new GameSessionViewModel(gameSession));
			}
			catch (ConnectionDroppedException)
			{
				ShowErrorBox("Connection Failed",
					String.Format("Unable to connect to {0}. The connection attempt timed out.", ServerEndPoint),
					new MainMenuViewModel());
			}
			catch (ServerFullException)
			{
				ShowErrorBox("Connection Rejected", "The server is full.", new MainMenuViewModel());
			}
			catch (ProtocolMismatchException)
			{
				ShowErrorBox("Connection Rejected", "The server uses an incompatible version of the network protocol.", new MainMenuViewModel());
			}
			catch (NetworkException e)
			{
				ShowErrorBox("Connection Error",
					String.Format("The connection attempt has been aborted due to a network error: {0}", e.Message),
					new MainMenuViewModel());
			}
		}

		/// <summary>
		///     Deactivates the view model, removing its content and view from the UI.
		/// </summary>
		protected override void OnDeactivated()
		{
			_gameSession.SafeDispose();
		}

		/// <summary>
		///     Stops loading the game session and returns to the main menu.
		/// </summary>
		public void Abort()
		{
			Commands.Disconnect();
		}
	}
}