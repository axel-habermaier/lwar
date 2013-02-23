
using System;

namespace Lwar.Assets
{
	using System.Collections.Generic;
	using Pegasus.AssetsCompiler;

	/// <summary>
	///   Provides access to all assets that should be compiled.
	/// </summary>
	public partial class AssetList : IAssetList
	{
		private readonly Asset _fontsLiberationMono12Fnt = new Asset("Fonts/Liberation Mono 12.fnt");
		private readonly Asset _shadersQuadVSVs = new Asset("Shaders/QuadVS.vs");
		private readonly Asset _shadersSphereVSVs = new Asset("Shaders/SphereVS.vs");
		private readonly Asset _shadersSpriteVSVs = new Asset("Shaders/SpriteVS.vs");
		private readonly Asset _shadersQuadFSFs = new Asset("Shaders/QuadFS.fs");
		private readonly Asset _shadersSphereFSFs = new Asset("Shaders/SphereFS.fs");
		private readonly Asset _shadersSpriteFSFs = new Asset("Shaders/SpriteFS.fs");
		private readonly Asset _texturesBulletPng = new Asset("Textures/Bullet.png");
		private readonly Asset _texturesPlanetPng = new Asset("Textures/Planet.png");
		private readonly Asset _texturesRocketPng = new Asset("Textures/Rocket.png");
		private readonly Asset _texturesShipPng = new Asset("Textures/Ship.png");
		private readonly Asset _texturesSunPng = new Asset("Textures/Sun.png");
		private readonly Asset _texturesSunHeatPng = new Asset("Textures/SunHeat.png");

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public AssetList()
		{
			OverrideProcessors();
		}

		/// <summary>
		///   Gets all assets that should be compiled.
		/// </summary>
		public IEnumerable<Asset> Assets 
		{
			get
			{
				yield return _fontsLiberationMono12Fnt;
				yield return _shadersQuadVSVs;
				yield return _shadersSphereVSVs;
				yield return _shadersSpriteVSVs;
				yield return _shadersQuadFSFs;
				yield return _shadersSphereFSFs;
				yield return _shadersSpriteFSFs;
				yield return _texturesBulletPng;
				yield return _texturesPlanetPng;
				yield return _texturesRocketPng;
				yield return _texturesShipPng;
				yield return _texturesSunPng;
				yield return _texturesSunHeatPng;
			}
		}

		/// <summary>
		///   Allows overriding the default asset processors of the assets.
		/// </summary>
		partial void OverrideProcessors();
	}
}

