namespace Pegasus.AssetsCompiler.Assets
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using Attributes;
	using Platform.Logging;

	/// <summary>
	///   Creates texture 2D asset instances.
	/// </summary>
	internal class Texture2DAssetFactory : IAssetFactory
	{
		/// <summary>
		///   Creates an asset instance for all assets of an supported type.
		/// </summary>
		/// <param name="assets">The assets that should be compiled.</param>
		/// <param name="attributes">The attributes that affect the compilation settings of some assets.</param>
		public IEnumerable<Asset> CreateAssets(IEnumerable<string> assets, IEnumerable<AssetAttribute> attributes)
		{
			var texture2DAttributes = attributes.OfType<Texture2DAttribute>().ToArray();

			foreach (var asset in assets)
			{
				if (!asset.EndsWith(".png", ignoreCase: true, culture: CultureInfo.InvariantCulture) ||
					asset.EndsWith(".Cubemap.png", ignoreCase: true, culture: CultureInfo.InvariantCulture))
					continue;

				var settings = texture2DAttributes.Where(a => a.Name == asset).ToArray();
				if (settings.Length > 1)
					Log.Warn("Found multiple specifications of '{0}' for cube map '{1}'. Using the first one.", typeof(CubeMapAttribute).Name, asset);

				if (settings.Length == 1)
					yield return new Texture2DAsset(asset) { Mipmaps = settings[0].Mipmaps, Uncompressed = settings[0].Uncompressed };
				else
					yield return new Texture2DAsset(asset);
			}
		}
	}
}