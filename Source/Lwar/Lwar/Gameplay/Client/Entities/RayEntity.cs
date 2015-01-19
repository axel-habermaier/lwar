namespace Lwar.Gameplay.Client.Entities
{
	using System;
	using Actors;
	using Network;
	using Pegasus.Math;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a ray.
	/// </summary>
	internal class RayEntity : Entity<RayEntity>, IRayEntity
	{
		/// <summary>
		///     The shield effect that is shown when the ray hits a ship.
		/// </summary>
		private ShieldActor _shield;

		/// <summary>
		///     Gets the length of the ray.
		/// </summary>
		public float Length { get; private set; }

		/// <summary>
		///     Gets the current target entity or null if there is none.
		/// </summary>
		public IEntity Target { get; private set; }

		/// <summary>
		///     Applies the remote ray update to the entity.
		/// </summary>
		/// <param name="origin">The updated ray origin.</param>
		/// <param name="direction">The updated ray direction.</param>
		/// <param name="length">The updated ray length.</param>
		/// <param name="target">The current ray target or null if no target is hit.</param>
		public void RemoteRayUpdate(Vector2 origin, float direction, float length, IEntity target)
		{
			Position = origin;
			Orientation = MathUtils.DegToRad(direction);
			Length = length;

			var ship = target as ShipEntity;
			if (ship == null)
				return;

			var angle = MathUtils.DegToRad(direction);
			var impactPosition = origin + new Vector2(MathUtils.Cos(angle), MathUtils.Sin(angle)) * length;

			if (ship != Target || _shield == null)
			{
				Target = target;
				_shield = ShieldActor.Create(GameSession, ship, impactPosition);
			}
			else
				_shield.Hit(impactPosition);
		}

		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the instance should be created for.</param>
		/// <param name="id">The generational identity of the ray.</param>
		public static RayEntity Create(ClientGameSession gameSession, NetworkIdentity id)
		{
			Assert.ArgumentNotNull(gameSession);

			var ray = gameSession.Allocate<RayEntity>();
			ray.Identifier = id;
			ray.Template = EntityTemplates.Ray;
			ray._shield = null;

			gameSession.Entities.Add(ray);
			return ray;
		}
	}
}