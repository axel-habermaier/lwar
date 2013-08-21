using System;

namespace Lwar.Assets.EntityTemplates
{
	using Compilation;

	/// <summary>
	///   The planet entity templates that are shared between the client and the server.
	/// </summary>
	public static class Planets
	{
		public static readonly EntityTemplate Earth = new EntityTemplate
		{
			Act = "gravity",
			Collide = "planet_hit",
			Health = 1,
			Mass = 10000,
			Radius = 256,
			CubeMap = "Textures.EarthCubemap",
			Model = t => String.Format("Model.CreateSphere(graphicsDevice, {0}, {1})", (int)t.Radius, 16),
			Format = "format_pos"
		};

		public static readonly EntityTemplate Mars = new EntityTemplate
		{
			Act = "gravity",
			Collide = "planet_hit",
			Health = 1,
			Mass = 10000,
			Radius = 128,
			CubeMap = "Textures.MarsCubemap",
			Model = t => String.Format("Model.CreateSphere(graphicsDevice, {0}, {1})", (int)t.Radius, 16),
			Format = "format_pos"
		};

		public static readonly EntityTemplate Moon = new EntityTemplate
		{
			Act = "gravity",
			Collide = "planet_hit",
			Health = 1,
			Mass = 10000,
			Radius = 64,
			CubeMap = "Textures.MoonCubemap",
			Model = t => String.Format("Model.CreateSphere(graphicsDevice, {0}, {1})", (int)t.Radius, 16),
			Format = "format_pos"
		};

		public static readonly EntityTemplate Jupiter = new EntityTemplate
		{
			Act = "gravity",
			Collide = "planet_hit",
			Health = 1,
			Mass = 10000,
			Radius = 512,
			CubeMap = "Textures.JupiterCubemap",
			Model = t => String.Format("Model.CreateSphere(graphicsDevice, {0}, {1})", (int)t.Radius, 16),
			Format = "format_pos"
		};
	}
}