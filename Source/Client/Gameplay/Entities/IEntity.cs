using System;

namespace Lwar.Client.Gameplay.Entities
{
	using Network;
	using Pegasus.Framework.Platform;
	using Rendering;

	/// <summary>
	///   Represents an entity.
	/// </summary>
	public interface IEntity : IGenerationalIdentity, IDisposable
	{
		/// <summary>
		///   Adds the entity to the game session and the render context.
		/// </summary>
		/// <param name="gameSession">The game session the entity should be added to.</param>
		/// <param name="renderContext">The render context the entity should be added to.</param>
		void Added(GameSession gameSession, RenderContext renderContext);

		/// <summary>
		///   Removes the entity from the game session and the render context.
		/// </summary>
		/// <remarks>This method is not called when the game session is shut down.</remarks>
		void Removed();

		/// <summary>
		///   Updates the entity's internal state.
		/// </summary>
		/// <param name="clock">The clock that should be used for time measurements.</param>
		void Update(Clock clock);

		/// <summary>
		///   Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="message">The update message that should be processed.</param>
		void RemoteUpdate(ref Message message);
	}
}