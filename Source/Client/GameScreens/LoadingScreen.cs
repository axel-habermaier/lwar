using System;

namespace Lwar.Client.GameScreens
{
	using System.Net;
	using Gameplay;
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Rendering.UserInterface;
	using Pegasus.Framework.Scripting;
	using Rendering;

	/// <summary>
	///   Loads a game session.
	/// </summary>
	public class LoadingScreen : GameScreen
	{
		/// <summary>
		///   The game session that is loaded.
		/// </summary>
		private GameSession _gameSession;

		/// <summary>
		///   The network session that is used to synchronize the game state between the client and the server.
		/// </summary>
		private NetworkSession _networkSession;

		/// <summary>
		///   The render context that is loaded.
		/// </summary>
		private RenderContext _renderContext;

		/// <summary>
		///   Shows a status message to the user.
		/// </summary>
		private Label _statusMessage;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="serverEndPoint">The remote end point of the server.</param>
		public LoadingScreen(IPEndPoint serverEndPoint)
		{
			Assert.ArgumentNotNull(serverEndPoint, () => serverEndPoint);

			_networkSession = new NetworkSession(serverEndPoint);
			_gameSession = new GameSession();
		}

		/// <summary>
		///   Initializes the game screen.
		/// </summary>
		public override void Initialize()
		{
			_renderContext = new RenderContext(Window, GraphicsDevice, Assets);
			_statusMessage = new Label(Assets.LoadFont("Fonts/Liberation Mono 12"))
			{
				Alignment = TextAlignment.Centered | TextAlignment.Middle,
				Text = String.Format("Connecting to {0}...", _networkSession.ServerEndPoint)
			};

			Commands.ShowConsole.Invoke(false);
		}

		/// <summary>
		///   Updates the game screen.
		/// </summary>
		/// <param name="topmost">Indicates whether the game screen is the topmost one.</param>
		public override void Update(bool topmost)
		{
			_networkSession.Update(_gameSession, _renderContext);

			if (_networkSession.IsSyncing)
				_statusMessage.Text = "Awaiting game state...";

			if (_networkSession.IsConnected)
			{
				ScreenManager.Add(new PlayingScreen(_gameSession, _networkSession, _renderContext));

				_networkSession = null;
				_gameSession = null;
				_renderContext = null;

				ScreenManager.Remove(this);
			}
		}

		/// <summary>
		///   Draws the game screen.
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
			_networkSession.SafeDispose();
			_gameSession.SafeDispose();
			_renderContext.SafeDispose();
		}
	}
}