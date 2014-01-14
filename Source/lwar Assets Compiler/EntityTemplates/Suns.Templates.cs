using System;

namespace Lwar.AssetsCompiler.EntityTemplates
{
	using System;
	using AssetsCompiler.EntityTemplates.Compilation;

	/// <summary>
	///   The sun entity templates that are shared between the client and the server.
	/// </summary>
	public static class Suns
	{
		public static readonly EntityTemplate Sun = new EntityTemplate
		{
			Act = "gravity",
			Collide = "planet_hit",
			Health = 1,
			Mass = 50000,
			Radius = 1500,
			Format = "format_pos"
		};
	}
}