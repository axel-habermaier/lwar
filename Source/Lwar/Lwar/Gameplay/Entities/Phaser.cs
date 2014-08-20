namespace Lwar.Gameplay.Entities
{
	using System;
	using Pegasus;

	/// <summary>
	///     Represents a phaser.
	/// </summary>
	public class Phaser : Entity<Phaser>
	{
		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the instance should be created for.</param>
		/// <param name="id">The generational identifier of the phaser.</param>
		public static Phaser Create(GameSession gameSession, Identifier id)
		{
			Assert.ArgumentNotNull(gameSession);

			var phaser = gameSession.Allocate<Phaser>();
			phaser.Identifier = id;
			return phaser;
		}
	}
}