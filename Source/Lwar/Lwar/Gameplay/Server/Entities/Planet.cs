namespace Lwar.Gameplay.Server.Entities
{
	using System;
	using Behaviors;
	using Network;
	using Network.Messages;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a planet that orbits a sun or another planet.
	/// </summary>
	public class Planet : Entity
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Planet()
		{
			ConstructorCache.Register(() => new Planet());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private Planet()
		{
			UpdateMessageType = MessageType.UpdatePosition;
		}

		/// <summary>
		///     Creates a planet entity and attaches it to the given orbited entity.
		/// </summary>
		/// <param name="gameSession">The game session the entity belongs to.</param>
		/// <param name="orbitedEntity">The entity that is orbitted.</param>
		/// <param name="planetType">The type of the planet.</param>
		/// <param name="orbitRadius">The radius of the planet's orbit.</param>
		/// <param name="orbitSpeed">
		///     The orbital speed of the planet. The sign of the speed determines the direction of the orbital movement.
		/// </param>
		/// <param name="orbitOffset">An offset to the position on the orbital trajectory.</param>
		public static Planet Create(GameSession gameSession, Entity orbitedEntity, EntityType planetType,
									float orbitRadius, float orbitSpeed, float orbitOffset)
		{
			Assert.ArgumentNotNull(gameSession);
			Assert.ArgumentNotNull(orbitedEntity);
			Assert.ArgumentSatisfies(planetType == EntityType.Earth || planetType == EntityType.Mars ||
									 planetType == EntityType.Jupiter || planetType == EntityType.Moon, "Invalid planet type.");

			var planet = gameSession.Allocate<Planet>();
			planet.GameSession = gameSession;
			planet.NetworkType = planetType;
			planet.Player = gameSession.Players.ServerPlayer;
			planet.AddBehavior(OrbitBehavior.Create(gameSession.Allocator, orbitRadius, orbitSpeed, orbitOffset));

			orbitedEntity.AttachChild(planet);
			return planet;
		}
	}
}