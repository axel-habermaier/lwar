namespace Lwar.Gameplay.Server.Behaviors
{
	using System;
	using Components;
	using Pegasus.Entities;
	using Pegasus.Math;

	/// <summary>
	///     Simulates propulsion systems affecting the motion of entities.
	/// </summary>
	public class PropulsionBehavior : EntityBehavior<Propulsion, Motion>
	{
		/// <summary>
		///     The number of seconds that have elapsed since the last update.
		/// </summary>
		private float _elapsedSeconds;

		/// <summary>
		///     Updates the positions and velocities.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		public void Simulate(float elapsedSeconds)
		{
			_elapsedSeconds = elapsedSeconds;
			Process();
		}

		/// <summary>
		///     Processes the entities affected by the behavior.
		/// </summary>
		/// <param name="entities">The entities affected by the behavior.</param>
		/// <param name="propulsions">The propulsion components of the affected entities.</param>
		/// <param name="motions">The motion components of the affected entities.</param>
		/// <param name="count">The number of entities that should be processed.</param>
		/// <remarks>
		///     All arrays have the same length. If a component is optional for an entity and the component is missing, a null
		///     value is placed in the array.
		/// </remarks>
		protected override void Process(Entity[] entities, Propulsion[] propulsions, Motion[] motions, int count)
		{
			for (var i = 0; i < count; ++i)
			{
				motions[i].Velocity += propulsions[i].Acceleration * _elapsedSeconds * propulsions[i].MaxAcceleration;

				if (propulsions[i].AfterBurnerEnabled && propulsions[i].RemainingEnergy > 0)
				{
					propulsions[i].RemainingEnergy -= propulsions[i].DepleteSpeed * _elapsedSeconds;

					var velocity = motions[i].Velocity;
					var maxSpeed = propulsions[i].MaxAfterBurnerSpeed;

					motions[i].Velocity = velocity.Length > maxSpeed ? velocity.Normalize() * maxSpeed : velocity;
				}
				else
				{
					propulsions[i].RemainingEnergy += propulsions[i].RechargeSpeed * _elapsedSeconds;

					var velocity = motions[i].Velocity;
					var maxSpeed = propulsions[i].MaxSpeed;

					motions[i].Velocity = velocity.Length > maxSpeed ? velocity.Normalize() * maxSpeed : velocity;
				}

				propulsions[i].RemainingEnergy = MathUtils.Clamp(propulsions[i].RemainingEnergy, 0, propulsions[i].MaxEnergy);
			}
		}
	}
}