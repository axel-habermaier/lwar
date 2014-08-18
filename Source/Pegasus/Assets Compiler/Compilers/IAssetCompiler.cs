namespace Pegasus.AssetsCompiler.Compilers
{
	using System;
	using System.Collections.Generic;
	using Assets;

	/// <summary>
	///     Represents a compiler that compiles source assets into a binary format that the runtime can load more efficiently.
	/// </summary>
	internal interface IAssetCompiler : IDisposable
	{
		/// <summary>
		///     Gets the additional assets created by the compiler.
		/// </summary>
		IEnumerable<Asset> AdditionalAssets { get; }

		/// <summary>
		///     Compiles all assets of the compiler's asset source type.
		/// </summary>
		/// <param name="assets">The assets that should be compiled.</param>
		void Compile(IEnumerable<Asset> assets);

		/// <summary>
		///     Removes the compiled assets and all temporary files written by the compiler.
		/// </summary>
		/// <param name="assets">The assets that should be cleaned.</param>
		void Clean(IEnumerable<Asset> assets);
	}
}