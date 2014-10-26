namespace Lwar.UserInterface.ViewModels
{
	using System;
	using Gameplay.Client;
	using Pegasus.Platform;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;
	using Pegasus.Platform.Network;
	using Pegasus.UserInterface.ViewModels;
	using Pegasus.Utilities;
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
		private ClientGameSession _gameSession;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="serverEndPoint">The remote end point of the server.</param>
		public LoadingViewModel(IPEndPoint serverEndPoint)
		{
			Commands.ShowConsole(false);
			View = new LoadingView();

			_gameSession = new ClientGameSession(serverEndPoint);
		}

		/// <summary>
		///     Gets the endpoint of the server that hosts the game session.
		/// </summary>
		public IPEndPoint ServerEndPoint
		{
			get { return _gameSession.ServerEndPoint; }
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
				ShowErrorBox("Connection Failed", String.Format("Unable to connect to {0}. The connection attempt timed out.", ServerEndPoint));
				Commands.Disconnect();
			}
			catch (ServerFullException)
			{
				ShowErrorBox("Connection Rejected", "The server is full.");
				Commands.Disconnect();
			}
			catch (ProtocolMismatchException)
			{
				ShowErrorBox("Connection Rejected", "The server uses an incompatible version of the network protocol.");
				Commands.Disconnect();
			}
			catch (ServerQuitException)
			{
				ShowErrorBox("Server Shutdown", "The server has ended the game session.");
				Commands.Disconnect();
			}
			catch (NetworkException e)
			{
				ShowErrorBox("Connection Error", String.Format("The connection attempt has been aborted due to a network error: {0}", e.Message));
				Commands.Disconnect();
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