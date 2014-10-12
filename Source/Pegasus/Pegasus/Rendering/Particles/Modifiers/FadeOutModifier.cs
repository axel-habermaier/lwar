namespace Pegasus.Rendering.Particles.Modifiers
{
	using System;

	/// <summary>
	///     Fades out the particles based on their remaining life time.
	/// </summary>
	public sealed class FadeOutModifier : Modifier
	{
		/// <summary>
		///     Executes the modifier, updating the given number of particles contained in the particles collection.
		/// </summary>
		/// <param name="particles">The particles that should be updated.</param>
		/// <param name="count">The number of particles that should be updated.</param>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		public override unsafe void Execute(ParticleCollection particles, int count, float elapsedSeconds)
		{
			var alpha = particles.Colors + 3;
			var age = particles.Age;

			while (count -- > 0)
			{
				*alpha = (byte)(*age * 255);

				alpha += 4;
				age += 1;
			}
		}
	}
}