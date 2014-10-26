namespace Lwar.Gameplay.Client.Entities
{
	using System;
	using Pegasus.Math;

	/// <summary>
	///     Represents an entity that accepts remote circle updates.
	/// </summary>
	public interface ICircleEntity : IEntity
	{
		/// <summary>
		///     Applies the remote circle update to the entity.
		/// </summary>
		/// <param name="center">The updated circle center.</param>
		/// <param name="radius">The updated circle radius.</param>
		void RemoteCircleUpdate(Vector2 center, float radius);
	}
}