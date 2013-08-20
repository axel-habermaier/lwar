using System;

namespace Lwar.Gameplay.Entities
{
	using Pegasus.Math;

	/// <summary>
	///   Represents a shockwave.
	/// </summary>
	public class Shockwave : Entity<Shockwave>
	{
		/// <summary>
		///   Gets the current radius of the shockwave.
		/// </summary>
		public float Radius { get; private set; }

		/// <summary>
		///   Applies the update message sent by the server to the entity's state.
		/// </summary>
		/// <param name="center">The updated circle center.</param>
		/// <param name="radius">The updated circle radius.</param>
		public override void RemoteUpdate(Vector2 center, float radius)
		{
			Position = center;
			Radius = radius;
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="id">The generational identifier of the shockwave.</param>
		public static Shockwave Create(Identifier id)
		{
			var shockwave = GetInstance();
			shockwave.Identifier = id;
			return shockwave;
		}
	}
}