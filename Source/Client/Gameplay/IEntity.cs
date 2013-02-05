using System;

namespace Lwar.Client.Gameplay
{
	using Pegasus.Framework.Math;

	/// <summary>
	///   Represents an entity.
	/// </summary>
	public interface IEntity : IDisposable
	{
		/// <summary>
		///   Gets or sets the entity's position.
		/// </summary>
		Vector2 Position { get; set; }

		/// <summary>
		///   Gets or sets the entity's rotation.
		/// </summary>
		float Rotation { get; set; }

		/// <summary>
		///   Gets or sets the entity's health.
		/// </summary>
		float Health { get; set; }

		/// <summary>
		///   Gets or sets the entity's unique identifier.
		/// </summary>
		Identifier Id { get; set; }

		/// <summary>
		///   Gets or sets the player the entity belongs to.
		/// </summary>
		Player Player { get; set; }

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

		/// <summary>
		///   Invoked when a collision occured between the entity and the given other entity.
		/// </summary>
		/// <param name="other">The other entity of the collision.</param>
		/// <param name="position">The position of the collision.</param>
		void OnCollision(IEntity other, Vector2 position);
	}
}