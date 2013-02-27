using System;

namespace Lwar.Client.Gameplay.Entities
{
	using Network;
	using Rendering;

	/// <summary>
	///   Represents an entity.
	/// </summary>
	public interface IEntity : IGenerationalIdentity, IDisposable
	{
		/// <summary>
		///   Adds the entity to the game state and the render context.
		/// </summary>
		/// <param name="gameSession">The game state the entity should be added to.</param>
		/// <param name="renderContext">The render context the entity should be added to.</param>
		void Added(GameSession gameSession, RenderContext renderContext);

		/// <summary>
		///   Removes the entity from the game state and the render context.
		/// </summary>
		/// <param name="gameSession">The game state the entity should be removed from.</param>
		/// <param name="renderContext">The render context the entity should be removed from.</param>
		/// <remarks> The remove method is not called when the game session is shut down.</remarks>
		void Removed(GameSession gameSession, RenderContext renderContext);

		/// <summary>
		///   Updates the entity's internal state.
		/// </summary>
		void Update();

		/// <summary>
		///   Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="message">The update message that should be processed.</param>
		void RemoteUpdate(ref Message message);
	}
}