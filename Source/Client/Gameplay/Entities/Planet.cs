using System;

namespace Lwar.Client.Gameplay.Entities
{
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Logging;

	/// <summary>
	///   Represents a planet.
	/// </summary>
	public partial class Planet : Entity<Planet>
	{
		/// <summary>
		///   A cached random number generator.
		/// </summary>
		private static readonly Random Random = new Random();

		/// <summary>
		///   The rotation speed of the planet.
		/// </summary>
		private float _rotationSpeed;

		/// <summary>
		///   Updates the entity's internal state.
		/// </summary>
		/// <param name="clock">The clock that should be used for time measurements.</param>
		public override void Update(Clock clock)
		{
			Rotation += _rotationSpeed * (float)clock.Seconds;
		}

		/// <summary>
		///   Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="message">The update message that should be processed.</param>
		public override void RemoteUpdate(ref Message message)
		{
			Assert.That(message.Type == MessageType.Update, "Unsupported update type.");
			Position = message.Update.Position;
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="id">The generational identifier of the planet.</param>
		public static Planet Create(Identifier id)
		{
			var rotationSpeed = Random.Next(30, 50) / 200.0f;
			rotationSpeed *= (Random.Next() % 2 == 1 ? 1 : -1);

			var planet = GetInstance();
			planet.Id = id;
			planet._rotationSpeed = rotationSpeed;
			return planet;
		}
	}
}