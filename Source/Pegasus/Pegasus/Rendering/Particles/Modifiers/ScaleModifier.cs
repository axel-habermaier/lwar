namespace Pegasus.Rendering.Particles.Modifiers
{
	using System;

	/// <summary>
	///     Changes the scale of the particles.
	/// </summary>
	public sealed class ScaleModifier : Modifier
	{
		/// <summary>
		///     The scale delta per second.
		/// </summary>
		public float Delta;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public ScaleModifier()
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="delta">The scale delta per second.</param>
		public ScaleModifier(float delta)
		{
			Delta = delta;
		}

		/// <summary>
		///     Executes the modifier, updating the given number of particles contained in the particles collection.
		/// </summary>
		/// <param name="particles">The particles that should be updated.</param>
		/// <param name="count">The number of particles that should be updated.</param>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		public override unsafe void Execute(ParticleCollection particles, int count, float elapsedSeconds)
		{
			var scales = particles.Scales;

			if (Delta < 0)
			{
				while (count-- > 0)
				{
					var scale = *scales + Delta * elapsedSeconds;
					*scales = scale < 0 ? 0 : scale;
					++scales;
				}
			}
			else
			{
				while (count-- > 0)
				{
					*scales += Delta * elapsedSeconds;
					++scales;
				}
			}
		}
	}
}