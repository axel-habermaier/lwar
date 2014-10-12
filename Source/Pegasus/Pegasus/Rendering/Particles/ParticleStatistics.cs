namespace Pegasus.Rendering.Particles
{
	using System;
	using Framework.UserInterface.ViewModels;

	/// <summary>
	///     Provides performance statistics about the particle system.
	/// </summary>
	internal static class ParticleStatistics
	{
		/// <summary>
		///     Gets or sets the time in milliseconds required to update all active particles.
		/// </summary>
		public static double UpdateTime { get; set; }

		/// <summary>
		///     Gets or sets the time in milliseconds required to draw all active particles.
		/// </summary>
		public static double DrawTime { get; set; }

		/// <summary>
		///     Gets or sets the number of active particles.
		/// </summary>
		public static int ParticleCount { get; set; }

		/// <summary>
		///     Updates the debug overlay with the current particle statistics and resets the statistics afterwards.
		/// </summary>
		/// <param name="debugOverlay">The debug overlay that should be updated with the current particle statistics.</param>
		internal static void UpdateDebugOverlay(DebugOverlayViewModel debugOverlay)
		{
			Assert.ArgumentNotNull(debugOverlay);

			debugOverlay.ParticleCount = ParticleCount;
			debugOverlay.ParticleUpdateTime = UpdateTime;
			debugOverlay.ParticleRenderTime = DrawTime;

			UpdateTime = 0;
			DrawTime = 0;
			ParticleCount = 0;
		}
	}
}