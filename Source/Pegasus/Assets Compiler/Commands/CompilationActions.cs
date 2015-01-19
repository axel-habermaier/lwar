namespace Pegasus.AssetsCompiler.Commands
{
	using System;

	/// <summary>
	///     Determines a set of compilation actions.
	/// </summary>
	[Flags]
	public enum CompilationActions
	{
		/// <summary>
		///     Indicates that the assets should be compiled.
		/// </summary>
		Compile = 1,

		/// <summary>
		///     Indicates that the assets should be cleaned.
		/// </summary>
		Clean = 2
	}
}