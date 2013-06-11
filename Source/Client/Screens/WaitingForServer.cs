using System;

namespace Lwar.Client.Screens
{
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform.Memory;
	using Pegasus.Framework.Rendering;
	using Pegasus.Framework.Rendering.UserInterface;

	/// <summary>
	///   Informs the user that the network session is waiting for new messages from the server.
	/// </summary>
	public class WaitingForServer : Screen
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
			_timeoutLabel.SafeDispose();
		}

		/// <summary>
		///   Initializes the screen.
		/// </summary>
		public override void Initialize()
		{
			_timeoutLabel = new Label(Assets.LoadFont("Fonts/Liberation Mono 12"))
			{
				Alignment = TextAlignment.Middle | TextAlignment.Centered
			};
		}

		/// <summary>
		///   Updates the screen.
		/// </summary>
		/// <param name="topmost">Indicates whether the app screen is the topmost one.</param>
		public override void Update(bool topmost)
		{
			_timeoutLabel.Area = new Rectangle(0, 0, Window.Width, Window.Height);
			_timeoutLabel.Text = String.Format("Waiting for server ({0} seconds)...", (int)(_networkSession.TimeToDrop / 1000) + 1);

			if (!_networkSession.IsLagging || _networkSession.IsDropped)
				ScreenManager.Remove(this);
		}

		/// <summary>
		///   Draws the user interface elements of the app screen.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the user interface.</param>
		public override void DrawUserInterface(SpriteBatch spriteBatch)
		{
			_timeoutLabel.Draw(spriteBatch);
		}
	}
}