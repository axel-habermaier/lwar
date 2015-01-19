namespace Lwar.Gameplay.Server.Entities
{
	using System;
	using Network;
	using Network.Messages;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a phaser.
	/// </summary>
	internal class Phaser : Entity
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Phaser()
		{
			ConstructorCache.Register(() => new Phaser());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private Phaser()
		{
			UpdateMessageType = MessageType.UpdateRay;
			NetworkType = EntityType.Ray;
		}

		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the entity belongs to.</param>
		/// <param name="player">The player the phaser belongs to.</param>
		public static Phaser Create(GameSession gameSession, Player player)
		{
			Assert.ArgumentNotNull(gameSession);
			Assert.ArgumentNotNull(player);

			var phaser = gameSession.Allocate<Phaser>();
			phaser.GameSession = gameSession;
			phaser.Player = player;
			return phaser;
		}
	}
}