using System;

namespace Lwar.Assets.Templates
{
	using Compilation;
	using Pegasus.Math;

	/// <summary>
	///   The ship entity templates that are shared between the client and the server.
	/// </summary>
	internal static class Ships
	{
		public static readonly Template Ship = new Template
		{
			Collide = "ship_hit",
			Energy = 1000,
			Health = 3000,
			Shield = 1,
			Mass = 1,
			Radius = 64,
			Acceleration = new Vector2(2000, 2000),
			Decelaration = new Vector2(2000, 2000),
			Rotation = 10,
			Format = "format_ship"
		};
	}
}