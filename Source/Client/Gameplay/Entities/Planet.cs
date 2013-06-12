using System;

namespace Lwar.Client.Gameplay.Entities
{
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;

	/// <summary>
	///   Represents a planet.
	/// </summary>
	public class Planet : Entity<Planet>
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
		///   Invoked when the entity is added to the game session.
		/// </summary>
		protected override void OnAdded()
		{
			Transform.Rotation = new Vector3(
				MathUtils.DegToRad(Random.Next(0, 360)),
				0.0f,
				MathUtils.DegToRad(Random.Next(0, 360)));
		}

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
			Assert.That(message.Type == MessageType.UpdatePosition, "Unsupported update type.");
			Position = message.UpdatePosition.Position;
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="id">The generational identifier of the planet.</param>
		/// <param name="template">The template defining the planet's type.</param>
		public static Planet Create(Identifier id, Template template)
		{
			Assert.ArgumentNotNull(template);

			var rotationSpeed = Random.Next(30, 50) / 200.0f;
			rotationSpeed *= (Random.Next() % 2 == 1 ? 1 : -1);

			var planet = GetInstance();
			planet.Id = id;
			planet._rotationSpeed = rotationSpeed;
			planet.Template = template;
			return planet;
		}
	}
}