namespace Lwar.Gameplay.Server.Scripts
{
	using System;
	using Components;
	using Pegasus.Entities;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     A base class for entity scripts.
	/// </summary>
	public abstract class Script : UniquePooledObject
	{
		/// <summary>
		///     The player input component of the entity the script is attached to.
		/// </summary>
		private PlayerInput _input;

		/// <summary>
		///     The motion component of the entity the script is attached to.
		/// </summary>
		private Motion _motion;

		/// <summary>
		///     The owner component of the entity the script is attached to.
		/// </summary>
		private Owner _owner;

		/// <summary>
		///     The transform component of the entity the script is attached to.
		/// </summary>
		private Transform _transform;

		/// <summary>
		///     Gets an entity factory that can be used to create entities.
		/// </summary>
		protected EntityFactory EntityFactory
		{
			get { return GameSession.EntityFactory; }
		}

		/// <summary>
		///     Gets the player input component of the entity the script is attached to.
		/// </summary>
		protected PlayerInput Input
		{
			get { return _input ?? (_input = Entity.GetRequiredComponent<PlayerInput>()); }
		}

		/// <summary>
		///     Gets the motion component of the entity the script is attached to.
		/// </summary>
		protected Motion Motion
		{
			get { return _motion ?? (_motion = Entity.GetRequiredComponent<Motion>()); }
		}

		/// <summary>
		///     Gets the owner component of the entity the script is attached to.
		/// </summary>
		protected Owner Owner
		{
			get { return _owner ?? (_owner = Entity.GetRequiredComponent<Owner>()); }
		}

		/// <summary>
		///     Gets the transform component of the entity the script is attached to.
		/// </summary>
		protected Transform Transform
		{
			get { return _transform ?? (_transform = Entity.GetRequiredComponent<Transform>()); }
		}

		/// <summary>
		///     Gets the entity the script belongs to.
		/// </summary>
		protected Entity Entity { get; private set; }

		/// <summary>
		///     Gets the allocator that is used to initialize game objects.
		/// </summary>
		protected PoolAllocator Allocator { get; private set; }

		/// <summary>
		///     Gets the game session the scripted entity belongs to.
		/// </summary>
		protected GameSession GameSession { get; private set; }

		/// <summary>
		///     Initializes the script when the entity has been added to the game session.
		/// </summary>
		protected virtual void Initialize()
		{
		}

		/// <summary>
		///     Deinitializes the script when the entity has been removed from the game session.
		/// </summary>
		protected virtual void Deinitialize()
		{
		}

		/// <summary>
		///     Updates the state of the script.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		public virtual void Update(float elapsedSeconds)
		{
		}

		/// <summary>
		///     Activates the script.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate the component.</param>
		/// <param name="gameSession">The game session the scripted entity belongs to.</param>
		/// <param name="entity">The entity the script belongs to.</param>
		public void Activate(PoolAllocator allocator, GameSession gameSession, Entity entity)
		{
			Assert.ArgumentNotNull(allocator);
			Assert.ArgumentNotNull(gameSession);

			Allocator = allocator;
			GameSession = gameSession;
			Entity = entity;

			_input = null;
			_motion = null;
			_owner = null;
			_transform = null;

			Initialize();
		}

		/// <summary>
		///     Deactivates the script.
		/// </summary>
		public void Deactivate()
		{
			Deinitialize();
		}
	}
}