using System;

namespace Pegasus.AssetsCompiler
{
	using System.Diagnostics;
	using System.Globalization;
	using System.Linq;
	using Framework;
	using Framework.Platform;
	using Framework.Scripting;

	/// <summary>
	///   Executes the asset compiler.
	/// </summary>
	public static class Program
	{
		/// <summary>
		///   The object used for thread synchronization.
		/// </summary>
		private static readonly object SyncObject = new object();

		/// <summary>
		///   Wires up the events to write all logged messages to the console.
		/// </summary>
		private static void PrintToConsole()
		{
			Log.OnFatalError += message => WriteToConsole(ConsoleColor.Red, message);
			Log.OnError += message => WriteToConsole(ConsoleColor.Red, message);
			Log.OnWarning += message => WriteToConsole(ConsoleColor.Yellow, message);
			Log.OnInfo += message => WriteToConsole(ConsoleColor.White, message);
		}

		/// <summary>
		///   Writes a colored message to the console.
		/// </summary>
		/// <param name="color">The color of the message.</param>
		/// <param name="message">The message that should be written to the console.</param>
		private static void WriteToConsole(ConsoleColor color, string message)
		{
			lock (SyncObject)
			{
				Console.ForegroundColor = color;
				Console.WriteLine(message);
			}
		}

		/// <summary>
		///   Compiles, recompiles, or cleans the assets.
		/// </summary>
		/// <param name="args"></param>
		public static void Main(string[] args)
		{
			var watch = new Stopwatch();
			watch.Start();

			try
			{
				PrintToConsole();
				Log.Info("Pegasus Asset Compiler, version {1}.{2} ({3} x{4})", Cvars.AppName.Value, Cvars.AppVersionMajor.Value,
						 Cvars.AppVersionMinor.Value, PlatformInfo.Platform, IntPtr.Size == 4 ? "86" : "64");

				Console.WriteLine();
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
			catch (AggregateException e)
			{
				Log.Error(e.InnerExceptions.First().Message);
			}
			catch (Exception e)
			{
				Log.Error(e.Message);
			}
			finally
			{
				var elapsedSeconds = watch.ElapsedMilliseconds / 1000.0;

				Console.WriteLine();
				Log.Info("Asset compilation completed ({0:F2}s).", elapsedSeconds.ToString(CultureInfo.InvariantCulture));
			}
		}
	}
}