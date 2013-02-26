using System;

namespace Lwar.Client.GameScreens
{
	using Gameplay;
	using Network;
	using Pegasus.Framework;
	using Rendering;

	/// <summary>
	///   Displays a game session.
	/// </summary>
	public class PlayingScreen : GameScreen
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
		///   The render context that is used to render the game session.
		/// </summary>
		private readonly RenderContext _renderContext;

		/// <summary>
		///   Manages the game and debug cameras.
		/// </summary>
		private CameraManager _cameraManager;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session that should be played.</param>
		/// <param name="networkSession">The network session that synchronizes the game state between the client and the server.</param>
		/// <param name="renderContext">The render context that should be used to render the game session.</param>
		public PlayingScreen(GameSession gameSession, NetworkSession networkSession, RenderContext renderContext)
		{
			Assert.ArgumentNotNull(gameSession, () => gameSession);
			Assert.ArgumentNotNull(networkSession, () => networkSession);
			Assert.ArgumentNotNull(renderContext, () => renderContext);

			_gameSession = gameSession;
			_networkSession = networkSession;
			_renderContext = renderContext;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_networkSession.SafeDispose();
			_gameSession.SafeDispose();
			_renderContext.SafeDispose();
			_cameraManager.SafeDispose();
		}

		/// <summary>
		///   Initializes the game screen.
		/// </summary>
		public override void Initialize()
		{
			_cameraManager = new CameraManager(Window, GraphicsDevice, InputDevice);
		}

		/// <summary>
		///   Updates the game screen.
		/// </summary>
		/// <param name="topmost">Indicates whether the game screen is the topmost one.</param>
		public override void Update(bool topmost)
		{
			_networkSession.Update(_gameSession, _renderContext);
			_gameSession.Update();
		}

		/// <summary>
		///   Draws the game screen.
		/// </summary>
		public override void Draw()
		{
			_renderContext.Draw(_cameraManager.ActiveCamera);
		}
	}
}