namespace Lwar.Gameplay.Entities
{
	using System;
	using Pegasus;

	/// <summary>
	///     Represents a bullet.
	/// </summary>
	public class Bullet : Entity<Bullet>
	{
		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the instance should be created for.</param>
		/// <param name="id">The generational identifier of the bullet.</param>
		public static Bullet Create(GameSession gameSession, Identifier id)
		{
			Assert.ArgumentNotNull(gameSession);

			var bullet = gameSession.Allocate<Bullet>();
			bullet.Identifier = id;
			bullet.Template = EntityTemplates.Bullet;
			return bullet;
		}
	}
}