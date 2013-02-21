using System;

namespace Pegasus.Framework.Platform.Assets
{
	/// <summary>
	///   Provides methods to compile assets.
	/// </summary>
	public interface IAssetsCompiler
	{
		/// <summary>
		///   Compiles all assets.
		/// </summary>
		void Compile();
	}
}