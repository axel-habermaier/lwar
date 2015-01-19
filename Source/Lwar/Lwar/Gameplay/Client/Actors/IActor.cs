namespace Lwar.Gameplay.Client.Actors
{
	using System;
	using Rendering;

	/// <summary>
	///     Represents an actor.
	/// </summary>
	internal interface IActor : IDisposable
	{
		/// <summary>
		///     Gets or sets the parent actor.
		/// </summary>
		IActor Parent { get; set; }

		/// <summary>
		///     Gets the transformation of the actor.
		/// </summary>
		Transformation Transform { get; }

		/// <summary>
		///     Gets a value indicating whether the actor has been removed from the game session.
		/// </summary>
		bool IsRemoved { get; }

		/// <summary>
		///     Adds the actor to the game session and the render context.
		/// </summary>
		/// <param name="gameSession">The game session the actor should be added to.</param>
		/// <param name="renderer">The render context the actor should be added to.</param>
		void Added(ClientGameSession gameSession, GameSessionRenderer renderer);

		/// <summary>
		///     Removes the actor from the game session and the render context.
		/// </summary>
		/// <remarks>This method is not called when the game session is shut down.</remarks>
		void Removed();

		/// <summary>
		///     Updates the actor's internal state.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		void Update(float elapsedSeconds);
	}
}