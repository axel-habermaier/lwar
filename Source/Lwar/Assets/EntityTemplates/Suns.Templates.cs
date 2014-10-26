namespace Lwar.Assets.EntityTemplates
{
	using System;
	using Compilation;

	/// <summary>
	///     The sun entity templates that are shared between the client and the server.
	/// </summary>
	public static class Suns
	{
		public static readonly EntityTemplate Sun = new EntityTemplate
		{
			Act = "gravity",
			Collide = "planet_hit",
			Health = 1,
			Mass = 5000000,
			Radius = 4000,
			Format = "format_pos"
		};
	}
}