namespace Lwar.UserInterface
{
	using System;
	using Network;
	using Pegasus.Platform;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Network;

	/// <summary>
	///     Displays a game session.
	/// </summary>
	public class InGameViewModel : LwarViewModel<InGameView>
	{
		/// <summary>
		///     The network session that synchronizes the game state between the client and the server.
		/// </summary>
		private readonly NetworkSession _networkSession;

		/// <summary>
		///     The timer that is used to send user input to the server.
		/// </summary>
		private Timer _timer = new Timer(1000.0 / Specification.InputUpdateFrequency);

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="serverEndPoint">The remote end point of the server.</param>
		public InGameViewModel(IPEndPoint serverEndPoint)
		{
			View = new InGameView();
			_networkSession = new NetworkSession(serverEndPoint);
			_timer.Timeout += SendInputTimeout;

			Log.Info("Connecting to {0}...", serverEndPoint);
		}

		/// <summary>
		///     Ensures that the user input is sent to the server during the next input cycle.
		/// </summary>
		private void SendInputTimeout()
		{
			//_sendInput = true;
		}
	}
}