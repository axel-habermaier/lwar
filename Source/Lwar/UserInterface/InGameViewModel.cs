namespace Lwar.UserInterface
{
	using System;
	using Network;
	using Pegasus.Framework;
	using Pegasus.Math;
	using Pegasus.Platform;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;
	using Pegasus.Platform.Network;
	using Pegasus.Rendering;

	/// <summary>
	///     Displays a game session.
	/// </summary>
	public class InGameViewModel : LwarViewModel<InGameView>
	{
		private readonly Camera2D _camera2D;

		/// <summary>
		///     The network session that synchronizes the game state between the client and the server.
		/// </summary>
		private readonly NetworkSession _networkSession;

		/// <summary>
		///     The timer that is used to send user input to the server.
		/// </summary>
		private readonly Timer _timer = new Timer(1000.0 / Specification.InputUpdateFrequency);

		private Texture2D _outputTexture;
		private RenderOutput _renderOutput;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="serverEndPoint">The remote end point of the server.</param>
		public InGameViewModel(IPEndPoint serverEndPoint)
		{
			OnDraw = Draw;
			OnSizeChanged = InitializeRenderOutput;

			View = new InGameView();
			_networkSession = new NetworkSession(serverEndPoint);
			_timer.Timeout += SendInputTimeout;

			Log.Info("Connecting to {0}...", serverEndPoint);
			_camera2D = new Camera2D(Application.Current.GraphicsDevice);
		}

		public Action OnDraw { get; private set; }

		public Action<Size> OnSizeChanged { get; private set; }

		public Texture2D OutputTexture
		{
			get { return _outputTexture; }
			set { ChangePropertyValue(ref _outputTexture, value); }
		}

		/// <summary>
		///     Ensures that the user input is sent to the server during the next input cycle.
		/// </summary>
		private void SendInputTimeout()
		{
			//_sendInput = true;
		}

		private void InitializeRenderOutput(Size size)
		{
			DisposeRenderOutput();

			if (size.Width == 0 || size.Height == 0)
				return;

			OutputTexture = new Texture2D(Application.Current.GraphicsDevice, size, SurfaceFormat.Rgba8, TextureFlags.RenderTarget);
			OutputTexture.SetName("RenderOutputPanel.ColorBuffer");

			_camera2D.Viewport = new Rectangle(0, 0, size);

			_renderOutput = new RenderOutput(Application.Current.GraphicsDevice)
			{
				RenderTarget = new RenderTarget(Application.Current.GraphicsDevice, null, OutputTexture),
				Viewport = _camera2D.Viewport,
				Camera = _camera2D
			};
		}

		private void DisposeRenderOutput()
		{
			if (_renderOutput != null)
				_renderOutput.RenderTarget.SafeDispose();

			OutputTexture.SafeDispose();
			_renderOutput.SafeDispose();

			OutputTexture = null;
		}

		private void Draw()
		{
			_renderOutput.ClearColor(new Color(255, 255, 0, 255));
		}
	}
}