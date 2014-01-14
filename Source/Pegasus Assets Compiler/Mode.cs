namespace Pegasus.AssetsCompiler
{
	using System;

	/// <summary>
	///     Indicates the mode of the asset compiler.
	/// </summary>
	public enum Mode
	{
		/// <summary>
		///     Indicates that the asset compiler is cleaning all compiled assets.
		/// </summary>
		Clean,

		/// <summary>
		///     Indicates that the asset compiler is compiling all assets that require compilation.
		/// </summary>
		Compile,

		/// <summary>
		///     Indicates that the asset compiler is compiling all assets.
		/// </summary>
		Recompile,

		/// <summary>
		///     Indicates that the asset compiler is monitoring changes made to asset files and compiling assets as needed.
		/// </summary>
		Monitor
	}
}