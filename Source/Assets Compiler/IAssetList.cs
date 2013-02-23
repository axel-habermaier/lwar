using System;

namespace Pegasus.AssetsCompiler
{
	using System.Collections.Generic;

	/// <summary>
	///   Provides access to all assets that should be compiled.
	/// </summary>
	public interface IAssetList
	{
		/// <summary>
		///   Gets all assets that should be compiled.
		/// </summary>
		IEnumerable<Asset> Assets { get; }
	}
}