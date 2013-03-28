﻿using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	/// <summary>
	///   Indicates the action that the compiler must take.
	/// </summary>
	internal enum CompilationAction
	{
		/// <summary>
		///   Indicates that the compiler can skip the asset as the latest version of the processed asset is already at the target
		///   location.
		/// </summary>
		Skip,

		/// <summary>
		///   Indicates that the compiler does not have to process the asset, but must copy the latest version of the processed
		///   asset to the target location.
		/// </summary>
		Copy,

		/// <summary>
		///   Indicates that the compiler has to process the asset.
		/// </summary>
		Process,
	}
}