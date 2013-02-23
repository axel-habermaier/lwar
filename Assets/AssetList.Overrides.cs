using System;

namespace Lwar.Assets
{
	using Pegasus.AssetsCompiler;

	public partial class AssetList
	{
		/// <summary>
		///   Allows overriding the default asset processors of the assets.
		/// </summary>
		partial void OverrideProcessors()
		{
			_texturesSunHeatPng.Processor = new CubeMapProcessor();
			_texturesSunPng.Processor = new CubeMapProcessor();
		}
	}
}