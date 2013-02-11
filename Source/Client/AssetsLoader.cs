using System;

namespace Lwar.Client
{
	using Pegasus.Framework.Platform.Assets;

	/// <summary>
	///   Loads assets into static asset fields.
	/// </summary>
	public static class AssetsLoader
	{
		/// <summary>
		///   Loads all assets.
		/// </summary>
		/// <param name="assets">The assets manager that should be used to load the assets.</param>
		public static void Load(AssetsManager assets)
		{
			Lwar.Client.Gameplay.Bullet.Texture = assets.LoadTexture2D("Textures/Bullet");
			Lwar.Client.Gameplay.GameSession.LoadingFont = assets.LoadFont("Fonts/Liberation Mono 12");
			Lwar.Client.Gameplay.Planet.Texture = assets.LoadTexture2D("Textures/Planet");
			Lwar.Client.Gameplay.Rocket.Texture = assets.LoadTexture2D("Textures/Rocket");
			Lwar.Client.Gameplay.Ship.Texture = assets.LoadTexture2D("Textures/Ship");
			Lwar.Client.Rendering.PlanetRenderer.VertexShader = assets.LoadVertexShader("Shaders/SphereVS");
			Lwar.Client.Rendering.PlanetRenderer.FragmentShader = assets.LoadFragmentShader("Shaders/SphereFS");
			Lwar.Client.Rendering.ShipRenderer.VertexShader = assets.LoadVertexShader("Shaders/QuadVS");
			Lwar.Client.Rendering.ShipRenderer.FragmentShader = assets.LoadFragmentShader("Shaders/QuadFS");
		}
	}
}

