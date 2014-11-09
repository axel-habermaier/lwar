namespace Lwar.Gameplay.Server.Entities
{
	using System;
	using Behaviors;
	using Network;
	using Network.Messages;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;
	using Pegasus.Scene;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a bullet.
	/// </summary>
	public class Bullet : Entity
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Bullet()
		{
			ConstructorCache.Register(() => new Bullet());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private Bullet()
		{
			UpdateMessageType = MessageType.UpdatePosition;
			NetworkType = EntityType.Bullet;
		}

		/// <summary>
		///     Updates the server-side state of the entity.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		public override void ServerUpdate(float elapsedSeconds)
		{
			Position2D += Velocity * elapsedSeconds;
		}

		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the entity belongs to.</param>
		/// <param name="player">The player the bullet belongs to.</param>
		/// <param name="initialPosition">The initial position of the bullet.</param>
		/// <param name="initialVelocity">The initial velocity of the bullet.</param>
		public static Bullet Create(GameSession gameSession, Player player, Vector2 initialPosition, Vector2 initialVelocity)
		{
			Assert.ArgumentNotNull(gameSession);
			Assert.ArgumentNotNull(player);

			var bullet = gameSession.Allocate<Bullet>();
			bullet.GameSession = gameSession;
			bullet.Player = player;
			bullet.Velocity = initialVelocity;
			bullet.Position2D = initialPosition;
			bullet.AddBehavior(TimeToLiveBehavior.Create(gameSession.Allocator, seconds: 3));
			bullet.AddBehavior(BoundaryBehavior.Create(gameSession.Allocator));
			return bullet;
		}
	}
}