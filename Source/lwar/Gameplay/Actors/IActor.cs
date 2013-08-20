using System;

namespace Lwar.Gameplay.Actors
{
	using Pegasus.Platform;
	using Rendering;

	/// <summary>
	///   Represents an actor.
	/// </summary>
	public interface IActor : IDisposable
	{
		/// <summary>
		///   Adds the actor to the game session and the render context.
		/// </summary>
		/// <param name="gameSession">The game session the actor should be added to.</param>
		/// <param name="renderContext">The render context the actor should be added to.</param>
		void Added(GameSession gameSession, RenderContext renderContext);

		/// <summary>
		///   Removes the actor from the game session and the render context.
		/// </summary>
		/// <remarks>This method is not called when the game session is shut down.</remarks>
		void Removed();

		/// <summary>
		///   Updates the actor's internal state.
		/// </summary>
		/// <param name="clock">The clock that should be used for time measurements.</param>
		void Update(Clock clock);
	}
}