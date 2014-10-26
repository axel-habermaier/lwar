namespace Lwar.Gameplay.Server
{
	using System;
	using Behaviors;
	using Network;
	using Pegasus.Entities;
	using Pegasus.Math;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a client- or server-side game session.
	/// </summary>
	public class GameSession : DisposableObject
	{
		/// <summary>
		///     The allocator that is used to allocate game objects.
		/// </summary>
		private readonly PoolAllocator _allocator;

		/// <summary>
		///     The entity behaviors used by the game session.
		/// </summary>
		private readonly EntityBehaviorCollection _behaviors;

		/// <summary>
		///     Dispatches entity events.
		/// </summary>
		private readonly EventDispatcher _eventDispatcher = new EventDispatcher();

		/// <summary>
		///     The behavior that removes entities at invalid positions.
		/// </summary>
		private BoundaryBehavior _boundaryBehavior;

		/// <summary>
		///     The behavior that applies the player inputs to the entities.
		/// </summary>
		private PlayerInputBehavior _playerInputBehavior;

		/// <summary>
		///     The behavior that moves and accelerates entities.
		/// </summary>
		private MotionBehavior _motionBehavior;

		/// <summary>
		///     The behavior that lets entities orbit around other entities.
		/// </summary>
		private OrbitBehavior _orbitBehavior;

		/// <summary>
		///     The behavior that updates relative entity transforms.
		/// </summary>
		private RelativeTransformBehavior _relativeTransformBehavior;

		/// <summary>
		///     The behavior that rotates entities based on their angular velocity.
		/// </summary>
		private RotationBehavior _rotationBehavior;

		/// <summary>
		///     The behavior that executes entity scripts.
		/// </summary>
		private ScriptBehavior _scriptBehavior;

		/// <summary>
		///     Indicates whether the game session is used by a server.
		/// </summary>
		private bool _serverMode;

		/// <summary>
		///     The sprite behavior that is used to draw sprite components.
		/// </summary>
		private SpriteBehavior _spriteBehavior;

		/// <summary>
		///     The server network behavior that is used to synchronize entities with all connected clients.
		/// </summary>
		private SyncToClientsBehavior _syncToClientsBehavior;

		/// <summary>
		///     The behavior that removes entities after their lifetime ran out.
		/// </summary>
		private TimeToLiveBehavior _timeToLiveBehavior;

		/// <summary>
		///     Initializes a new client-side instance.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate game objects.</param>
		public GameSession(PoolAllocator allocator)
		{
			Assert.ArgumentNotNull(allocator);

			Entities = new EntityCollection(allocator, _eventDispatcher);

			_allocator = allocator;
			_behaviors = new EntityBehaviorCollection(_eventDispatcher);
		}

		/// <summary>
		///     Gets the entities of the game session.
		/// </summary>
		public EntityCollection Entities { get; private set; }

		/// <summary>
		///     Gets the collection of players that are participating in the game session.
		/// </summary>
		public PlayerCollection Players { get; private set; }

		/// <summary>
		///     Gets the entity templates that are can be used to create entities.
		/// </summary>
		public EntityTemplates Templates { get; private set; }

		/// <summary>
		///     Initializes a client-side game session.
		/// </summary>
		public void InitializeClient()
		{
			_serverMode = false;

			Templates = new EntityTemplates(_allocator, Entities, serverMode: false);
			Players = new PlayerCollection(_allocator, serverMode: false);

			_behaviors.Add(_spriteBehavior = new SpriteBehavior());
			_behaviors.Add(_scriptBehavior = new ScriptBehavior(_allocator, this));
			_behaviors.Add(_relativeTransformBehavior = new RelativeTransformBehavior());
		}

		/// <summary>
		///     Initializes a server-side game session.
		/// </summary>
		/// <param name="serverLogic">The server logic that handles the communication between the server and the clients.</param>
		public void InitializeServer(ServerLogic serverLogic)
		{
			Assert.ArgumentNotNull(serverLogic);

			_serverMode = true;

			Templates = new EntityTemplates(_allocator, Entities, serverMode: true);
			Players = new PlayerCollection(_allocator, serverMode: true);

			_behaviors.Add(_syncToClientsBehavior = new SyncToClientsBehavior(_allocator, serverLogic));
			_behaviors.Add(_motionBehavior = new MotionBehavior());
			_behaviors.Add(_rotationBehavior = new RotationBehavior());
			_behaviors.Add(_playerInputBehavior = new PlayerInputBehavior());
			_behaviors.Add(_boundaryBehavior = new BoundaryBehavior());
			_behaviors.Add(_timeToLiveBehavior = new TimeToLiveBehavior());
			_behaviors.Add(_orbitBehavior = new OrbitBehavior());
			_behaviors.Add(_scriptBehavior = new ScriptBehavior(_allocator, this));
			_behaviors.Add(_relativeTransformBehavior = new RelativeTransformBehavior());

			CreateGalaxy();
		}

		/// <summary>
		///     Updates the state of the game session.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		public void Update(float elapsedSeconds)
		{
			Entities.ApplyChanges();

			if (!_serverMode)
			{
				_relativeTransformBehavior.Update();
				return;
			}

			_playerInputBehavior.ApplyInput();
			_scriptBehavior.Update(elapsedSeconds);
			_orbitBehavior.UpdateOrbits(elapsedSeconds);
			_rotationBehavior.Update(elapsedSeconds);
			_motionBehavior.Update(elapsedSeconds);
			_timeToLiveBehavior.RemoveDeadEntities(elapsedSeconds);
			_relativeTransformBehavior.Update();
			_boundaryBehavior.RemoveEntitiesWithInvalidPositions();
			_syncToClientsBehavior.SendEntityUpdates();
		}

		/// <summary>
		///     Draws the game session to the given render output.
		/// </summary>
		/// <param name="renderOutput">The render output the game session should be drawn to.</param>
		public void Draw(RenderOutput renderOutput)
		{
			Assert.ArgumentNotNull(renderOutput);
			Assert.That(!_serverMode, "A server cannot draw the game session.");

			_spriteBehavior.Draw(renderOutput);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_behaviors.SafeDispose();

			Players.SafeDispose();
			Entities.SafeDispose();
		}

		/// <summary>
		///     Creates the galaxy with suns and planets.
		/// </summary>
		private void CreateGalaxy()
		{
			var sun = Templates.CreateSun(Players.ServerPlayer);
			var earth = Templates.CreatePlanet(Players.ServerPlayer, sun, EntityType.Earth, 12000, 0.05f, 0);

			Templates.CreatePlanet(Players.ServerPlayer, earth, EntityType.Moon, 1500, 0.7f, MathUtils.PiOver4);
			Templates.CreatePlanet(Players.ServerPlayer, sun, EntityType.Mars, 14000, -0.07f, MathUtils.Pi);
			Templates.CreatePlanet(Players.ServerPlayer, sun, EntityType.Jupiter, 20000, -0.085f, MathUtils.PiOver2);
		}
	}
}