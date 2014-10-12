namespace Lwar.Gameplay.Actors
{
	using System;
	using Pegasus;
	using Pegasus.Math;
	using Pegasus.Platform;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering.Particles;

	/// <summary>
	///     Represents an explosion
	/// </summary>
	public class Explosion : Actor<Explosion>
	{
		/// <summary>
		///     The position of the explosion;
		/// </summary>
		private Vector2 _position;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public Explosion()
		{
			ParticleEffect = new ParticleEffect();
		}

		/// <summary>
		///     Gets the particle effect used to draw the explosion.
		/// </summary>
		public ParticleEffect ParticleEffect { get; private set; }

		/// <summary>
		///     Updates the actor's internal state.
		/// </summary>
		/// <param name="clock">The clock that should be used for time measurements.</param>
		public override void Update(Clock clock)
		{
			ParticleEffect.Update((float)clock.Seconds);

			if (ParticleEffect.IsCompleted)
				GameSession.Actors.Remove(this);
		}

		/// <summary>
		///     Invoked when the actor is added to the game session.
		/// </summary>
		protected override void OnAdded()
		{
			Transform.Position2D = _position;
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			ParticleEffect.SafeDispose();
		}

		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the instance should be created for.</param>
		/// <param name="position">The position of the explosion.</param>
		public static Explosion Create(GameSession gameSession, Vector2 position)
		{
			Assert.ArgumentNotNull(gameSession);

			var explosion = gameSession.Allocate<Explosion>();
			explosion._position = position;
			return explosion;
		}
	}
}