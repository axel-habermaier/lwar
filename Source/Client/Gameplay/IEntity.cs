using System;

namespace Pegasus.Gameplay
{
	using Client.Gameplay;
	using Framework.Math;
	using Framework.Platform;

	/// <summary>
	///   Represents an entity.
	/// </summary>
	public interface IEntity : IDisposable
	{
		/// <summary>
		///   Gets or sets the entity's position.
		/// </summary>
		Vector2f8 Position { get; set; }

		/// <summary>
		///   Gets or sets the entity's unique identifier.
		/// </summary>
		EntityIdentifier Id { get; set; }

		/// <summary>
		///   Invoked when the entity should update its internal state.
		/// </summary>
		void Update();

		/// <summary>
		///   Invoked when the entity should draw itself.
		/// </summary>
		void Draw();

		/// <summary>
		///   Invoked when the entity has been added to a level.
		/// </summary>
		/// <param name="session">The game session the entity belongs to.</param>
		void Added(GameSession session);

		/// <summary>
		///   Invoked when the entity has been removed from a level.
		/// </summary>
		void Removed();
	}
}