using System;

namespace Lwar.Client.Gameplay
{
	using System.Collections.Generic;
	using Pegasus.Framework;

	/// <summary>
	///   Manages a list of entities, delaying additions and removals of entities until the end of the frame.
	/// </summary>
	public class EntityList : DisposableObject
	{
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
			Assert.ArgumentSatisfies(!_entities.Contains(entity), () => entity, "The entity is already in the level.");

			_entities.Add(entity);
			_entitiesById[entity.Id.Id] = entity;
			entity.Added(_session);
		}

		/// <summary>
		///   Removes the given entity from the list at the end of the frame.
		/// </summary>
		/// <param name="id">The identifier of the entity that should be removed from the list.</param>
		public void Remove(Identifier id)
		{
			Remove(Find(id));
		}

		/// <summary>
		///   Removes the given entity from the list at the end of the frame.
		/// </summary>
		/// <param name="entity">The entity that should be removed from the list.</param>
		public void Remove(IEntity entity)
		{
			Assert.ArgumentNotNull(entity, () => entity);
			Assert.ArgumentSatisfies(_entities.Contains(entity), () => entity, "The entity is not in the level.");

			entity.Removed();
			_entities.Remove(entity);
			_entitiesById[entity.Id.Id] = null;
			entity.Dispose();
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
			_entities.SafeDisposeAll();
			_entities.Clear();
		}
	}
}