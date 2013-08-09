using System;

namespace Lwar.Screens
{
	using Gameplay;
	using Gameplay.Entities;
	using Network;
	using Network.Messages;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform.Logging;
	using Pegasus.Framework.Platform.Memory;
	using Pegasus.Framework.Rendering;
	using Pegasus.Framework.Rendering.UserInterface;
	using Scripting;

	/// <summary>
	///   Loads a game session.
	/// </summary>
	public class Loading : Screen
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
			Assert.ArgumentNotNull(gameSession);
			Assert.ArgumentNotNull(networkSession);

			_gameSession = gameSession;
			_networkSession = networkSession;
		}

		/// <summary>
		///   Initializes the screen.
		/// </summary>
		public override void Initialize()
		{
			_statusMessage = new Label(Assets.LoadFont("Fonts/Liberation Mono 11"))
			{
				Alignment = TextAlignment.Centered | TextAlignment.Middle,
				Text = String.Format("Connecting to {0}...", _networkSession.ServerEndPoint)
			};

			Commands.ShowConsole(false);
		}

		/// <summary>
		///   Updates the screen.
		/// </summary>
		/// <param name="topmost">Indicates whether the app screen is the topmost one.</param>
		public override void Update(bool topmost)
		{
			if (_networkSession.IsSyncing)
				_statusMessage.Text = "Awaiting game state...";

			if (_networkSession.IsConnected)
			{
				var localPlayer = _gameSession.Players.LocalPlayer;
				Assert.NotNull(localPlayer, "Game state synced but local player is unknown.");

				_networkSession.Send(SelectionMessage.Create(localPlayer, EntityType.Ship,
															 EntityType.Gun, EntityType.Phaser,
															 EntityType.Rocket, EntityType.Phaser));

				_gameSession.EventMessages.Enabled = true;
			}

			_statusMessage.Area = new Rectangle(0, 0, Window.Width, Window.Height);

			if (_networkSession.IsConnected || _networkSession.IsDropped || _networkSession.IsFaulted)
				ScreenManager.Remove(this);

			if (_networkSession.ServerIsFull)
				MessageBox.Show(this, LogType.Error, "The server is full.", true);

			if (_networkSession.VersionMismatch)
				MessageBox.Show(this, LogType.Error, "The server uses an incompatible version of the network protocol.", true);
		}

		/// <summary>
		///   Draws the user interface elements of the app screen.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the user interface.</param>
		public override void DrawUserInterface(SpriteBatch spriteBatch)
		{
			_statusMessage.Draw(spriteBatch);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_statusMessage.SafeDispose();
		}
	}
}