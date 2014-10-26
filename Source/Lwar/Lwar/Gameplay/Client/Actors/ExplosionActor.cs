namespace Lwar.Gameplay.Client.Actors
{
	using System;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering.Particles;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents an explosion
	/// </summary>
	public class ExplosionActor : Actor<ExplosionActor>
	{
		/// <summary>
		///     The position of the explosion;
		/// </summary>
		private Vector2 _position;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public ExplosionActor()
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
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		public override void Update(float elapsedSeconds)
		{
			ParticleEffect.Update(elapsedSeconds);

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
		public static ExplosionActor Create(ClientGameSession gameSession, Vector2 position)
		{
			Assert.ArgumentNotNull(gameSession);

			var explosion = gameSession.Allocate<ExplosionActor>();
			explosion._position = position;

			gameSession.Actors.Add(explosion);
			return explosion;
		}
	}
}