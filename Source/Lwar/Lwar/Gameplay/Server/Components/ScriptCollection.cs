namespace Lwar.Gameplay.Server.Components
{
	using System;
	using System.Collections.Generic;
	using Pegasus.Entities;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;
	using Scripts;

	/// <summary>
	///     Represents collection of scripts that affect an entity.
	/// </summary>
	public sealed class ScriptCollection : Component
	{
		/// <summary>
		///     The scripts the collection consists of.
		/// </summary>
		private readonly List<Script> _scripts = new List<Script>();

		/// <summary>
		///     Indicates whether the script collection is active.
		/// </summary>
		private bool _isActive;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static ScriptCollection()
		{
			ConstructorCache.Register(() => new ScriptCollection());
		}

		/// <summary>
		///     Adds the given script to the collection.
		/// </summary>
		/// <param name="script">The script that should be added.</param>
		public void Add(Script script)
		{
			Assert.ArgumentNotNull(script);
			Assert.ArgumentSatisfies(!_scripts.Contains(script), "The script has already been added to the collection.");
			Assert.That(!_isActive, "The script collection is already active and cannot be modified.");

			_scripts.Add(script);
		}

		/// <summary>
		///     Updates the state of the script.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		public void Update(float elapsedSeconds)
		{
			foreach (var script in _scripts)
				script.Update(elapsedSeconds);
		}

		/// <summary>
		///     Invoked when the entity the script collection belongs to has been added to the game session.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate the component.</param>
		/// <param name="gameSession">The game session the scripted entity belongs to.</param>
		/// <param name="entity">The entity the script belongs to.</param>
		public void OnAdded(PoolAllocator allocator, GameSession gameSession, Entity entity)
		{
			Assert.ArgumentNotNull(allocator);
			Assert.ArgumentNotNull(gameSession);

			_isActive = true;
			foreach (var script in _scripts)
				script.Activate(allocator, gameSession, entity);
		}

		/// <summary>
		///     Invoked when the entity the script collection belongs to has been removed from the game session.
		/// </summary>
		public void OnRemoved()
		{
			foreach (var script in _scripts)
				script.Deactivate();

			_isActive = false;
			_scripts.SafeDisposeAll();
		}

		/// <summary>
		///     Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override void OnReturning()
		{
			_isActive = false;
			_scripts.SafeDisposeAll();

			base.OnReturning();
		}

		/// <summary>
		///     Allocates a component using the given allocator.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate the component.</param>
		public static ScriptCollection Create(PoolAllocator allocator)
		{
			Assert.ArgumentNotNull(allocator);
			return allocator.Allocate<ScriptCollection>();
		}
	}
}