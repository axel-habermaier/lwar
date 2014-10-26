namespace Lwar.Gameplay.Client.Entities
{
	using System;
	using Pegasus.Entities;
	using Pegasus.Math;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a shockwave.
	/// </summary>
	public class ShockwaveEntity : Entity<ShockwaveEntity>, ICircleEntity
	{
		/// <summary>
		///     Gets the current radius of the shockwave.
		/// </summary>
		public float Radius { get; private set; }

		/// <summary>
		///     Applies the remote circle update to the entity.
		/// </summary>
		/// <param name="center">The updated circle center.</param>
		/// <param name="radius">The updated circle radius.</param>
		public void RemoteCircleUpdate(Vector2 center, float radius)
		{
			Position = center;
			Radius = radius;
		}

		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the instance should be created for.</param>
		/// <param name="id">The generational identity of the shockwave.</param>
		public static ShockwaveEntity Create(ClientGameSession gameSession, Identity id)
		{
			Assert.ArgumentNotNull(gameSession);

			var shockwave = gameSession.Allocate<ShockwaveEntity>();
			shockwave.Identifier = id;

			gameSession.Entities.Add(shockwave);
			return shockwave;
		}
	}
}