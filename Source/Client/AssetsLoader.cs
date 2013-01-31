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
			Lwar.Client.Gameplay.Ship.Texture = assets.LoadTexture2D("Textures/Ship");
		}
	}
}

