using System;

namespace Lwar.Client.Gameplay
{
	using System.Collections.Generic;
	using Entities;
	using Network;
	using Pegasus.Framework;
	using Rendering;

	/// <summary>
	///   Manages the active entities of a game session.
	/// </summary>
	public sealed class EntityList : DisposableObject
	{
		/// <summary>
		///   The list of active entities.
		/// </summary>
		private readonly DeferredList<IEntity> _entities = new DeferredList<IEntity>(false);

		/// <summary>
		///   Maps generational identifiers to entity instances.
		/// </summary>
		private readonly IdentifierMap<IEntity> _entityMap = new IdentifierMap<IEntity>();

		/// <summary>
		///   The game session the entity list belongs to.
		/// </summary>
		private readonly GameSession _gameSession;

		/// <summary>
		///   The render context that is used to draw the entities.
		/// </summary>
		private readonly RenderContext _renderContext;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the entity list belongs to.</param>
		/// <param name="renderContext">The render context that should be used to draw the entities.</param>
		public EntityList(GameSession gameSession, RenderContext renderContext)
		{
			Assert.ArgumentNotNull(gameSession, () => gameSession);
			Assert.ArgumentNotNull(renderContext, () => renderContext);

			_gameSession = gameSession;
			_renderContext = renderContext;
		}

		/// <summary>
		///   Gets the entity that corresponds to the given identifier. Returns null if no entity with the given identifier could
		///   be found, or if the generation did not match.
		/// </summary>
		/// <param name="identifier">The identifier of the entity that should be returned.</param>
		public IEntity this[Identifier identifier]
		{
			get { return _entityMap[identifier]; }
		}

		/// <summary>
		///   Adds the given entity to the list.
		/// </summary>
		/// <param name="entity">The entity that should be added.</param>
		public void Add(IEntity entity)
		{
			Assert.ArgumentNotNull(entity, () => entity);
			Assert.That(_entityMap[entity.Id] == null, "An entity with the same id has already been added.");

			_entities.Add(entity);
			_entityMap.Add(entity);

			entity.Added(_gameSession, _renderContext);
		}

		/// <summary>
		///   Removes the entity with the given id from the list.
		/// </summary>
		/// <param name="entityId">The identifier of the entity that should be removed.</param>
		public void Remove(Identifier entityId)
		{
			var entity = _entityMap[entityId];
			Assert.NotNull(entity, "Cannot remove unknown entity.");

			_entities.Remove(entity);
			_entityMap.Remove(entity);

			entity.Removed(_gameSession, _renderContext);
		}

		/// <summary>
		///   Updates the entity list.
		/// </summary>
		public void Update()
		{
			_entities.Update();

			foreach (var entity in _entities)
				entity.Update();
		}

		/// <summary>
		///   Enumerates all active entities.
		/// </summary>
		public List<IEntity>.Enumerator GetEnumerator()
		{
			return _entities.GetEnumerator();
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_entities.SafeDispose();
		}

		/// <summary>
		///   Updates the entity referenced in the update message.
		/// </summary>
		/// <param name="message">The remote update message that should be processed.</param>
		public void RemoteUpdate(ref Message message)
		{
			var entity = _entityMap[message.Update.Entity];
			if (entity == null)
				return;

			entity.RemoteUpdate(ref message);
		}
	}
}