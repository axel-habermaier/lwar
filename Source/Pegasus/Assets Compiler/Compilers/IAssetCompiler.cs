namespace Pegasus.AssetsCompiler.Compilers
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using System.Xml.Linq;
	using Assets;

	/// <summary>
	///     Represents a compiler that compiles source assets into a binary format that the runtime can load more efficiently.
	/// </summary>
	public interface IAssetCompiler
	{
		/// <summary>
		///     Gets the assets compiled by the asset compiler.
		/// </summary>
		IEnumerable<Asset> Assets { get; }

		/// <summary>
		///     Compiles the assets.
		/// </summary>
		/// <param name="assets">The metadata of the assets that should be compiled.</param>
		Task<bool> Compile(IEnumerable<XElement> assets);

		/// <summary>
		///     Cleans the asset if the compiler supports the given asset type.
		/// </summary>
		/// <param name="assetMetadata">The metadata of the asset that should be cleaned.</param>
		void Clean(XElement assetMetadata);
	}
}