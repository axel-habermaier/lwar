namespace Lwar.Gameplay.Server.Behaviors
{
	using System;
	using Components;
	using Pegasus.Entities;
	using Pegasus.Math;

	/// <summary>
	///     Applies the effects of the current player input to an entity.
	/// </summary>
	public class PlayerInputBehavior : EntityBehavior<PlayerInput, Transform, Rotation, Propulsion>
	{
		/// <summary>
		///     Applies the user input.
		/// </summary>
		public void ApplyInput()
		{
			Process();
		}

		/// <summary>
		///     Processes the entities affected by the behavior.
		/// </summary>
		/// <param name="entities">The entities affected by the behavior.</param>
		/// <param name="inputs">The input components of the affected entities.</param>
		/// <param name="transforms">The transform components of the affected entities.</param>
		/// <param name="rotations">The rotation components of the affected entities.</param>
		/// <param name="propulsions">The propulsion components of the affected entities.</param>
		/// <param name="count">The number of entities that should be processed.</param>
		/// <remarks>
		///     All arrays have the same length. If a component is optional for an entity and the component is missing, a null
		///     value is placed in the array.
		/// </remarks>
		protected override void Process(Entity[] entities, PlayerInput[] inputs, Transform[] transforms,
										Rotation[] rotations, Propulsion[] propulsions, int count)
		{
			for (var i = 0; i < count; ++i)
			{
				// Compute the angular velocity, considering the target orientation.
				// We always use the shortest path to rotate towards the target.
				var targetOrientation = Vector2.ToAngle(inputs[i].Target);
				var orientationDelta = targetOrientation - transforms[i].Orientation;
				orientationDelta = MathUtils.Atan2(MathUtils.Sin(orientationDelta), MathUtils.Cos(orientationDelta));
				rotations[i].Velocity = rotations[i].MaxSpeed * orientationDelta;

				// Update the acceleration of the entity
				var acceleration = Vector2.Zero;

				if (inputs[i].Forward)
					acceleration += new Vector2(1, 0);
				if (inputs[i].Backward)
					acceleration += new Vector2(-1, 0);
				if (inputs[i].StrafeLeft)
					acceleration += new Vector2(0, -1);
				if (inputs[i].StrafeRight)
					acceleration += new Vector2(0, 1);

				propulsions[i].Acceleration = Vector2.Rotate(acceleration, transforms[i].Orientation).Normalize();
				propulsions[i].AfterBurnerEnabled = inputs[i].AfterBurner;
			}
		}
	}
}