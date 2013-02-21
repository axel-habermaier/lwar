using System;

namespace Pegasus.Framework.Platform.Assets
{
	using System.Collections.Generic;

	/// <summary>
	///   Provides methods to compile assets.
	/// </summary>
	public interface IAssetsCompiler
	{
		/// <summary>
		///   Compiles all assets and returns the names of the assets that have been changed.
		/// </summary>
		IEnumerable<string> Compile();
	}
}