namespace Pegasus.Rendering.Particles.Modifiers
{
	using System;
	using UserInterface;
	using Utilities;

	/// <summary>
	///     Represents a collection of particle modifiers.
	/// </summary>
	public sealed class ModifierCollection : CustomCollection<Modifier>
	{
		/// <summary>
		///     Executes the modifiers contained in the collection, updating the given number of particles contained in the particles
		///     collection.
		/// </summary>
		/// <param name="particles">The particles that should be updated.</param>
		/// <param name="count">The number of particles that should be updated.</param>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		internal void Execute(ParticleCollection particles, int count, float elapsedSeconds)
		{
			foreach (var modifier in this)
				modifier.Execute(particles, count, elapsedSeconds);
		}

		/// <summary>
		///     Resets the internal state of all modifiers contained in the collection.
		/// </summary>
		internal void ResetState()
		{
			foreach (var modifier in this)
				modifier.ResetState();
		}
	}
}