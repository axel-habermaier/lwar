using System;

namespace Lwar.Client.Gameplay
{
	using System.Collections.Generic;
	using Pegasus.Framework;
	using Pegasus.Gameplay;

	/// <summary>
	///   Manages a list of entities, delaying additions and removals of entities until the end of the frame.
	/// </summary>
	public class EntityList : DisposableObject
	{
		/// <summary>
		///   The entities that have been added during the current frame.
		/// </summary>
		private readonly List<IEntity> _added = new List<IEntity>(8);

		/// <summary>
		///   The list of all entities in the level.
		/// </summary>
		private readonly List<IEntity> _entities = new List<IEntity>(1024);

		/// <summary>
		///   Maps each entity identifier to the corresponding entity instance, if the entity is currently in the level.
		///   When an entity is retrieved by its identifier, the entity's generation must be checked as well to ensure
		///   that the correct version of the entity is used.
		/// </summary>
		private readonly IEntity[] _entitiesById = new IEntity[UInt16.MaxValue];

		/// <summary>
		///   The entities that have been removed during the current frame.
		/// </summary>
		private readonly List<IEntity> _removed = new List<IEntity>(8);

		/// <summary>
		///   The game session the entities belong to.
		/// </summary>
		private readonly GameSession _session;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="session"> The game session the entities belong to.</param>
		public EntityList(GameSession session)
		{
			Assert.ArgumentNotNull(session, () => session);

			_session = session;
		}

		/// <summary>
		///   Adds the given entity to the list at the end of the frame. The entity is initialized immediately, however.
		/// </summary>
		/// <param name="entity">The entity that should be added to the list.</param>
		public void Add(IEntity entity)
		{
			Assert.ArgumentNotNull(entity, () => entity);
			Assert.ArgumentSatisfies(!_added.Contains(entity), () => entity, "The entity has already been added during this frame.");
			Assert.ArgumentSatisfies(!_entities.Contains(entity), () => entity, "The entity is already in the level.");
			Assert.ArgumentSatisfies(!_removed.Contains(entity), () => entity, "The entity has just been removed during this frame.");

			// Do not accept any new entities when the entity list is being disposed
			if (IsDisposing)
			{
				entity.Dispose();
				return;
			}

			_added.Add(entity);
			_entitiesById[entity.Id.Id] = entity;

			entity.Id = entity.Id.IncreaseGenerationCount();
			entity.Added(_session);
		}

		/// <summary>
		///   Removes the given entity from the list at the end of the frame and returns
		///   the entity instance to the pool for later re-use. The entity is disposed immediately, however.
		/// </summary>
		/// <param name="entity">The entity that should be removed from the list.</param>
		public void Remove(IEntity entity)
		{
			Assert.ArgumentNotNull(entity, () => entity);
			Assert.ArgumentSatisfies(!_added.Contains(entity), () => entity, "The entity has just been added during this frame.");
			Assert.ArgumentSatisfies(_entities.Contains(entity), () => entity, "The entity is not in the level.");
			Assert.ArgumentSatisfies(!_removed.Contains(entity), () => entity,
									 "The entity has already been removed during this frame.");

			_removed.Add(entity);
			entity.Removed();
		}

		/// <summary>
		///   Returns the entity instance with the given id, if the entity is currently in the level.
		/// </summary>
		/// <param name="id">The id of the entity that should be returned.</param>
		public IEntity Find(Identifier id)
		{
			var entity = _entitiesById[id.Id];

			// The entity might have been removed in the meantime
			if (entity == null)
				return null;

			// The entity might have been removed in the meantime, but added again as a logically different entity
			if (entity.Id.Generation != id.Generation)
				return null;

			return entity;
		}

		/// <summary>
		///   Returns the entity instance with the given id, if the entity is currently in the level.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity that should be returned.</typeparam>
		/// <param name="id">The id of the entity that should be returned.</param>
		public TEntity Find<TEntity>(Identifier id)
			where TEntity : class, IEntity
		{
			return Find(id) as TEntity;
		}

		/// <summary>
		///   Carries out all pending additions and removals and updates all entities.
		/// </summary>
		public void Update()
		{
			foreach (var entity in _removed)
			{
				_entities.Remove(entity);
				_entitiesById[entity.Id.Id] = null;

				entity.Dispose();
			}

			_entities.AddRange(_added);

			_removed.Clear();
			_added.Clear();

			foreach (var entity in _entities)
				entity.Update();
		}

		/// <summary>
		///   Draws all entities.
		/// </summary>
		public void Draw()
		{
			foreach (var entity in _entities)
				entity.Draw();
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			// Remove all entities
			foreach (var entity in _entities)
				Remove(entity);

			// Update the entity list; this guarantees that all entities are removed normally without introducing a special case
			Update();

			Assert.That(_entities.Count == 0, "There are some active entities left.");
			Assert.That(_added.Count == 0, "There are some entities left that should be added.");
			Assert.That(_removed.Count == 0, "There are some entities left that should be removed.");
		}
	}
}