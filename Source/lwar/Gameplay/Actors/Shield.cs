using System;

namespace Lwar.Gameplay.Actors
{
	using Entities;
	using Pegasus;
	using Pegasus.Math;
	using Pegasus.Platform;

	/// <summary>
	///   Represents a shield that protects a ship. The shield is only visible if it has just been hit. Invisible shields are
	///   removed from the game session.
	/// </summary>
	public class Shield : Actor<Shield>
	{
		/// <summary>
		///   The time (in seconds) it takes for the shield to fully fade out.
		/// </summary>
		private const float FadeOutTime = 1.0f;

		/// <summary>
		///   Track the previous ship position so that we can correctly 'drag along' the impact position.
		/// </summary>
		private Vector2 _previousShipPosition;

		/// <summary>
		///   The time (in seconds) for which the shield remains visible (but slowly fading out).
		/// </summary>
		private float _remainingTime;

		/// <summary>
		///   Gets the remaining time to live in the range [0,1], starting with 1.
		/// </summary>
		public float TimeToLive
		{
			get { return _remainingTime / FadeOutTime; }
		}

		/// <summary>
		///   Gets the ship that the shield belongs to.
		/// </summary>
		public Ship Ship { get; private set; }

		/// <summary>
		///   Gets the position where the shield has been hit.
		/// </summary>
		public Vector2 ImpactPosition { get; private set; }

		/// <summary>
		///   Updates the actor's internal state.
		/// </summary>
		/// <param name="clock">The clock that should be used for time measurements.</param>
		public override void Update(Clock clock)
		{
			_remainingTime -= (float)clock.Seconds;

			if (_remainingTime < 0.0f)
				GameSession.Actors.Remove(this);

			var offset = Ship.Position - _previousShipPosition;
			_previousShipPosition = Ship.Position;
			ImpactPosition += offset;
		}

		/// <summary>
		///   Invoked when the actor is added to the game session.
		/// </summary>
		protected override void OnAdded()
		{
			_previousShipPosition = Ship.Position;
			Transform.AttachTo(Ship.Transform);
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="ship">The ship that the shield belongs to.</param>
		/// <param name="impactPosition">The position where the shield has been hit.</param>
		public static Shield Create(Ship ship, Vector2 impactPosition)
		{
			Assert.ArgumentNotNull(ship);

			var shield = GetInstance();
			shield.Ship = ship;
			shield._remainingTime = FadeOutTime;
			shield.ImpactPosition = impactPosition;
			return shield;
		}
	}
}