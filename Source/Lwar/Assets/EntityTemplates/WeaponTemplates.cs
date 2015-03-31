namespace Lwar.Assets.EntityTemplates
{
	using System;
	using Compilation;

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
			Mass = 0.01f,
			Radius = 16,
			Acceleration = new Vector2(6000, 0),
			Format = "format_pos"
		};

		public static readonly EntityTemplate Rocket = new EntityTemplate
		{
			Act = "rocket_aim",
			Collide = "rocket_hit",
			Energy = 1000,
			Health = 1,
			Shield = 1,
			Mass = 1,
			Radius = 32,
			Acceleration = new Vector2(2000, 1000),
			Decelaration = new Vector2(1000, 1000),
			Rotation = 20,
			Format = "format_pos_rot"
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
			Interval = 100,
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

        public static readonly EntityTemplate RocketLauncher = new EntityTemplate
        {
            Act = "rocket_launch",
            Interval = 1000,
            Energy = 10,
            Health = 1
        };
	}
}
