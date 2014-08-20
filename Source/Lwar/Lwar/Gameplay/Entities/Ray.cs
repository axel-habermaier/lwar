namespace Lwar.Gameplay.Entities
{
	using System;
	using Pegasus;
	using Pegasus.Math;

	/// <summary>
	///     Represents a ray.
	/// </summary>
	public class Ray : Entity<Ray>
	{
		/// <summary>
		///     Gets the length of the ray.
		/// </summary>
		public float Length { get; private set; }

		/// <summary>
		///     Gets the current target entity or null if there is none.
		/// </summary>
		public IEntity Target { get; private set; }

		/// <summary>
		///     Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="origin">The updated ray origin.</param>
		/// <param name="direction">The updated ray direction.</param>
		/// <param name="length">The updated ray length.</param>
		/// <param name="target">The current ray target or null if no target is hit.</param>
		public override void RemoteUpdate(Vector2 origin, float direction, float length, IEntity target)
		{
			Position = origin;
			Rotation = MathUtils.DegToRad(direction);
			Length = length;
			Target = target;
		}

		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the instance should be created for.</param>
		/// <param name="id">The generational identifier of the ray.</param>
		public static Ray Create(GameSession gameSession, Identifier id)
		{
			Assert.ArgumentNotNull(gameSession);

			var ray = gameSession.Allocate<Ray>();
			ray.Identifier = id;
			ray.Template = EntityTemplates.Ray;
			return ray;
		}
	}
}