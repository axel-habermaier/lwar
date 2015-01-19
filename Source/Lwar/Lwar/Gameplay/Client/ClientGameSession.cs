namespace Lwar.Gameplay.Client
{
	using System;
	using System.Collections.Generic;
	using Actors;
	using Assets;
	using Entities;
	using Network;
	using Network.Messages;
	using Pegasus;
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
	internal class ClientGameSession : DisposableObject
	{
		/// <summary>
		///     The assets used by the game session.
		/// </summary>
		private readonly GameBundle _assets;

		/// <summary>
		///     The dispatcher that is used to dispatch incoming messages from the server.
		/// </summary>
		private readonly MessageHandler _messageHandler;

		/// <summary>
		///     The clock that is used for time measurements.
		/// </summary>
		private Clock _clock = new Clock();

		/// <summary>
		///     Used to initialize the game session.
		/// </summary>
		private IEnumerator<bool> _initializationRoutine;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="serverEndPoint">The remote end point of the server.</param>
		public ClientGameSession(IPEndPoint serverEndPoint)
		{
			_messageHandler = new MessageHandler(this);
			_assets = new GameBundle(Application.Current.RenderContext);

			Allocator = new PoolAllocator();
			Renderer = new GameSessionRenderer();

			Actors = new ActorList(this, Renderer);
			Entities = new EntityList(this, Renderer);
			Players = new PlayerList(this);
			RootTransform = new Transformation();
			EventMessages = new EventMessageList(this);

			var channel = UdpChannel.Create(Allocator, serverEndPoint, NetworkProtocol.MaxPacketSize);
			Connection = Connection.Create(Allocator, channel);
			Connection.Send(ClientConnectMessage.Create(Allocator, Cvars.PlayerName));

			_initializationRoutine = Initialize().GetEnumerator();
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
		///     Gets the renderer that is used to draw the game session.
		/// </summary>
		public GameSessionRenderer Renderer { get; private set; }

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
				ConstructorCache.Register(() => new T());

			return Allocator.Allocate<T>();
		}

		/// <summary>
		///     Loads the game session and syncs the game state with the server. Returns true to indicate that loading has completed.
		/// </summary>
		public bool Load()
		{
			_initializationRoutine.MoveNext();
			if (!_initializationRoutine.Current)
				return false;

			_initializationRoutine = null;
			return true;
		}

		/// <summary>
		///     A state machine that initializes a game session.
		/// </summary>
		private IEnumerable<bool> Initialize()
		{
			while (!_assets.LoadAsync(timeoutInMilliseconds: 10))
				yield return false;

			// Initialize the entity templates
			EntityTemplates.Initialize(Application.Current.RenderContext);

			while (!IsSynced)
			{
				Connection.SendQueuedMessages();
				Connection.DispatchReceivedMessages(_messageHandler);
				yield return false;
			}

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

			// Initialize the renderer
			Renderer.Initialize();

			// Clear the event messages that might contain incorrect events (such as join messages for players that
			// have already been playing while we're still connecting) and perform an update to fully initialize the game state
			EventMessages.Clear();
			_clock.Reset();
			Update(false);

			yield return true;
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
			Renderer.SafeDispose();
			InputDevice.SafeDispose();
			Connection.SafeDispose();
			Allocator.SafeDispose();

			_assets.SafeDispose();
		}
	}
}