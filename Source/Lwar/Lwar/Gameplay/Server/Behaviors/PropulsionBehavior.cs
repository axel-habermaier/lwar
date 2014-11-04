namespace Lwar.Gameplay.Server.Behaviors
{
	using System;
	using Components;
	using Pegasus.Entities;
	using Pegasus.Math;
	using Pegasus.Platform.Logging;

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
				var isAlreadyEnabled = propulsions[i].AfterBurnerState == AfterBurnerState.Active;
				var canEnable = propulsions[i].RemainingEnergy > propulsions[i].MinRequiredEnergy;
				var afterBurnerEnabled = propulsions[i].AfterBurnerEnabled && propulsions[i].RemainingEnergy > 0 && (isAlreadyEnabled || canEnable);
				float maxSpeed;

				if (afterBurnerEnabled)
				{
					propulsions[i].AfterBurnerState = AfterBurnerState.Active;
					propulsions[i].RemainingEnergy -= propulsions[i].DepleteSpeed * _elapsedSeconds;
					maxSpeed = propulsions[i].MaxAfterBurnerSpeed;
				}
				else
				{
					switch (propulsions[i].AfterBurnerState)
					{
						case AfterBurnerState.FullyCharged:
							break;
						case AfterBurnerState.Recharging:
							propulsions[i].RemainingEnergy += propulsions[i].RechargeSpeed * _elapsedSeconds;
							if (propulsions[i].RemainingEnergy >= propulsions[i].MaxEnergy)
								propulsions[i].AfterBurnerState = AfterBurnerState.FullyCharged;
							break;
						case AfterBurnerState.Active:
							propulsions[i].AfterBurnerState = AfterBurnerState.WaitingForRecharging;
							propulsions[i].RemainingRechargeDelay = propulsions[i].RechargeDelay;
							break;
						case AfterBurnerState.WaitingForRecharging:
							propulsions[i].RemainingRechargeDelay -= _elapsedSeconds;
							if (propulsions[i].RemainingRechargeDelay < 0)
								propulsions[i].AfterBurnerState = AfterBurnerState.Recharging;
							break;
						default:
							throw new InvalidOperationException("Unknown after burner state.");
					}

					maxSpeed = propulsions[i].MaxSpeed;
				}

				var acceleration = propulsions[i].Acceleration;
				var velocity = propulsions[i].Velocity;
				if (velocity.Length > maxSpeed)
					acceleration -= velocity.Normalize() * 1.5f;

				acceleration *= propulsions[i].MaxAcceleration * _elapsedSeconds;
				propulsions[i].Velocity += acceleration;
				motions[i].Velocity += acceleration;

				propulsions[i].RemainingEnergy = MathUtils.Clamp(propulsions[i].RemainingEnergy, 0, propulsions[i].MaxEnergy);
				Log.Info("{0}", propulsions[i].RemainingEnergy );
			}
		}
	}
}