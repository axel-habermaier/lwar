using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using System.Collections.Generic;
	using Assets;

	/// <summary>
	///   Represents a compiler that compiles source assets into a binary format that the runtime can load more efficiently.
	/// </summary>
	internal interface IAssetCompiler
	{
		/// <summary>
		///   Compiles all assets of the compiler's asset source type. Returns true to indicate that the compilation of all assets
		///   has been successful.
		/// </summary>
		/// <param name="assets">The asset that should be compiled.</param>
		bool Compile(IEnumerable<Asset> assets);
	}
}