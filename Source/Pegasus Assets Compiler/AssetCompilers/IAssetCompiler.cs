namespace Pegasus.AssetsCompiler.AssetCompilers
{
	using System;
	using System.Collections.Generic;
	using Assets;

	/// <summary>
	///     Represents a compiler that compiles source assets into a binary format that the runtime can load more efficiently.
	/// </summary>
	public interface IAssetCompiler
	{
		/// <summary>
		///     Gets the tag name of the assets supported by the compiler.
		/// </summary>
		string AssetTag { get; }

		/// <summary>
		///     Creates an asset instance from the given metadata in the asset project file.
		/// </summary>
		/// <param name="context">The context the asset should be compiled in.</param>
		/// <param name="metadata">The metadata of the asset defined in the asset project file..</param>
		Asset CreateAsset(CompilationContext context, Dictionary<string, string> metadata);

		/// <summary>
		///     Compiles the given assets, returning true on success.
		/// </summary>
		/// <param name="assets">The assets that should be compiled.</param>
		bool Compile(IEnumerable<Asset> assets);

		/// <summary>
		///     Removes the compiled assets and all temporary files written by the compiler.
		/// </summary>
		/// <param name="assets">The assets that should be cleaned.</param>
		void Clean(IEnumerable<Asset> assets);
	}
}