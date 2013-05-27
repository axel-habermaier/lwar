using System;

namespace Lwar.Client.GameStates
{
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Rendering;
	using Pegasus.Framework.Rendering.UserInterface;

	/// <summary>
	///   Informs the user that the network session is waiting for new messages from the server.
	/// </summary>
	public class WaitingForServer : GameState
	{
		/// <summary>
		///   The network session that is waiting for new messages from the server.
		/// </summary>
		private readonly NetworkSession _networkSession;

		/// <summary>
		///   The label that informs the user about the remaining time before the connection is dropped.
		/// </summary>
		private Label _timeoutLabel;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="networkSession">The network session that is waiting for new messages from the server.</param>
		public WaitingForServer(NetworkSession networkSession)
		{
			Assert.ArgumentNotNull(networkSession);

			IsOpaque = false;
			_networkSession = networkSession;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
		}

		/// <summary>
		///   Initializes the game state.
		/// </summary>
		public override void Initialize()
		{
			_timeoutLabel = new Label(Assets.LoadFont("Fonts/Liberation Mono 12"))
			{
				Alignment = TextAlignment.Middle | TextAlignment.Centered
			};
		}

		/// <summary>
		///   Updates the game state.
		/// </summary>
		/// <param name="topmost">Indicates whether the game screen is the topmost one.</param>
		public override void Update(bool topmost)
		{
			if (!_networkSession.IsLagging || _networkSession.IsDropped)
				StateManager.Remove(this);

			_timeoutLabel.Area = new Rectangle(0, 0, Window.Width, Window.Height);
			_timeoutLabel.Text = String.Format("Waiting for server ({0} seconds)...", (int)(_networkSession.TimeToDrop / 1000) + 1);
		}

		/// <summary>
		///   Draws the user interface elements of the game state.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the user interface.</param>
		public override void DrawUserInterface(SpriteBatch spriteBatch)
		{
			_timeoutLabel.Draw(spriteBatch);
		}
	}
}