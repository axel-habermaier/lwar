using System;

namespace Pegasus.AssetsCompiler.Assets
{
	/// <summary>
	///   Creates assets of a certain type.
	/// </summary>
	public interface IAssetFactory
	{
		/// <summary>
		///   Creates an asset instance for the asset with the given name. If the asset type is not supported, null must be
		///   returned.
		/// </summary>
		/// <param name="assetName">The name of the asset that should be created.</param>
		Asset CreateAsset(string assetName);
	}
}