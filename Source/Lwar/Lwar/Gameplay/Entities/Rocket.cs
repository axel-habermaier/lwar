namespace Lwar.Gameplay.Entities
{
	using System;
	using Pegasus;

	/// <summary>
	///     Represents a rocket.
	/// </summary>
	public class Rocket : Entity<Rocket>
	{
		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the instance should be created for.</param>
		/// <param name="id">The generational identifier of the rocket.</param>
		public static Rocket Create(GameSession gameSession, Identifier id)
		{
			Assert.ArgumentNotNull(gameSession);

			var rocket = gameSession.Allocate<Rocket>();
			rocket.Identifier = id;
			rocket.Template = EntityTemplates.Rocket;
			return rocket;
		}
	}
}