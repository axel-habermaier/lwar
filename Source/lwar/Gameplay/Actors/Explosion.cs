namespace Lwar.Gameplay.Actors
{
	using System;
	using Pegasus.Math;
	using Pegasus.Platform;

	/// <summary>
	///   Represents an explosion
	/// </summary>
	public class Explosion : Actor<Explosion>
	{
		/// <summary>
		///   The time (in seconds) it takes for the explision effect to play.
		/// </summary>
		private const float PlayTime = 1.0f;

		/// <summary>
		///   The position of the explosion;
		/// </summary>
		private Vector2 _position;

		/// <summary>
		///   The time (in seconds) for which the explosion continues playing.
		/// </summary>
		private float _remainingTime;

		/// <summary>
		///   Gets the remaining time to live in the range [0,1], starting with 1.
		/// </summary>
		public float TimeToLive
		{
			get { return _remainingTime / PlayTime; }
		}

		/// <summary>
		///   Updates the actor's internal state.
		/// </summary>
		/// <param name="clock">The clock that should be used for time measurements.</param>
		public override void Update(Clock clock)
		{
			_remainingTime -= (float)clock.Seconds;

			if (_remainingTime < 0.0f)
				GameSession.Actors.Remove(this);
		}

		/// <summary>
		///   Invoked when the actor is added to the game session.
		/// </summary>
		protected override void OnAdded()
		{
			Transform.Position2D = _position;
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="position">The position of the explosion.</param>
		public static Explosion Create(Vector2 position)
		{
			var explosion = GetInstance();
			explosion._position = position;
			explosion._remainingTime = PlayTime;
			return explosion;
		}
	}
}