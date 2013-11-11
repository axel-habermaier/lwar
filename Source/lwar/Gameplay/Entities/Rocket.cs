namespace Lwar.Gameplay.Entities
{
	using System;

	/// <summary>
	///   Represents a rocket.
	/// </summary>
	public class Rocket : Entity<Rocket>
	{
		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="id">The generational identifier of the rocket.</param>
		public static Rocket Create(Identifier id)
		{
			var rocket = GetInstance();
			rocket.Identifier = id;
			rocket.Template = EntityTemplates.Rocket;
			return rocket;
		}
	}
}