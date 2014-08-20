namespace Lwar.Gameplay.Entities
{
	using System;
	using Pegasus;
	using Pegasus.Platform;

	/// <summary>
	///     Represents a planet.
	/// </summary>
	public class Planet : Entity<Planet>
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
		/// <param name="clock">The clock that should be used for time measurements.</param>
		public override void Update(Clock clock)
		{
			Rotation += _rotationSpeed * (float)clock.Seconds;
		}

		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the instance should be created for.</param>
		/// <param name="id">The generational identifier of the planet.</param>
		/// <param name="entityTemplate">The template defining the planet's type.</param>
		public static Planet Create(GameSession gameSession, Identifier id, EntityTemplate entityTemplate)
		{
			Assert.ArgumentNotNull(gameSession);
			Assert.ArgumentNotNull(entityTemplate);

			var planet = gameSession.Allocate<Planet>();
			planet.Identifier = id;
			planet._rotationSpeed = Random.Next(30, 50) / 200.0f;
			planet.Template = entityTemplate;
			return planet;
		}
	}
}