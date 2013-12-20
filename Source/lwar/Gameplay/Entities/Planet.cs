namespace Lwar.Gameplay.Entities
{
	using System;
	using Pegasus;
	using Pegasus.Math;
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
		///     Invoked when the entity is added to the game session.
		/// </summary>
		protected override void OnAdded()
		{
			Transform.Rotation = new Vector3(
				MathUtils.DegToRad(Random.Next(0, 360)),
				0.0f,
				MathUtils.DegToRad(Random.Next(0, 360)));
		}

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
		/// <param name="id">The generational identifier of the planet.</param>
		/// <param name="entityTemplate">The template defining the planet's type.</param>
		public static Planet Create(Identifier id, EntityTemplate entityTemplate)
		{
			Assert.ArgumentNotNull(entityTemplate);

			var rotationSpeed = Random.Next(30, 50) / 200.0f;
			rotationSpeed *= (Random.Next() % 2 == 1 ? 1 : -1);

			var planet = GetInstance();
			planet.Identifier = id;
			planet._rotationSpeed = rotationSpeed;
			planet.Template = entityTemplate;
			return planet;
		}
	}
}