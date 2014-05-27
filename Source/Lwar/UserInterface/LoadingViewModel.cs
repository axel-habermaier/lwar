namespace Lwar.UserInterface
{
	using System;
	using Gameplay;
	using Gameplay.Entities;
	using Network;
	using Network.Messages;
	using Pegasus;
	using Pegasus.Platform.Memory;
	using Scripting;

	/// <summary>
	///     Loads a game session.
	/// </summary>
	internal class LoadingViewModel : LwarViewModel<LoadingView>
	{
		/// <summary>
		///     The game session that is loaded.
		/// </summary>
		private readonly GameSession _gameSession;

		/// <summary>
		///     The network session that is used to synchronize the game state between the client and the server.
		/// </summary>
		private readonly NetworkSession _networkSession;

		/// <summary>
		/// The status message that is displayed on the loading screen.
		/// </summary>
		private string _statusMessage;

		/// <summary>
		/// Gets or sets the status message that is displayed on the loading screen.
		/// </summary>
		public string StatusMessage
		{
			get { return _statusMessage; }
			set { ChangePropertyValue(ref _statusMessage, value); }
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session that should be loaded.</param>
		/// <param name="networkSession">The network session that synchronizes the game state between the client and the server.</param>
		public LoadingViewModel(GameSession gameSession, NetworkSession networkSession)
		{
			Assert.ArgumentNotNull(gameSession);
			Assert.ArgumentNotNull(networkSession);

			_gameSession = gameSession;
			_networkSession = networkSession;

			View = new LoadingView();
		}

		/// <summary>
		///     Activates the view model, presenting its content and view on the UI.
		/// </summary>
		protected override void OnActivated()
		{
			StatusMessage = String.Format("Connecting to {0}...", _networkSession.ServerEndPoint);
			Commands.ShowConsole(false);
		}

		/// <summary>
		///     Updates the view model's state.
		/// </summary>
		protected override void OnUpdate()
		{
			if (_networkSession.IsSyncing)
				StatusMessage = "Awaiting game state...";

			if (_networkSession.IsConnected)
			{
				var localPlayer = _gameSession.Players.LocalPlayer;
				Assert.NotNull(localPlayer, "Game state synced but local player is unknown.");

				_networkSession.Send(SelectionMessage.Create(localPlayer, EntityType.Ship,
															 EntityType.Gun, EntityType.Phaser,
															 EntityType.Phaser, EntityType.Phaser));

				_gameSession.EventMessages.Enabled = true;
			}

			if (_networkSession.IsConnected || _networkSession.IsDropped || _networkSession.IsFaulted)
			{
				Parent.Child = null;
				this.SafeDispose();
			}

			//if (_networkSession.ServerIsFull)
				//MessageBox.Show(this, LogType.Error, "The server is full.", true);

			//if (_networkSession.VersionMismatch)
				//MessageBox.Show(this, LogType.Error, "The server uses an incompatible version of the network protocol.", true);
		}
	}
}