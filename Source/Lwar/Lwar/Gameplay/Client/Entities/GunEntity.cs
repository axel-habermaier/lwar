namespace Lwar.Gameplay.Client.Entities
{
	using System;
	using Network;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a gun.
	/// </summary>
	public class GunEntity : Entity<GunEntity>
	{
		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the instance should be created for.</param>
		/// <param name="id">The generational identity of the phaser.</param>
		public static GunEntity Create(ClientGameSession gameSession, NetworkIdentity id)
		{
			Assert.ArgumentNotNull(gameSession);

			var gun = gameSession.Allocate<GunEntity>();
			gun.Identifier = id;

			gameSession.Entities.Add(gun);
			return gun;
		}
	}
}