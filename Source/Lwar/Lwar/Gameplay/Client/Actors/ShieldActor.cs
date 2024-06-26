﻿namespace Lwar.Gameplay.Client.Actors
{
	using System;
	using Entities;
	using Pegasus.Math;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a shield that protects a ship. The shield is only visible if it has just been hit. Invisible shields are
	///     removed from the game session.
	/// </summary>
	internal class ShieldActor : Actor<ShieldActor>
	{
		/// <summary>
		///     The time (in seconds) it takes for the shield to fully fade out.
		/// </summary>
		private const float FadeOutTime = 1.0f;

		/// <summary>
		///     Track the previous ship position so that we can correctly 'drag along' the impact position.
		/// </summary>
		private Vector2 _previousShipPosition;

		/// <summary>
		///     The time (in seconds) for which the shield remains visible (but slowly fading out).
		/// </summary>
		private float _remainingTime;

		/// <summary>
		///     Gets the remaining time to live in the range [0,1], starting with 1.
		/// </summary>
		public float TimeToLive
		{
			get { return _remainingTime / FadeOutTime; }
		}

		/// <summary>
		///     Gets the ship that the shield belongs to.
		/// </summary>
		public ShipEntity Ship { get; private set; }

		/// <summary>
		///     Gets the position where the shield has been hit.
		/// </summary>
		public Vector2 ImpactPosition { get; private set; }

		/// <summary>
		///     Updates the actor's internal state.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		public override void Update(float elapsedSeconds)
		{
			_remainingTime -= elapsedSeconds;

			if (_remainingTime < 0.0f)
				GameSession.Actors.Remove(this);

			var offset = Ship.Position - _previousShipPosition;
			_previousShipPosition = Ship.Position;
			ImpactPosition += offset;
		}

		/// <summary>
		///     Invoked when the actor is added to the game session.
		/// </summary>
		protected override void OnAdded()
		{
			_previousShipPosition = Ship.Position;
			Transform.AttachTo(Ship.Transform);
		}

		/// <summary>
		///     Hits the shield at the given position, resetting its time to live.
		/// </summary>
		/// <param name="impactPosition"></param>
		public void Hit(Vector2 impactPosition)
		{
			ImpactPosition = impactPosition;
			_remainingTime = FadeOutTime;
		}

		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the instance should be created for.</param>
		/// <param name="ship">The ship that the shield belongs to.</param>
		/// <param name="impactPosition">The position where the shield has been hit.</param>
		public static ShieldActor Create(ClientGameSession gameSession, ShipEntity ship, Vector2 impactPosition)
		{
			Assert.ArgumentNotNull(gameSession);
			Assert.ArgumentNotNull(ship);

			var shield = gameSession.Allocate<ShieldActor>();
			shield.Ship = ship;
			shield._remainingTime = FadeOutTime;
			shield.ImpactPosition = impactPosition;

			gameSession.Actors.Add(shield);
			return shield;
		}
	}
}