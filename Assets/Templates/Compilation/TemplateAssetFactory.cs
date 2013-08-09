using System;

namespace Lwar.Assets.Templates.Compilation
{
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using Pegasus.AssetsCompiler.Assets;
	using Pegasus.AssetsCompiler.Assets.Attributes;

	/// <summary>
	///   Creates template asset instances.
	/// </summary>
	public class TemplateAssetFactory : IAssetFactory
	{
		/// <summary>
		///   Creates an asset instance for all assets of an supported type.
		/// </summary>
		/// <param name="assets">The assets that should be compiled.</param>
		/// <param name="attributes">The attributes that affect the compilation settings of some assets.</param>
		public IEnumerable<Asset> CreateAssets(IEnumerable<string> assets, IEnumerable<AssetAttribute> attributes)
		{
			return from asset in assets
				   where asset.EndsWith(".Templates.cs", ignoreCase: true, culture: CultureInfo.InvariantCulture)
				   select new TemplateAsset(asset);
		}
	}
}