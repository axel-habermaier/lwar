namespace Pegasus.Rendering.Particles.Emitteres
{
	using System;

	/// <summary>
	///     Emits all particles at the origin.
	/// </summary>
	public sealed class PointEmitter : Emitter
	{
		/// <summary>
		///     Initializes the position and velocity of the given number of newly emitted particles.
		/// </summary>
		/// <param name="positions">The positions of the particles.</param>
		/// <param name="velocities">The velocities of the particles.</param>
		/// <param name="count">The number of particles that should be initialized.</param>
		protected override unsafe void InitializeParticles(float* positions, float* velocities, int count)
		{
			while (count-- > 0)
			{
				positions[0] = 0;
				positions[1] = 0;
				positions[2] = 0;

				var speed = RandomValues.NextSingle(InitialSpeed.LowerBound, InitialSpeed.UpperBound);
				RandomValues.NextUnitVector(velocities);

				velocities[0] *= speed;
				velocities[1] *= speed;
				velocities[2] *= speed;

				positions += 3;
				velocities += 3;
			}
		}
	}
}