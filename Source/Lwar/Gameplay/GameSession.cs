namespace Lwar.Gameplay
{
	using System;
	using Actors;
	using Entities;
	using Network;
	using Network.Messages;
	using Pegasus;
	using Pegasus.Assets;
	using Pegasus.Framework;
	using Pegasus.Platform;
	using Pegasus.Platform.Memory;
	using Pegasus.Platform.Network;
	using Rendering;
	using Scripting;

	/// <summary>
	///     Represents a game session, managing the state of entities, players, etc.
	/// </summary>
	public class GameSession : DisposableObject
	{
		/// <summary>
		///     The assets manager that is used to load the assets required by the game session.
		/// </summary>
		private readonly AssetsManager _assets;

		/// <summary>
		///     The clock that is used for time measurements.
		/// </summary>
		private Clock _clock = new Clock();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="serverEndPoint">The remote end point of the server.</param>
		public GameSession(IPEndPoint serverEndPoint)
		{
			_assets = new AssetsManager(Application.Current.GraphicsDevice, asyncLoading: true);
			EntityTemplates.Initialize(Application.Current.GraphicsDevice, _assets);

			RenderContext = new RenderContext(_assets);
			NetworkSession = new NetworkSession(serverEndPoint, new MessageDispatcher(this));

			Actors = new ActorList(this, RenderContext);
			Entities = new EntityList(this, RenderContext);
			Players = new PlayerList();
			RootTransform = new Transformation();
			EventMessages = new EventMessageList(this);
		}

		/// <summary>
		///     Gets the render context that is used to draw the game session.
		/// </summary>
		public RenderContext RenderContext { get; private set; }

		/// <summary>
		///     The network session that handles the communiation with the server hosting the game session.
		/// </summary>
		public NetworkSession NetworkSession { get; private set; }

		/// <summary>
		///     The entities that are currently active.
		/// </summary>
		public EntityList Entities { get; private set; }

		/// <summary>
		///     The actors that are currently active, not including any entities.
		/// </summary>
		public ActorList Actors { get; private set; }

		/// <summary>
		///     The players that are currently playing.
		/// </summary>
		public PlayerList Players { get; private set; }

		/// <summary>
		///     Gets the root transformation.
		/// </summary>
		public Transformation RootTransform { get; private set; }

		/// <summary>
		///     Gets the event messages that display game session events to the user such as player kills or received chat messages.
		/// </summary>
		public EventMessageList EventMessages { get; private set; }

		/// <summary>
		///     Gets the camera manager for the game session.
		/// </summary>
		public CameraManager CameraManager { get; private set; }

		/// <summary>
		///     Gets the game camera that is used to draw the game session.
		/// </summary>
		public GameCamera Camera
		{
			get
			{
				Assert.NotNull(CameraManager, "The camera manager has not been initialized.");
				return CameraManager.GameCamera;
			}
		}

		/// <summary>
		///     Gets a value indicating whether the game camera is active.
		/// </summary>
		public bool IsGameCameraActive
		{
			get
			{
				Assert.NotNull(CameraManager, "The camera manager has not been initialized.");
				return CameraManager.ActiveCamera == CameraManager.GameCamera;
			}
		}

		/// <summary>
		///     Gets the local player.
		/// </summary>
		public Player LocalPlayer
		{
			get
			{
				Assert.NotNull(Players.LocalPlayer, "The local player is unknown.");
				return Players.LocalPlayer;
			}
		}

		/// <summary>
		///     The input manager that handles the user input for the game session.
		/// </summary>
		public InputManager InputManager { get; private set; }

		/// <summary>
		///     Loads the game session and syncs the game state with the server. Returns true to indicate that loading has completed.
		/// </summary>
		public bool Load()
		{
			NetworkSession.Update();
			return _assets.LoadPending(timeoutInMilliseconds: 5) && NetworkSession.IsSynced;
		}

		/// <summary>
		///     Initializes the game session once it has been fully loaded and the game state has been synced.
		/// </summary>
		public void Initialize()
		{
			Assert.NotNull(LocalPlayer, "Game state synced but local player is unknown.");
			Assert.That(_assets.LoadingCompleted, "Not all assets have been loaded.");

			// Handle chat input and player name changes
			Commands.OnSay += OnSay;
			Cvars.PlayerNameChanged += OnPlayerNameChanged;

			// Resend player name, as it might have been changed during the connection attempt
			OnPlayerNameChanged(Cvars.PlayerName);

			// Initialize the camera and input managers
			CameraManager = new CameraManager(LocalPlayer);
			InputManager = new InputManager(this);

			// Initialize the render context
			RenderContext.Initialize();
			Assert.That(_assets.LoadingCompleted, "The render context loaded further assets.");
		}

		/// <summary>
		///     Updates the state of the game session.
		/// </summary>
		public void Update()
		{
			NetworkSession.Update();
			CameraManager.Update();
			InputManager.Update();
			InputManager.SendInput();

			Players.Update();
			Entities.Update(_clock);
			Actors.Update(_clock);
			RootTransform.Update();
			EventMessages.Update();

			_clock.Reset();
		}

		/// <summary>
		///     Invoked when the local player changed his or her name.
		/// </summary>
		/// <param name="name">The previous name of the local player.</param>
		private void OnPlayerNameChanged(string name)
		{
			NetworkSession.Send(NameMessage.Create(LocalPlayer, Cvars.PlayerName));
		}

		/// <summary>
		///     Invoked when the local player entered a chat message.
		/// </summary>
		/// <param name="message">The message that the local player wants to send.</param>
		private void OnSay(string message)
		{
			NetworkSession.Send(ChatMessage.Create(LocalPlayer, message));
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Commands.OnSay -= OnSay;
			Cvars.PlayerNameChanged -= OnPlayerNameChanged;

			EntityTemplates.Dispose();
			Actors.SafeDispose();
			Entities.SafeDispose();
			Players.SafeDispose();
			EventMessages.SafeDispose();
			CameraManager.SafeDispose();
			InputManager.SafeDispose();
			NetworkSession.SafeDispose();
			RenderContext.SafeDispose();

			_assets.SafeDispose();
		}
	}
}