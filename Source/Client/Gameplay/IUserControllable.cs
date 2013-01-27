using System;

namespace Client.Gameplay
{
	using Pegasus.Framework.Math;
	using Pegasus.Gameplay;

	/// <summary>
	///   Represents an entity that can be controlled by the user.
	/// </summary>
	public interface IUserControllable : IEntity
	{
		/// <summary>
		///   Invoked when the user wants to issue a 'move to' command. Should return true to indicate that the
		///   command is valid.
		/// </summary>
		/// <param name="position">The target position.</param>
		bool ValidateMoveTo(Vector2f8 position);

		/// <summary>
		///   Invoked when the user wants to issue a 'context-sensitive action' command for the given target entity. For instance,
		///   the user could request a unit to attack another one or to harvest a resource. Should return true to indicate that the
		///   command is valid.
		/// </summary>
		/// <param name="entity">The entity that should be the target of the context-sensitive action.</param>
		bool ValidateContextAction(IEntity entity);
	}
}