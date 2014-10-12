namespace Pegasus.Rendering.Particles.Modifiers
{
	using System;

	/// <summary>
	///     A base class for modifiers that affect and update particle properties.
	/// </summary>
	public abstract class Modifier
	{
		/// <summary>
		///     Executes the modifier, updating the given number of particles contained in the particles collection.
		/// </summary>
		/// <param name="particles">The particles that should be updated.</param>
		/// <param name="count">The number of particles that should be updated.</param>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		public abstract void Execute(ref ParticleCollection particles, int count, float elapsedSeconds);

		/// <summary>
		///     Resets the internal state of the modifier.
		/// </summary>
		public virtual void ResetState()
		{
		}
	}
}