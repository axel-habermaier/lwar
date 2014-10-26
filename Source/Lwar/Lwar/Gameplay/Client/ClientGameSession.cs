namespace Lwar.Gameplay.Client
{
	using System;
	using Actors;
	using Entities;
	using Network;
	using Network.Messages;
	using Pegasus;
	using Pegasus.Assets;
	using Pegasus.Platform;
	using Pegasus.Platform.Memory;
	using Pegasus.Platform.Network;
	using Pegasus.UserInterface.Input;
	using Pegasus.Utilities;
	using Rendering;
	using Scripting;

	/// <summary>
	///     Represents a client-side game session, managing the states of entities, players, etc.
	/// </summary>
	public class ClientGameSession : DisposableObject
	{
		/// <summary>
		///     The assets manager that is used to load the assets required by the game session.
		/// </summary>
		private readonly AssetsManager _assets;

		/// <summary>
		///     The dispatcher that is used to dispatch incoming messages from the server.
		/// </summary>
		private readonly MessageHandler _messageHandler;

		/// <summary>
		///     The clock that is used for time measurements.
		/// </summary>
		private Clock _clock = new Clock();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="serverEndPoint">The remote end point of the server.</param>
		public ClientGameSession(IPEndPoint serverEndPoint)
		{
			_messageHandler = new MessageHandler(this);
			_assets = new AssetsManager(Application.Current.GraphicsDevice, asyncLoading: true);

			EntityTemplates.Initialize(Application.Current.GraphicsDevice, _assets);

			var channel = UdpChannel.Create(serverEndPoint, NetworkProtocol.MaxPacketSize);
			Allocator = new PoolAllocator();
			Connection = Connection.Create(Allocator, channel);
			RenderContext = new RenderContext(_assets);

			Actors = new ActorList(this, RenderContext);
			Entities = new EntityList(this, RenderContext);
			Players = new PlayerList(this);
			RootTransform = new Transformation();
			EventMessages = new EventMessageList(this);

			Connection.Send(ClientConnectMessage.Create(Allocator, Cvars.PlayerName));
			Connection.SendQueuedMessages();
		}

		/// <summary>
		///     Gets the connection to the remote server that hosts the game session.
		/// </summary>
		public Connection Connection { get; private set; }

		/// <summary>
		///     Gets the endpoint of the server.
		/// </summary>
		public IPEndPoint ServerEndPoint
		{
			get { return Connection.RemoteEndPoint; }
		}

		/// <summary>
		///     Gets the object pool that is used to allocate gameplay objects.
		/// </summary>
		public PoolAllocator Allocator { get; private set; }

		/// <summary>
		///     Gets the render context that is used to draw the game session.
		/// </summary>
		public RenderContext RenderContext { get; private set; }

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
		///     Gets the input device that is used to capture all user input.
		/// </summary>
		public LogicalInputDevice InputDevice { get; private set; }

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
		///     Gets or sets a value indicating whether the server and client game state have been synced.
		/// </summary>
		public bool IsSynced { get; set; }

		/// <summary>
		///     Allocates an instance of the given type in the appropriate object pool.
		/// </summary>
		/// <typeparam name="T">The type of the object that should be allocated.</typeparam>
		public T Allocate<T>()
			where T : class, new()
		{
			if (!ConstructorCache.IsCached<T>())
				ConstructorCache.Set(() => new T());

			return Allocator.Allocate<T>();
		}

		/// <summary>
		///     Loads the game session and syncs the game state with the server. Returns true to indicate that loading has completed.
		/// </summary>
		public bool Load()
		{
			Connection.DispatchReceivedMessages(_messageHandler);
			Connection.SendQueuedMessages();

			if (!_assets.LoadPending(timeoutInMilliseconds: 10) || !IsSynced)
				return false;

			Assert.NotNull(LocalPlayer, "Game state synced but local player is unknown.");
			Assert.That(_assets.LoadingCompleted, "Not all assets have been loaded.");

			// Handle chat input and player name changes
			Commands.OnSay += OnSay;
			Cvars.PlayerNameChanged += OnPlayerNameChanged;

			// Resend player name, as it might have been changed during the connection attempt
			OnPlayerNameChanged(Cvars.PlayerName);

			// Initialize the input device and manager as well as the camera manager
			InputDevice = new LogicalInputDevice();
			InputManager = new InputManager(this);
			CameraManager = new CameraManager(LocalPlayer, InputDevice);

			// Initialize the render context
			RenderContext.Initialize();
			Assert.That(_assets.LoadingCompleted, "The render context loaded further assets.");

			// Clear the event messages that might contain incorrect events (such as join messages for players that
			// have already been playing while we're still connecting) and perform an update to fully initialize the game state
			EventMessages.Clear();
			_clock.Reset();
			Update(false);

			return true;
		}

		/// <summary>
		///     Updates the state of the game session.
		/// </summary>
		/// <param name="inGameMenuOpen">Indicates whether the in-game menu is currently open.</param>
		public void Update(bool inGameMenuOpen)
		{
			if (!inGameMenuOpen)
			{
				InputDevice.Update();
				InputManager.Update();
			}

			Connection.DispatchReceivedMessages(_messageHandler);
			CameraManager.Update();

			var elapsedSeconds = (float)_clock.Seconds;
			_clock.Reset();

			Entities.Update(elapsedSeconds);
			Actors.Update(elapsedSeconds);
			RootTransform.Update();
			EventMessages.Update();

			InputManager.SendInput();
			Connection.SendQueuedMessages();
		}

		/// <summary>
		///     Invoked when the local player changed his or her name.
		/// </summary>
		/// <param name="name">The previous name of the local player.</param>
		private void OnPlayerNameChanged(string name)
		{
			Connection.Send(PlayerNameMessage.Create(Allocator, LocalPlayer.Identity, Cvars.PlayerName));
		}

		/// <summary>
		///     Invoked when the local player entered a chat message.
		/// </summary>
		/// <param name="message">The message that the local player wants to send.</param>
		private void OnSay(string message)
		{
			Assert.NotNull(LocalPlayer);
			Connection.Send(PlayerChatMessage.Create(Allocator, LocalPlayer.Identity, message));
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Commands.OnSay -= OnSay;
			Cvars.PlayerNameChanged -= OnPlayerNameChanged;

			Connection.Disconnect();
			EntityTemplates.Dispose();
			Actors.SafeDispose();
			Entities.SafeDispose();
			Players.SafeDispose();
			CameraManager.SafeDispose();
			InputManager.SafeDispose();
			RenderContext.SafeDispose();
			InputDevice.SafeDispose();
			Connection.SafeDispose();
			Allocator.SafeDispose();

			_assets.SafeDispose();
		}
	}
}