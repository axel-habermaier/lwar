using System;

namespace Lwar.Client.Gameplay.Entities
{
	using Network.Messages;
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
		void Removed(GameSession gameSession, RenderContext renderContext);

		/// <summary>
		///   Updates the entity's internal state.
		/// </summary>
		void Update();

		/// <summary>
		///   Applies the remote update record to the entity's state.
		/// </summary>
		/// <param name="update">The update record that has been sent by the server for this entity.</param>
		/// <param name="timestamp">The timestamp that indicates when the update record has been sent.</param>
		void RemoteUpdate(UpdateRecord update, uint timestamp);
	}
}