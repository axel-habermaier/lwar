using System;

namespace Pegasus.AssetsCompiler.Assets
{
	using System.Collections.Generic;
	using Attributes;

	/// <summary>
	///   Creates assets of a certain type.
	/// </summary>
	public interface IAssetFactory
	{
		/// <summary>
		///   Creates an asset instance for all assets of an supported type.
		/// </summary>
		/// <param name="assets">The assets that should be compiled.</param>
		/// <param name="attributes">The attributes that affect the compilation settings of some assets.</param>
		IEnumerable<Asset> CreateAssets(IEnumerable<string> assets, IEnumerable<AssetAttribute> attributes);
	}
}