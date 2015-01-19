namespace Lwar.Gameplay.Client.Entities
{
	using System;
	using Pegasus.Math;

	/// <summary>
	///     Represents an entity that accepts remote ray updates.
	/// </summary>
	internal interface IRayEntity : IEntity
	{
		/// <summary>
		///     Applies the remote ray update to the entity.
		/// </summary>
		/// <param name="origin">The updated ray origin.</param>
		/// <param name="direction">The updated ray direction.</param>
		/// <param name="length">The updated ray length.</param>
		/// <param name="target">The current ray target or null if no target is hit.</param>
		void RemoteRayUpdate(Vector2 origin, float direction, float length, IEntity target);
	}
}