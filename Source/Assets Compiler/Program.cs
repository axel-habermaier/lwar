using System;

namespace Pegasus.AssetsCompiler
{
	/// <summary>
	///   Runs the asset compiler.
	/// </summary>
	public static class Program
	{
		/// <summary>
		///   The entry point of the asset compiler.
		/// </summary>
		/// <param name="args">The command line arguments passed to the asset compiler.</param>
		private static void Main(string[] args)
		{
			var compiler = new Compiler();
			compiler.Compile();
		}
	}
}