namespace Lwar.Gameplay.Client.Entities
{
	using System;
	using System.Collections.Generic;
	using Network;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;
	using Rendering;

	/// <summary>
	///     Manages the active entities of a game session.
	/// </summary>
	internal sealed class EntityList : DisposableObject
	{
		/// <summary>
		///     The list of active entities.
		/// </summary>
		private readonly List<IEntity> _entities = new List<IEntity>();

		/// <summary>
		///     Maps generational identities to entity instances.
		/// </summary>
		private readonly IdentifierMap<IEntity> _entityMap = new IdentifierMap<IEntity>(UInt16.MaxValue);

		/// <summary>
		///     The game session the entity list belongs to.
		/// </summary>
		private readonly ClientGameSession _gameSession;

		/// <summary>
		///     The render context that is used to draw the entities.
		/// </summary>
		private readonly GameSessionRenderer _renderer;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the entity list belongs to.</param>
		/// <param name="renderer">The render context that should be used to draw the entities.</param>
		public EntityList(ClientGameSession gameSession, GameSessionRenderer renderer)
		{
			Assert.ArgumentNotNull(gameSession);
			Assert.ArgumentNotNull(renderer);

			_gameSession = gameSession;
			_renderer = renderer;
		}

		/// <summary>
		///     Gets the entity that corresponds to the given identity. Returns null if no entity with the given identity could
		///     be found, or if the generation did not match.
		/// </summary>
		/// <param name="identity">The identity of the entity that should be returned.</param>
		public IEntity this[NetworkIdentity identity]
		{
			get { return _entityMap[identity]; }
		}

		/// <summary>
		///     Adds the given entity.
		/// </summary>
		/// <param name="entityIdentifier">The identity of the entity that should be added.</param>
		/// <param name="playerIdentifier">The identity of the player the added entity belongs to.</param>
		/// <param name="parentEntity">The parent of the added entity.</param>
		/// <param name="entityType">The type of the entity that should be added.</param>
		public void Add(NetworkIdentity entityIdentifier, NetworkIdentity playerIdentifier, NetworkIdentity parentEntity, EntityType entityType)
		{
			var player = _gameSession.Players[playerIdentifier];
			Assert.NotNull(player, "Cannot add entity for unknown player.");
			Assert.ArgumentSatisfies(parentEntity == NetworkProtocol.ReservedEntityIdentity || _entityMap.Contains(parentEntity),
				"Tried to add an entity with an unknown parent.");

			IEntity entity;
			switch (entityType)
			{
				case EntityType.Ship:
					entity = ShipEntity.Create(_gameSession, entityIdentifier, player);
					break;
				case EntityType.Earth:
					entity = PlanetEntity.Create(_gameSession, entityIdentifier, EntityTemplates.Earth);
					break;
				case EntityType.Mars:
					entity = PlanetEntity.Create(_gameSession, entityIdentifier, EntityTemplates.Mars);
					break;
				case EntityType.Moon:
					entity = PlanetEntity.Create(_gameSession, entityIdentifier, EntityTemplates.Moon);
					break;
				case EntityType.Jupiter:
					entity = PlanetEntity.Create(_gameSession, entityIdentifier, EntityTemplates.Jupiter);
					break;
				case EntityType.Sun:
					entity = SunEntity.Create(_gameSession, entityIdentifier);
					break;
				case EntityType.Bullet:
					entity = BulletEntity.Create(_gameSession, entityIdentifier);
					break;
				case EntityType.Rocket:
					entity = RocketEntity.Create(_gameSession, entityIdentifier);
					break;
				case EntityType.Phaser:
					entity = PhaserEntity.Create(_gameSession, entityIdentifier);
					break;
				case EntityType.Ray:
					entity = RayEntity.Create(_gameSession, entityIdentifier);
					break;
				case EntityType.Shockwave:
					entity = ShockwaveEntity.Create(_gameSession, entityIdentifier);
					break;
				case EntityType.Gun:
					entity = GunEntity.Create(_gameSession, entityIdentifier);
					break;
				default:
					throw new InvalidOperationException("Unexpected entity type.");
			}

			if (parentEntity == NetworkProtocol.ReservedEntityIdentity)
				entity.Parent = null;
			else
				entity.Parent = _entityMap[parentEntity];
		}

		/// <summary>
		///     Adds the given entity to the list.
		/// </summary>
		/// <param name="entity">The entity that should be added.</param>
		public void Add(IEntity entity)
		{
			Assert.ArgumentNotNull(entity);
			Assert.ArgumentSatisfies(!_entityMap.Contains(entity.Identifier), "An entity with the same id has already been added.");

			_entities.Add(entity);
			_entityMap.Add(entity.Identifier, entity);

			entity.Added(_gameSession, _renderer);
		}

		/// <summary>
		///     Removes the entity with the given id from the list.
		/// </summary>
		/// <param name="entityId">The identity of the entity that should be removed.</param>
		public void Remove(NetworkIdentity entityId)
		{
			Assert.ArgumentSatisfies(_entityMap.Contains(entityId), "Cannot remove unknown entity.");
			var entity = _entityMap[entityId];

			_entityMap.Remove(entity.Identifier);
			entity.Removed();
		}

		/// <summary>
		///     Updates the entity list.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		public void Update(float elapsedSeconds)
		{
			for (var i = 0; i < _entities.Count; ++i)
			{
				if (_entities[i].IsRemoved)
				{
					_entities[i].SafeDispose();
					_entities[i] = _entities[_entities.Count - 1];
					_entities.RemoveAt(_entities.Count - 1);
					--i;
				}
				else
					_entities[i].Update(elapsedSeconds);
			}
		}

		/// <summary>
		///     Handles a collision between two entities.
		/// </summary>
		/// <param name="entityIdentity1">The identity of the first entity of the collision.</param>
		/// <param name="entityIdentity2">The identity of the second entity of the collision.</param>
		/// <param name="impactPosition">The position of the impact.</param>
		public void OnCollision(NetworkIdentity entityIdentity1, NetworkIdentity entityIdentity2, Vector2 impactPosition)
		{
			var entity1 = _gameSession.Entities[entityIdentity1];
			var entity2 = _gameSession.Entities[entityIdentity2];

			if (entity1 == null || entity2 == null)
				return;

			entity1.OnCollision(entity2, impactPosition);
			entity2.OnCollision(entity1, impactPosition);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_entities.SafeDisposeAll();
		}
	}
}