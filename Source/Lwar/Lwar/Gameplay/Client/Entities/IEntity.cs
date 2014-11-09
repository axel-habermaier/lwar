namespace Lwar.Gameplay.Client.Entities
{
	using System;
	using Actors;
	using Network;
	using Pegasus.Math;

	/// <summary>
	///     Represents an entity, a special form of an actor that is synchronized with and under control of the server.
	/// </summary>
	public interface IEntity : IActor
	{
		/// <summary>
		///     Gets the generational identity of the entity.
		/// </summary>
		NetworkIdentity Identifier { get; }

		/// <summary>
		///     Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="position">The updated entity position.</param>
		void RemotePositionUpdate(Vector2 position);

		/// <summary>
		///     Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="rotation">The updated entity rotation.</param>
		void RemoteOrientationUpdate(float rotation);

		/// <summary>
		///     Handles a collision between this entity and another entity at the given impact position.
		/// </summary>
		/// <param name="other">The other entity this instance collided with.</param>
		/// <param name="impactPosition">The position of the impact.</param>
		void OnCollision(IEntity other, Vector2 impactPosition);

		/// <summary>
		///     Checks whether the entity accepts an update with the given sequence number. All following entity updates are only
		///     accepted when their sequence number exceeds the given one.
		/// </summary>
		/// <param name="sequenceNumber">The sequence number that should be checked.</param>
		bool AcceptUpdate(uint sequenceNumber);
	}
}