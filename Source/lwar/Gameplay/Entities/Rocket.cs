﻿using System;

namespace Lwar.Gameplay.Entities
{
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
			rocket.Template = Templates.Rocket;
			return rocket;
		}
	}
}