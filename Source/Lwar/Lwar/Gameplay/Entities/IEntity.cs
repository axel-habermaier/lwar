namespace Lwar.Gameplay.Entities
{
	using System;
	using Actors;
	using Pegasus.Math;

	/// <summary>
	///     Represents an entity, a special form of an actor that is synchronized with and under control of the server.
	/// </summary>
	public interface IEntity : IActor
	{
		/// <summary>
		///     Gets the generational identifier of the entity.
		/// </summary>
		Identifier Identifier { get; }

		/// <summary>
		///     Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="position">The updated entity position.</param>
		void RemotePositionUpdate(Vector2 position);

		/// <summary>
		///     Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="rotation">The updated entity rotation.</param>
		void RemoteRotationUpdate(float rotation);

		/// <summary>
		///     Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="center">The updated circle center.</param>
		/// <param name="radius">The updated circle radius.</param>
		void RemoteCircleUpdate(Vector2 center, float radius);

		/// <summary>
		///     Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="origin">The updated ray origin.</param>
		/// <param name="direction">The updated ray direction.</param>
		/// <param name="length">The updated ray length.</param>
		/// <param name="target">The current ray target or null if no target is hit.</param>
		void RemoteRayUpdate(Vector2 origin, float direction, float length, IEntity target);

		/// <summary>
		///     Invoked when the entity collided another entity.
		/// </summary>
		/// <param name="other">The other entity this instance collided with.</param>
		/// <param name="impact">The position of the impact.</param>
		void CollidedWith(IEntity other, Vector2 impact);
	}
}