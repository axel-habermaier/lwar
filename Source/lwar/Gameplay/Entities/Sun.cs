using System;

namespace Lwar.Gameplay.Entities
{
	/// <summary>
	///   Represents a sun.
	/// </summary>
	public class Sun : Entity<Sun>
	{
		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="id">The generational identifier of the sun.</param>
		public static Sun Create(Identifier id)
		{
			var sun = GetInstance();
			sun.Identifier = id;
			sun.Template = Templates.Sun;
			return sun;
		}
	}
}