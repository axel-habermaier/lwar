using System;

namespace Lwar.Client.Screens
{
	using System;
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Shows a list of events (e.g., X killed Y) and received chat messages on the screen.
	/// </summary>
	public class EventLog
	{
		/// <summary>
		///   The game session that is running.
		/// </summary>
		private readonly GameSession _gameSession;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="assets">The assets manager that should be used to load required assets.</param>
		/// <param name="gameSession">The game session that is running.</param>
		public EventLog(AssetsManager assets, GameSession gameSession)
		{
			Assert.ArgumentNotNull(assets);
			Assert.ArgumentNotNull(gameSession);

			_gameSession = gameSession;
		}

		/// <summary>
		///   Draws the event log.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used for drawing.</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			Assert.ArgumentNotNull(spriteBatch);
		}
	}
}