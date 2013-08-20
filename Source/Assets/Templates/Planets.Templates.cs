using System;

namespace Lwar.Assets.Templates
{
	using Compilation;

	/// <summary>
	///   The planet entity templates that are shared between the client and the server.
	/// </summary>
	internal static class Planets
	{
		public static readonly Template Earth = new Template
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

		public static readonly Template Mars = new Template
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

		public static readonly Template Moon = new Template
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

		public static readonly Template Jupiter = new Template
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