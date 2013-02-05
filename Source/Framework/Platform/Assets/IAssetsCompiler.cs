using System;

namespace Pegasus.Framework.Platform.Assets
{
	/// <summary>
	///   Provides methods to compile assets.
	/// </summary>
	public interface IAssetsCompiler
	{
		/// <summary>
		///   Compiles the asset at the given path, writing the result to the target directory. The name of the compiled
		///   asset file is returned on success.
		/// </summary>
		/// <param name="asset">The path of the asset that should be compiled, relative to the Assets base directory.</param>
		string Compile(string asset);

		/// <summary>
		///   Compiles all assets.
		/// </summary>
		void Compile();
	}
}