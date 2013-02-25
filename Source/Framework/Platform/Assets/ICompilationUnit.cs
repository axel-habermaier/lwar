using System;

namespace Pegasus.Framework.Platform.Assets
{
	using System.Collections.Generic;

	/// <summary>
	///   Represents a compilation unit that compiles all assets into a binary format.
	/// </summary>
	public interface ICompilationUnit
	{
		/// <summary>
		///   Compiles all assets and returns the names of the assets that have been changed.
		/// </summary>
		IEnumerable<string> Compile();

		/// <summary>
		///   Removes the hash files of all assets, as well as their compiled outputs in the temp and target directories.
		/// </summary>
		void Clean();
	}
}