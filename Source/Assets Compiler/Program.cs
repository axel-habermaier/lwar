using System;

namespace Pegasus.AssetsCompiler
{
	using System.Diagnostics;
	using System.Globalization;
	using System.Threading;
	using System.Threading.Tasks;
	using Framework;
	using Framework.Platform;
	using Framework.Scripting;

	/// <summary>
	///   Executes the asset compiler.
	/// </summary>
	public static class Program
	{
		/// <summary>
		///   Wires up the events to write all logged messages to the console.
		/// </summary>
		private static void PrintToConsole()
		{
			Log.OnFatalError += message => WriteToError(ConsoleColor.Red, message);
			Log.OnError += message => WriteToError(ConsoleColor.Red, message);
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
			WriteColored(color, () => Console.WriteLine(message));
		}

		/// <summary>
		///   Writes a colored message to the error output.
		/// </summary>
		/// <param name="color">The color of the message.</param>
		/// <param name="message">The message that should be written to the console.</param>
		private static void WriteToError(ConsoleColor color, string message)
		{
			WriteColored(color, () => Console.Error.WriteLine(message));
		}

		/// <summary>
		///   Writes a colored message to the console, ensuring that the color is reset afterwards.
		/// </summary>
		/// <param name="color">The color of the message.</param>
		/// <param name="action">Writes the message to the console.</param>
		private static void WriteColored(ConsoleColor color, Action action)
		{
			var currentColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			action();
			Console.ForegroundColor = currentColor;
		}

		/// <summary>
		///   Compiles, recompiles, or cleans the assets.
		/// </summary>
		/// <param name="args"></param>
		public static int Main(string[] args)
		{
			TaskScheduler.UnobservedTaskException += (o, e) => { throw e.Exception.InnerException; };
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

			var watch = new Stopwatch();
			watch.Start();

			try
			{
				PrintToConsole();
				Log.Info("Pegasus Asset Compiler ({0} x{1})", PlatformInfo.Platform, IntPtr.Size == 4 ? "86" : "64");

				Console.WriteLine();
				var command = args.Length == 1 ? args[0].Trim().ToLower() : String.Empty;
				var recompile = command == "recompile";
				var compile = command == "compile";
				var clean = command == "clean";

				if (!recompile && !clean && !compile)
				{
					Log.Error("The asset compiler must be invoked with exactly one argument: 'clean', 'compile', or 'recompile'.");
					return -1;
				}

				if (recompile)
				{
					compile = true;
					clean = true;
				}

				Configuration.CheckFxcAvailability();

				var success = true;
				using (var compilationUnit = new CompilationUnit())
				{
					if (clean)
						compilationUnit.Clean();

					if (compile)
						success = compilationUnit.Compile();

					var elapsedSeconds = watch.ElapsedMilliseconds / 1000.0;

					if (clean && !(recompile || compile))
						Log.Info("Done.");
					else
					{
						Console.WriteLine();
						Log.Info("Asset compilation completed ({0:F2}s).", elapsedSeconds.ToString(CultureInfo.InvariantCulture));
					}
				}

				return success ? 0 : -1;
			}
			catch (AggregateException e)
			{
				foreach (var exception in e.InnerExceptions)
					Log.Error(exception.Message);

				Log.Error(e.StackTrace);
				return -1;
			}
			catch (Exception e)
			{
				Log.Error(e.Message);
				Log.Error(e.StackTrace);
				return -1;
			}
		}
	}
}