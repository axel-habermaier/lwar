namespace Lwar.Assets.EntityTemplates
{
	using System;
	using Compilation;

	/// <summary>
	///     The planet entity templates that are shared between the client and the server.
	/// </summary>
	public static class Planets
	{
		public static readonly EntityTemplate Earth = new EntityTemplate
		{
			Act = "gravity",
			Collide = "planet_hit",
			Health = 1,
			Mass = 50000,
			Radius = 700,
			CubeMap = "Earth",
			Model = t => String.Format("Model.CreateSphere(renderContext.GraphicsDevice, {0}, {1})", (int)t.Radius, 16),
			Format = "format_pos"
		};

		public static readonly EntityTemplate Mars = new EntityTemplate
		{
			Act = "gravity",
			Collide = "planet_hit",
			Health = 1,
			Mass = 30000,
			Radius = 350,
			CubeMap = "Mars",
			Model = t => String.Format("Model.CreateSphere(renderContext.GraphicsDevice, {0}, {1})", (int)t.Radius, 16),
			Format = "format_pos"
		};

		public static readonly EntityTemplate Moon = new EntityTemplate
		{
			Act = "gravity",
			Collide = "planet_hit",
			Health = 1,
			Mass = 20000,
			Radius = 250,
			CubeMap = "Moon",
			Model = t => String.Format("Model.CreateSphere(renderContext.GraphicsDevice, {0}, {1})", (int)t.Radius, 16),
			Format = "format_pos"
		};

		public static readonly EntityTemplate Jupiter = new EntityTemplate
		{
			Act = "gravity",
			Collide = "planet_hit",
			Health = 1,
			Mass = 100000,
			Radius = 1000,
			CubeMap = "Jupiter",
			Model = t => String.Format("Model.CreateSphere(renderContext.GraphicsDevice, {0}, {1})", (int)t.Radius, 16),
			Format = "format_pos"
		};
	}
}