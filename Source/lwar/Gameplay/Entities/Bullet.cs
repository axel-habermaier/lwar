using System;

namespace Lwar.Gameplay.Entities
{
	/// <summary>
	///   Represents a bullet.
	/// </summary>
	public class Bullet : Entity<Bullet>
	{
		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="id">The generational identifier of the bullet.</param>
		public static Bullet Create(Identifier id)
		{
			var bullet = GetInstance();
			bullet.Identifier = id;
			bullet.Template = Templates.Bullet;
			return bullet;
		}
	}
}