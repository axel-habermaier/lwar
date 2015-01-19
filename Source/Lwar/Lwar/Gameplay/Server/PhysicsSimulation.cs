namespace Lwar.Gameplay.Server
{
	using System;
	using Pegasus.Utilities;

	/// <summary>
	///     Simulates the game physics.
	/// </summary>
	internal class PhysicsSimulation
	{
		/// <summary>
		///     The game session the physics simulation belongs to.
		/// </summary>
		private readonly GameSession _gameSession;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the physics simulation belongs to.</param>
		public PhysicsSimulation(GameSession gameSession)
		{
			Assert.ArgumentNotNull(gameSession);
			_gameSession = gameSession;
		}

		/// <summary>
		///     Advances the simulation by the given amount of time.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds the simulation should advance.</param>
		public void Simulate(float elapsedSeconds)
		{
		}
	}
}