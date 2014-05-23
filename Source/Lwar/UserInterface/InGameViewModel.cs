namespace Lwar.UserInterface
{
	using System;
	using Network;
	using Pegasus.Framework;
	using Pegasus.Platform;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;
	using Pegasus.Platform.Network;
	using Pegasus.Rendering;
	using Rendering;

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
		private readonly Timer _timer = new Timer(1000.0 / Specification.InputUpdateFrequency);

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="serverEndPoint">The remote end point of the server.</param>
		public InGameViewModel(IPEndPoint serverEndPoint)
		{
			OnDraw = Draw;

			View = new InGameView();
			_networkSession = new NetworkSession(serverEndPoint);
			_timer.Timeout += SendInputTimeout;
			CameraManager = new CameraManager(Application.Current.Window, Application.Current.GraphicsDevice, Application.Current.Window.InputDevice);

			Log.Info("Connecting to {0}...", serverEndPoint);
		}

		/// <summary>
		///     Gets the camera manager that toggles between the game camera and the debug camera.
		/// </summary>
		public CameraManager CameraManager { get; private set; }

		/// <summary>
		///     Gets the callback that should be used to redraw the 3D scene.
		/// </summary>
		public Action<RenderOutput> OnDraw { get; private set; }

		/// <summary>
		///     Ensures that the user input is sent to the server during the next input cycle.
		/// </summary>
		private void SendInputTimeout()
		{
			//_sendInput = true;
		}

		/// <summary>
		///     Draws the 3D scene to the given render output.
		/// </summary>
		/// <param name="renderOutput">The renderoutput the current scene should be drawn to.</param>
		private void Draw(RenderOutput renderOutput)
		{
			renderOutput.ClearColor(new Color(255, 255, 0, 255));
		}

		/// <summary>
		///     Deactivates the view model, removing its content and view from the UI.
		/// </summary>
		protected override void OnDeactivated()
		{
			CameraManager.SafeDispose();
			_networkSession.SafeDispose();
		}
	}
}