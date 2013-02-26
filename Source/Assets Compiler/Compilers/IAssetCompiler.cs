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
		///   Compiles all assets of the compiler's asset source type.
		/// </summary>
		/// <param name="assets">The asset that should be compiled.</param>
		void Compile(IEnumerable<Asset> assets);
	}
}