using System;

namespace Lwar.Gameplay.Entities
{
	using Actors;
	using Network;
	using Network.Messages;
	using Pegasus.Framework.Math;

	/// <summary>
	///   Represents an entity, a special form of an actor that is synchronized with and under control of the server.
	/// </summary>
	public interface IEntity : IActor, IGenerationalIdentity
	{
		/// <summary>
		///   Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="message">The update message that should be processed.</param>
		void RemoteUpdate(ref Message message);

		/// <summary>
		///   Invoked when the entity collided another entity.
		/// </summary>
		/// <param name="other">The other entity this instance collided with.</param>
		/// <param name="impact">The position of the impact.</param>
		void CollidedWith(IEntity other, Vector2 impact);
	}
}