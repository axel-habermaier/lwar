namespace Lwar.Gameplay.Entities
{
	using System;

	/// <summary>
	///     Represents a phaser.
	/// </summary>
	public class Phaser : Entity<Phaser>
	{
		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="id">The generational identifier of the phaser.</param>
		public static Phaser Create(Identifier id)
		{
			var phaser = GetInstance();
			phaser.Identifier = id;
			return phaser;
		}
	}
}