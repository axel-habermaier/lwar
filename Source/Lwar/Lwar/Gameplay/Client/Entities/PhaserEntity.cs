namespace Lwar.Gameplay.Client.Entities
{
	using System;
	using Network;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a phaser.
	/// </summary>
	public class PhaserEntity : Entity<PhaserEntity>
	{
		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the instance should be created for.</param>
		/// <param name="id">The generational identity of the phaser.</param>
		public static PhaserEntity Create(ClientGameSession gameSession, NetworkIdentity id)
		{
			Assert.ArgumentNotNull(gameSession);

			var phaser = gameSession.Allocate<PhaserEntity>();
			phaser.Identifier = id;

			gameSession.Entities.Add(phaser);
			return phaser;
		}
	}
}