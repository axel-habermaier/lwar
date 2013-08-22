using System;

namespace Pegasus.AssetsCompiler.Assets
{
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using Attributes;

	/// <summary>
	///   Creates Xaml asset instances.
	/// </summary>
	internal class XamlAssetFactory : IAssetFactory
	{
		/// <summary>
		///   Creates an asset instance for all assets of an supported type.
		/// </summary>
		/// <param name="assets">The assets that should be compiled.</param>
		/// <param name="attributes">The attributes that affect the compilation settings of some assets.</param>
		public IEnumerable<Asset> CreateAssets(IEnumerable<string> assets, IEnumerable<AssetAttribute> attributes)
		{
			return from asset in assets
				   where asset.EndsWith(".xaml", ignoreCase: true, culture: CultureInfo.InvariantCulture)
				   select new XamlAsset(asset);
		}
	}
}