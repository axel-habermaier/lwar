namespace Lwar.Gameplay.Entities
{
	using System;
	using Actors;
	using Pegasus;
	using Pegasus.Math;
	using Pegasus.Platform.Logging;

	/// <summary>
	///     Represents a ray.
	/// </summary>
	public class Ray : Entity<Ray>
	{
		/// <summary>
		///     The shield effect that is shown when the ray hits a ship.
		/// </summary>
		private Shield _shield;

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
		public override void RemoteRayUpdate(Vector2 origin, float direction, float length, IEntity target)
		{
			Position = origin;
			Rotation = MathUtils.DegToRad(direction);
			Length = length;
			
			var ship = target as Ship;
			if (ship == null)
				return;

			var angle = MathUtils.DegToRad(direction);
			var impactPosition = origin + new Vector2(MathUtils.Cos(angle), MathUtils.Sin(angle)) * length;

			if (ship != Target || _shield == null)
			{
				Target = target;
				_shield = Shield.Create(GameSession, ship, impactPosition);
				GameSession.Actors.Add(_shield);
			}
			else
				_shield.Hit(impactPosition);
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
			ray._shield = null;
			return ray;
		}
	}
}