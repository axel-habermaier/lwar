using System;

namespace Lwar.Client.GameStates
{
	using Gameplay;
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Rendering.UserInterface;
	using Pegasus.Framework.Scripting;

	/// <summary>
	///   Loads a game session.
	/// </summary>
	public class Loading : GameState
	{
		/// <summary>
		///   The game session that is loaded.
		/// </summary>
		private readonly GameSession _gameSession;

		/// <summary>
		///   The network session that is used to synchronize the game state between the client and the server.
		/// </summary>
		private readonly NetworkSession _networkSession;

		/// <summary>
		///   Shows a status message to the user.
		/// </summary>
		private Label _statusMessage;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session that should be loaded.</param>
		/// <param name="networkSession">The network session that synchronizes the game state between the client and the server.</param>
		public Loading(GameSession gameSession, NetworkSession networkSession)
		{
			Assert.ArgumentNotNull(gameSession, () => gameSession);
			Assert.ArgumentNotNull(networkSession, () => networkSession);

			_gameSession = gameSession;
			_networkSession = networkSession;
		}

		/// <summary>
		///   Initializes the game state.
		/// </summary>
		public override void Initialize()
		{
			_statusMessage = new Label(Assets.LoadFont("Fonts/Liberation Mono 12"))
			{
				Alignment = TextAlignment.Centered | TextAlignment.Middle,
				Text = String.Format("Connecting to {0}...", _networkSession.ServerEndPoint)
			};

			Commands.ShowConsole.Invoke(false);
		}

		/// <summary>
		///   Updates the game state.
		/// </summary>
		/// <param name="topmost">Indicates whether the game state is the topmost one.</param>
		public override void Update(bool topmost)
		{
			if (_networkSession.IsSyncing)
				_statusMessage.Text = "Awaiting game state...";

			if (_networkSession.IsConnected)
			{
				Assert.NotNull(_gameSession.LocalPlayer, "Game state synced but local player is unknown.");

				_networkSession.Send(Message.ChangePlayerName(_gameSession.LocalPlayer, Cvars.PlayerName.Value));
				_networkSession.Send(Message.ChangeSelection(_gameSession.LocalPlayer, EntityType.Ship,
															 EntityType.Gun, EntityType.Gun,
															 EntityType.Gun, EntityType.Gun));
			}

			if (_networkSession.IsConnected || _networkSession.IsDropped || _networkSession.IsFaulted)
				StateManager.Remove(this);

			if (_networkSession.ServerIsFull)
				ShowMessageBox("The server is full.", LogType.Error, true);

			_statusMessage.Area = new Rectangle(0, 0, Window.Width, Window.Height);
		}

		/// <summary>
		///   Draws the game state.
		/// </summary>
		public override void Draw()
		{
			_statusMessage.Draw(SpriteBatch);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
		}
	}
}