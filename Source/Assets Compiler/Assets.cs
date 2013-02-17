using System;

namespace Pegasus.AssetsCompiler
{
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	///   Holds the assets that are compiled.
	/// </summary>
	public static class Assets
	{
		/// <summary>
		///   The 2D textures that are compiled.
		/// </summary>
		public static readonly string[] Textures2D =
		{
			"Textures/Bullet.png",
			"Textures/Planet.png",
			"Textures/Ship.png",
			"Textures/Rocket.png",
			"Textures/Test.png",
		};

		/// <summary>
		///   The cube maps that are compiled.
		/// </summary>
		public static readonly string[] CubeMaps =
		{
			"Textures/Sun.png",
			"Textures/SunHeat.png",
		};

		/// <summary>
		///   The vertex shaders that are compiled.
		/// </summary>
		public static readonly string[] VertexShaders =
		{
			"Shaders/QuadVS",
			"Shaders/SphereVS",
			"Shaders/SpriteVS"
		};

		/// <summary>
		///   The fragment shaders that are compiled.
		/// </summary>
		public static readonly string[] FragmentShaders =
		{
			"Shaders/QuadFS",
			"Shaders/SphereFS",
			"Shaders/SpriteFS"
		};

		/// <summary>
		///   The fonts that are compiled.
		/// </summary>
		public static readonly string[] Fonts =
		{
			"Fonts/Liberation Mono 12.fnt"
		};

		/// <summary>
		///   Gets the list of all assets that are compiled.
		/// </summary>
		public static IEnumerable<string> All
		{
			get { return Textures2D.Union(CubeMaps).Union(VertexShaders).Union(FragmentShaders).Union(Fonts); }
		}
	}
}