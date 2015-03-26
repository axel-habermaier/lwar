namespace Lwar.Gameplay.Server
{
	using System;
	using Entities;
	using Network;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;
	using Pegasus.Scene;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a client- or server-side game session.
	/// </summary>
	internal class GameSession : DisposableObject
	{
		/// <summary>
		///     Indicates whether the game session is used by a server.
		/// </summary>
		private bool _serverMode;

		/// <summary>
		///     Initializes a new client-side instance.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate game objects.</param>
		public GameSession(PoolAllocator allocator)
		{
			Assert.ArgumentNotNull(allocator);

			Allocator = allocator;
			SceneGraph = new SceneGraph(allocator);

			SceneGraph.NodeAdded += OnNodeAdded;
			SceneGraph.NodeRemoved += OnNodeRemoved;
		}

		/// <summary>
		///     Gets the allocator that is used to allocate game objects.
		/// </summary>
		public PoolAllocator Allocator { get; private set; }

		/// <summary>
		///     Gets the scene graph of the game session.
		/// </summary>
		public SceneGraph SceneGraph { get; private set; }

		/// <summary>
		///     Gets the collection of players that are participating in the game session.
		/// </summary>
		public PlayerCollection Players { get; private set; }

		/// <summary>
		///     Gets the physics simulation of the game session.
		/// </summary>
		public PhysicsSimulation Physics { get; private set; }

		/// <summary>
		///     Allocates an instance of the given type using the game session's allocator.
		/// </summary>
		/// <typeparam name="T">The type of the object that should be allocated.</typeparam>
		public T Allocate<T>()
			where T : class, new()
		{
			return Allocator.Allocate<T>();
		}

		/// <summary>
		///     Raised when an entity has been added to the game session.
		/// </summary>
		public event Action<Entity> EntityAdded;

		/// <summary>
		///     Raised when an entity has been removed from the game session.
		/// </summary>
		public event Action<Entity> EntityRemoved;

		/// <summary>
		///     If an entity has been added, raises the entity added event.
		/// </summary>
		/// <param name="sceneNode">The scene node that has been added.</param>
		private void OnNodeAdded(SceneNode sceneNode)
		{
			var entity = sceneNode as Entity;
			if (entity != null && EntityAdded != null)
				EntityAdded(entity);
		}

		/// <summary>
		///     If an entity has been removed, raises the entity removed event.
		/// </summary>
		/// <param name="sceneNode">The scene node that has been removed.</param>
		private void OnNodeRemoved(SceneNode sceneNode)
		{
			var entity = sceneNode as Entity;
			if (entity != null && EntityRemoved != null)
				EntityRemoved(entity);
		}

		/// <summary>
		///     Initializes a client-side game session.
		/// </summary>
		public void InitializeClient()
		{
			_serverMode = false;

			Players = new PlayerCollection(Allocator, serverMode: false);
		}

		/// <summary>
		///     Initializes a server-side game session.
		/// </summary>
		/// <param name="serverLogic">The server logic that handles the communication between the server and the clients.</param>
		public void InitializeServer(ServerLogic serverLogic)
		{
			Assert.ArgumentNotNull(serverLogic);

			_serverMode = true;

			Physics = new PhysicsSimulation(this);
			Players = new PlayerCollection(Allocator, serverMode: true);

			CreateGalaxy();
		}

		/// <summary>
		///     Updates the state of the game session.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		public void Update(float elapsedSeconds)
		{
			foreach (var entity in SceneGraph.EnumeratePreOrder<Entity>())
				entity.ServerUpdate(elapsedSeconds);

			SceneGraph.ExecuteBehaviors(elapsedSeconds);
			Physics.Simulate(elapsedSeconds);
		}

		/// <summary>
		///     Draws the game session to the given render output.
		/// </summary>
		/// <param name="renderOutput">The render output the game session should be drawn to.</param>
		public void Draw(RenderOutput renderOutput)
		{
			Assert.ArgumentNotNull(renderOutput);
			Assert.That(!_serverMode, "A server cannot draw the game session.");
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			SceneGraph.SafeDispose();
			Players.SafeDispose();
		}

		/// <summary>
		///     Creates the galaxy with suns and planets.
		/// </summary>
		private void CreateGalaxy()
		{
			var sun = Sun.Create(this, Vector2.Zero);
			sun.AttachTo(SceneGraph.Root);

			var earth = Planet.Create(this, sun, EntityType.Earth, 12000, 0.05f, 0);

			Planet.Create(this, earth, EntityType.Moon, 1500, 0.7f, MathUtils.PiOver4);
			Planet.Create(this, sun, EntityType.Mars, 14000, -0.07f, MathUtils.Pi);
			Planet.Create(this, sun, EntityType.Jupiter, 20000, -0.085f, MathUtils.PiOver2);
		}
	}
}