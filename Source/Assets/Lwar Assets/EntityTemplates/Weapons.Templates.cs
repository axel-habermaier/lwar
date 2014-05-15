namespace Lwar.Assets.EntityTemplates
{
	using System;
	using Compilation;
	using Pegasus.Math;

	/// <summary>
	///     The weapon entity templates that are shared between the client and the server.
	/// </summary>
	public static class Weapons
	{
		public static readonly EntityTemplate Bullet = new EntityTemplate
		{
			Act = "decay",
			Collide = "bullet_hit",
			Health = 2000,
			Shield = 1,
			Mass = 0.1f,
			Radius = 16,
			Acceleration = new Vector2(0, 3000),
			Format = "format_pos"
		};

		public static readonly EntityTemplate Rocket = new EntityTemplate
		{
			Act = "aim",
			Energy = 1000,
			Health = 1,
			Shield = 1,
			Mass = 1,
			Radius = 16,
			Acceleration = new Vector2(500, 20),
			Decelaration = new Vector2(20, 20),
			Rotation = 1,
			Format = "format_ship"
		};

		public static readonly EntityTemplate Ray = new EntityTemplate
		{
			Act = "ray_act",
			Energy = 1,
			Health = 1,
			Radius = 4096,
			Format = "format_ray"
		};

		public static readonly EntityTemplate Shockwave = new EntityTemplate
		{
		};

		public static readonly EntityTemplate Gun = new EntityTemplate
		{
			Act = "gun_shoot",
			Interval = 150,
			Energy = 1000,
			Health = 1
		};

		public static readonly EntityTemplate Phaser = new EntityTemplate
		{
			Act = "phaser_shoot",
			Interval = 0,
			Energy = 1000,
			Health = 1
		};
	}
}