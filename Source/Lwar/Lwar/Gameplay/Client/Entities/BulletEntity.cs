namespace Lwar.Gameplay.Client.Entities
{
	using System;
	using Network;
	using Pegasus.Entities;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a bullet.
	/// </summary>
	public class BulletEntity : Entity<BulletEntity>
	{
		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the instance should be created for.</param>
		/// <param name="id">The generational identity of the bullet.</param>
		public static BulletEntity Create(ClientGameSession gameSession, Identity id)
		{
			Assert.ArgumentNotNull(gameSession);

			var bullet = gameSession.Allocate<BulletEntity>();
			bullet.Identifier = id;
			bullet.Template = EntityTemplates.Bullet;

			gameSession.Entities.Add(bullet);
			return bullet;
		}
	}
}