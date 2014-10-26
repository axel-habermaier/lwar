namespace Lwar.Gameplay.Server.Behaviors
{
	using System;
	using Components;
	using Pegasus.Entities;

	/// <summary>
	///     Moves entities with motion components.
	/// </summary>
	public class MotionBehavior : EntityBehavior<Transform, Motion>
	{
		/// <summary>
		///     The number of seconds that have elapsed since the last update.
		/// </summary>
		private float _elapsedSeconds;

		/// <summary>
		///     Updates the positions and velocities.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		public void Update(float elapsedSeconds)
		{
			_elapsedSeconds = elapsedSeconds;
			Process();
		}

		/// <summary>
		///     Processes the entities affected by the behavior.
		/// </summary>
		/// <param name="entities">The entities affected by the behavior.</param>
		/// <param name="transforms">The transform components of the affected entities.</param>
		/// <param name="motions">The motions components of the affected entities.</param>
		/// <param name="count">The number of entities that should be processed.</param>
		/// <remarks>
		///     All arrays have the same length. If a component is optional for an entity and the component is missing, a null
		///     value is placed in the array.
		/// </remarks>
		protected override void Process(Entity[] entities, Transform[] transforms, Motion[] motions, int count)
		{
			for (var i = 0; i < count; ++i)
			{
				var velocity = motions[i].Velocity;
				var maxSpeed = motions[i].MaxSpeed;

				velocity += motions[i].Acceleration * _elapsedSeconds;
				velocity = velocity.Length > maxSpeed ? velocity.Normalize() * maxSpeed : velocity;

				transforms[i].Position += velocity * _elapsedSeconds;
				motions[i].Velocity = velocity;
			}
		}
	}
}