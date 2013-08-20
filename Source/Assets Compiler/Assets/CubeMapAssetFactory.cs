using System;

namespace Pegasus.AssetsCompiler.Assets
{
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using Attributes;
	using Platform.Logging;

	/// <summary>
	///   Creates cube map asset instances.
	/// </summary>
	internal class CubeMapAssetFactory : IAssetFactory
	{
		/// <summary>
		///   Creates an asset instance for all assets of an supported type.
		/// </summary>
		/// <param name="assets">The assets that should be compiled.</param>
		/// <param name="attributes">The attributes that affect the compilation settings of some assets.</param>
		public IEnumerable<Asset> CreateAssets(IEnumerable<string> assets, IEnumerable<AssetAttribute> attributes)
		{
			var cubemapAttributes = attributes.OfType<CubeMapAttribute>().ToArray();

			foreach (var asset in assets)
			{
				if (!asset.EndsWith(".Cubemap.png", ignoreCase: true, culture: CultureInfo.InvariantCulture))
					continue;

				var settings = cubemapAttributes.Where(a => a.Name == asset).ToArray();
				if (settings.Length > 1)
					Log.Warn("Found multiple specifications of '{0}' for cube map '{1}'. Using the first one.", typeof(CubeMapAttribute).Name, asset);

				if (settings.Length == 1)
					yield return new CubeMapAsset(asset) { Mipmaps = settings[0].Mipmaps, Uncompressed = settings[0].Uncompressed };
				else
					yield return new CubeMapAsset(asset);
			}
		}
	}
}