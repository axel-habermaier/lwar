namespace Lwar.Gameplay.Client.Entities
{
	using System;
	using Network;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a rocket.
	/// </summary>
	internal class RocketEntity : Entity<RocketEntity>
	{
		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the instance should be created for.</param>
		/// <param name="id">The generational identity of the rocket.</param>
		public static RocketEntity Create(ClientGameSession gameSession, NetworkIdentity id)
		{
			Assert.ArgumentNotNull(gameSession);

			var rocket = gameSession.Allocate<RocketEntity>();
			rocket.Identifier = id;
			rocket.Template = EntityTemplates.Rocket;

			gameSession.Entities.Add(rocket);
			return rocket;
		}
	}
}