using System;

namespace Pegasus.AssetsCompiler.Assets
{
	using System.Globalization;

	/// <summary>
	///   Creates cube map asset instances.
	/// </summary>
	internal class CubeMapAssetFactory : IAssetFactory
	{
		/// <summary>
		///   Creates an asset instance for the asset with the given name. If the asset type is not supported, null must be
		///   returned.
		/// </summary>
		/// <param name="assetName">The name of the asset that should be created.</param>
		public Asset CreateAsset(string assetName)
		{
			if (assetName.EndsWith("Cubemap.png", ignoreCase: true, culture: CultureInfo.InvariantCulture))
				return new CubeMapAsset(assetName);

			return null;
		}
	}
}