using System;

namespace Lwar.Client.Gameplay.Entities
{
	using System.Collections.Generic;
	using Network;
	using Network.Messages;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Memory;
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
			Assert.ArgumentNotNull(gameSession);
			Assert.ArgumentNotNull(renderContext);

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
			Assert.ArgumentNotNull(entity);
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

			entity.Removed();
		}

		/// <summary>
		///   Updates the entity list.
		/// </summary>
		/// <param name="clock">The clock that should be used for time measurements.</param>
		public void Update(Clock clock)
		{
			Assert.ArgumentNotNull(clock);
			_entities.Update();

			foreach (var entity in _entities)
				entity.Update(clock);
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

		/// <summary>
		///   Handles a collision between the two entities.
		/// </summary>
		/// <param name="entityIdentifier1">The identifier of the first entity of the collision.</param>
		/// <param name="entityIdentifier2">The identifier of the second entity of the collision.</param>
		/// <param name="impactPosition">The position of the impact.</param>
		public void OnCollision(Identifier entityIdentifier1, Identifier entityIdentifier2, Vector2 impactPosition)
		{
			var entity1 = this[entityIdentifier1];
			var entity2 = this[entityIdentifier2];

			if (entity1 == null || entity2 == null)
				return;

			entity1.CollidedWith(entity2, impactPosition);
			entity2.CollidedWith(entity1, impactPosition);
		}
	}
}