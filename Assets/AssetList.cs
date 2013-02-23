using System;

namespace Lwar.Assets
{
	using Pegasus.AssetsCompiler;

	public class AssetList : Pegasus.AssetsCompiler.AssetList
	{
		/// <summary>
		///   Allows overriding the default asset processors of the assets.
		/// </summary>
		protected override void OverrideProcessors()
		{
			OverrideProcessor("Textures/Sun.png", new CubeMapProcessor());
			OverrideProcessor("Textures/SunHeat.png", new CubeMapProcessor());
		}
	}
}