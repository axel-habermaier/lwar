using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	/// <summary>
	///   Indicates the action that the Compiler must take.
	/// </summary>
	internal enum CompilationAction
	{
		/// <summary>
		///   Indicates that the Compiler can skip the asset as the latest version of the processed asset is already at the target
		///   location.
		/// </summary>
		Skip,

		/// <summary>
		///   Indicates that the Compiler does not have to process the asset, but must copy the latest version of the processed
		///   asset to the target location.
		/// </summary>
		Copy,

		/// <summary>
		///   Indicates that the Compiler has to process the asset.
		/// </summary>
		Process,
	}
}