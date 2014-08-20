namespace Lwar.Gameplay.Entities
{
	using System;
	using Pegasus;

	/// <summary>
	///     Represents a sun.
	/// </summary>
	public class Sun : Entity<Sun>
	{
		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the instance should be created for.</param>
		/// <param name="id">The generational identifier of the sun.</param>
		public static Sun Create(GameSession gameSession, Identifier id)
		{
			Assert.ArgumentNotNull(gameSession);

			var sun = gameSession.Allocate<Sun>();
			sun.Identifier = id;
			sun.Template = EntityTemplates.Sun;
			return sun;
		}
	}
}