namespace Lwar.Gameplay.Client.Entities
{
	using System;
	using Network;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a sun.
	/// </summary>
	internal class SunEntity : Entity<SunEntity>
	{
		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the instance should be created for.</param>
		/// <param name="id">The generational identity of the sun.</param>
		public static SunEntity Create(ClientGameSession gameSession, NetworkIdentity id)
		{
			Assert.ArgumentNotNull(gameSession);

			var sun = gameSession.Allocate<SunEntity>();
			sun.Identifier = id;
			sun.Template = EntityTemplates.Sun;

			gameSession.Entities.Add(sun);
			return sun;
		}
	}
}