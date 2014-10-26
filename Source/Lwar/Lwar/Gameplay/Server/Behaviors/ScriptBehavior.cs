namespace Lwar.Gameplay.Server.Behaviors
{
	using System;
	using Components;
	using Pegasus.Entities;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Executes script components.
	/// </summary>
	public class ScriptBehavior : EntityBehavior<ScriptCollection>
	{
		/// <summary>
		///     The allocator that is used to initialize game objects.
		/// </summary>
		private readonly PoolAllocator _allocator;

		/// <summary>
		///     The game session the behavior belongs to.
		/// </summary>
		private readonly GameSession _gameSession;

		/// <summary>
		///     The number of seconds that have elapsed since the last update.
		/// </summary>
		private float _elapsedSeconds;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to initialize game objects.</param>
		/// <param name="gameSession">The game session the behavior belongs to.</param>
		public ScriptBehavior(PoolAllocator allocator, GameSession gameSession)
		{
			Assert.ArgumentNotNull(allocator);
			Assert.ArgumentNotNull(gameSession);

			_allocator = allocator;
			_gameSession = gameSession;
		}

		/// <summary>
		///     Updates the scripts.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		public void Update(float elapsedSeconds)
		{
			_elapsedSeconds = elapsedSeconds;
			Process();
		}

		/// <summary>
		///     Invoked when the given entity is affected by the behavior.
		/// </summary>
		/// <param name="entity">The entity that is affected by the behavior.</param>
		/// <param name="scriptCollection">The script collection of the entity that is affected by the behavior.</param>
		protected override void OnAdded(Entity entity, ScriptCollection scriptCollection)
		{
			scriptCollection.OnAdded(_allocator, _gameSession, entity);
		}

		/// <summary>
		///     Invoked when the given entity is no longer affected by the behavior.
		/// </summary>
		/// <param name="entity">The entity that is no longer affected by the behavior.</param>
		/// <param name="scriptCollection">The script collection of the entity that is no longer affected by the behavior.</param>
		protected override void OnRemoved(Entity entity, ScriptCollection scriptCollection)
		{
			scriptCollection.OnRemoved();
		}

		/// <summary>
		///     Processes the entities affected by the behavior.
		/// </summary>
		/// <param name="entities">The entities affected by the behavior.</param>
		/// <param name="scripts">The script components of the affected entities.</param>
		/// <param name="count">The number of entities that should be processed.</param>
		/// <remarks>
		///     All arrays have the same length. If a component is optional for an entity and the component is missing, a null
		///     value is placed in the array.
		/// </remarks>
		protected override void Process(Entity[] entities, ScriptCollection[] scripts, int count)
		{
			for (var i = 0; i < count; ++i)
				scripts[i].Update(_elapsedSeconds);
		}
	}
}