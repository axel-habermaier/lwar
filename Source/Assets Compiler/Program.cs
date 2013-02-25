using System;

namespace Pegasus.AssetsCompiler
{
	using Framework;
	using Framework.Platform;
	using Framework.Scripting;

	/// <summary>
	///   Executes the asset compiler.
	/// </summary>
	public static class Program
	{
		/// <summary>
		///   Compiles, recompiles, or cleans the assets.
		/// </summary>
		/// <param name="args"></param>
		public static void Main(string[] args)
		{
			try
			{
				Log.PrintToConsoleColored(true);
				Log.Info("Pegasus Asset Compiler, version {1}.{2} ({3} x{4})", Cvars.AppName.Value, Cvars.AppVersionMajor.Value,
						 Cvars.AppVersionMinor.Value, PlatformInfo.Platform, IntPtr.Size == 4 ? "86" : "64");

				var command = args.Length == 1 ? args[0].Trim().ToLower() : String.Empty;
				var recompile = command == "recompile";
				var compile = command == "compile";
				var clean = command == "clean";

				if (!recompile && !clean && !compile)
					Log.Error("The asset compiler must be invoked with exactly one argument: 'clean', 'compile', or 'recompile'.");
				else
				{
					if (recompile)
					{
						compile = true;
						clean = true;
					}

					var compilationUnit = CompilationUnit.Create();
					if (clean)
						compilationUnit.Clean();

					if (compile)
						compilationUnit.Compile();
				}
			}
			finally
			{
				Log.Info("Done.");
			}
		}
	}
}