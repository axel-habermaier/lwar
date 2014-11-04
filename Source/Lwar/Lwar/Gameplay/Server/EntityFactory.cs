namespace Lwar.Gameplay.Server
{
	using System;
	using Components;
	using Network;
	using Network.Messages;
	using Pegasus.Entities;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Provides factory methods for the creation of entity types.
	/// </summary>
	public class EntityFactory
	{
		/// <summary>
		///     The allocator that is used to allocate components and entities.
		/// </summary>
		private readonly PoolAllocator _allocator;

		/// <summary>
		///     The entity collection that is used to create the entities.
		/// </summary>
		private readonly EntityCollection _entityCollection;

		/// <summary>
		///     Indicates whether entities are created for use by a server.
		/// </summary>
		private readonly bool _serverMode;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate components and entities.</param>
		/// <param name="entityCollection">The entity collection that should be used to create the entities.</param>
		/// <param name="serverMode">Indicates whether the entities are created for use by a server.</param>
		public EntityFactory(PoolAllocator allocator, EntityCollection entityCollection, bool serverMode)
		{
			Assert.ArgumentNotNull(allocator);
			Assert.ArgumentNotNull(entityCollection);

			_allocator = allocator;
			_entityCollection = entityCollection;
			_serverMode = serverMode;
		}

		/// <summary>
		///     Creates a ship entity.
		/// </summary>
		/// <param name="player">The player the ship belongs to.</param>
		/// <param name="identity">The network identity of the ship.</param>
		/// <param name="position">The initial position of the ship.</param>
		/// <param name="orientation">The initial orientation of the ship.</param>
		public Entity CreateShip(Player player, Identity identity = default(Identity),
								 Vector2 position = default(Vector2), float orientation = 0)
		{
			Assert.ArgumentNotNull(player);

			var entity = _entityCollection.CreateEntity();
			entity.Add(Transform.Create(_allocator, position, orientation));
			entity.Add(NetworkSync.Create(_allocator, identity, EntityType.Ship, MessageType.UpdateShip));
			entity.Add(Owner.Create(_allocator, player));
			entity.Add(Motion.Create(_allocator));
			entity.Add(Rotation.Create(_allocator, maxSpeed: 7));
			entity.Add(ScriptCollection.Create(_allocator));
			entity.Add(Propulsion.Create(_allocator, maxAcceleration: 7000, maxSpeed: 1000, maxAfterBurnerSpeed: 3000,
				maxEnergy: 1000, minRequiredEnergy: 100, rechargeDelay: 1, rechargeSpeed: 200, depleteSpeed: 300));

			if (!_serverMode)
				entity.Add(Sprite.Create(_allocator, null));
			else
				entity.Add(PlayerInput.Create(_allocator));

			return entity;
		}

		/// <summary>
		///     Creates a sun entity.
		/// </summary>
		/// <param name="player">The player the sun belongs to.</param>
		/// <param name="identity">The network identity of the sun.</param>
		/// <param name="position">The position of the sun.</param>
		public Entity CreateSun(Player player, Identity identity = default(Identity), Vector2 position = default(Vector2))
		{
			Assert.ArgumentNotNull(player);

			var entity = _entityCollection.CreateEntity();
			entity.Add(Transform.Create(_allocator, position));
			entity.Add(NetworkSync.Create(_allocator, identity, EntityType.Sun, MessageType.UpdatePosition));
			entity.Add(Owner.Create(_allocator, player));

			return entity;
		}

		/// <summary>
		///     Creates a planet entity.
		/// </summary>
		/// <param name="player">The player the planet belongs to.</param>
		/// <param name="identity">The network identity of the planet.</param>
		/// <param name="orbitedEntity">The entity that is orbitted.</param>
		/// <param name="planetType">The type of the planet.</param>
		/// <param name="orbitRadius">The radius of the planet's orbit.</param>
		/// <param name="orbitSpeed">
		///     The orbital speed of the planet. The sign of the speed determines the direction of the orbital movement.
		/// </param>
		/// <param name="orbitOffset">An offset to the position on the orbital trajectory.</param>
		public Entity CreatePlanet(Player player, Entity orbitedEntity, EntityType planetType, float orbitRadius, float orbitSpeed,
								   float orbitOffset, Identity identity = default(Identity))
		{
			Assert.ArgumentNotNull(player);
			Assert.ArgumentSatisfies(planetType == EntityType.Earth || planetType == EntityType.Mars ||
									 planetType == EntityType.Jupiter || planetType == EntityType.Moon, "Invalid planet type.");

			var entity = _entityCollection.CreateEntity();
			entity.Add(Transform.Create(_allocator));
			entity.Add(NetworkSync.Create(_allocator, identity, planetType, MessageType.UpdatePosition));
			entity.Add(Owner.Create(_allocator, player));
			entity.Add(Orbit.Create(_allocator, orbitRadius, orbitSpeed, orbitOffset));
			entity.Add(RelativeTransform.Create(_allocator, orbitedEntity));

			return entity;
		}

		/// <summary>
		///     Creates a bullet entity.
		/// </summary>
		/// <param name="player">The player the planet belongs to.</param>
		/// <param name="position">The initial position of the bullet.</param>
		/// <param name="velocity">The initial velocity of the bullet.</param>
		/// <param name="identity">The network identity of the bullet.</param>
		public Entity CreateBullet(Player player, Vector2 position, Vector2 velocity, Identity identity = default(Identity))
		{
			Assert.ArgumentNotNull(player);

			var entity = _entityCollection.CreateEntity();
			entity.Add(Transform.Create(_allocator, position));
			entity.Add(NetworkSync.Create(_allocator, identity, EntityType.Bullet, MessageType.UpdatePosition));
			entity.Add(Owner.Create(_allocator, player));
			entity.Add(Motion.Create(_allocator, velocity: velocity));
			entity.Add(TimeToLive.Create(_allocator, 2));

			return entity;
		}

		/// <summary>
		///     Creates a phaser entity.
		/// </summary>
		/// <param name="player">The player the planet belongs to.</param>
		/// <param name="parentEntity">The parent entity of the phaser.</param>
		/// <param name="identity">The network identity of the bullet.</param>
		public Entity CreatePhaser(Player player, Entity parentEntity, Identity identity = default(Identity))
		{
			Assert.ArgumentNotNull(player);

			var entity = _entityCollection.CreateEntity();
			entity.Add(Transform.Create(_allocator));
			entity.Add(NetworkSync.Create(_allocator, identity, EntityType.Ray, MessageType.UpdateRay));
			entity.Add(Owner.Create(_allocator, player));
			entity.Add(Ray.Create(_allocator, maxLength: 2000));
			entity.Add(RelativeTransform.Create(_allocator, parentEntity));

			return entity;
		}
	}
}