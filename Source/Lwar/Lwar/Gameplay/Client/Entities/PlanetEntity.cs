namespace Lwar.Gameplay.Client.Entities
{
	using System;
	using Network;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a planet.
	/// </summary>
	public class PlanetEntity : Entity<PlanetEntity>
	{
		/// <summary>
		///     A cached random number generator.
		/// </summary>
		private static readonly Random Random = new Random();

		/// <summary>
		///     The rotation speed of the planet.
		/// </summary>
		private float _rotationSpeed;

		/// <summary>
		///     Updates the entity's internal state.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		public override void Update(float elapsedSeconds)
		{
			Orientation += _rotationSpeed * elapsedSeconds;
		}

		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the instance should be created for.</param>
		/// <param name="id">The generational identity of the planet.</param>
		/// <param name="entityTemplate">The template defining the planet's type.</param>
		public static PlanetEntity Create(ClientGameSession gameSession, NetworkIdentity id, EntityTemplate entityTemplate)
		{
			Assert.ArgumentNotNull(gameSession);
			Assert.ArgumentNotNull(entityTemplate);

			var planet = gameSession.Allocate<PlanetEntity>();
			planet.Identifier = id;
			planet._rotationSpeed = Random.Next(30, 50) / 200.0f;
			planet.Template = entityTemplate;

			gameSession.Entities.Add(planet);
			return planet;
		}
	}
}