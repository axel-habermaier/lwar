namespace Pegasus.AssetsCompiler
{
	using System;
	using System.Diagnostics;
	using System.Globalization;
	using System.Threading;
	using System.Threading.Tasks;
	using Framework;
	using Platform;
	using Platform.Logging;

	/// <summary>
	///     Executes the asset compiler.
	/// </summary>
	public static class Program
	{
		/// <summary>
		///     Compiles, recompiles, or cleans the assets.
		/// </summary>
		/// <param name="args">The command line arguments passed by the user.</param>
		[STAThread]
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
				var command = args.Length >= 1 ? args[0].Trim().ToLower() : String.Empty;
				var recompile = command == "recompile";
				var compile = command == "compile";
				var clean = command == "clean";
				var ui = args.Length >= 4 && args[3].Trim().ToLower() == "/ui";
				var project = args.Length >= 3 ? args[2].Trim() : String.Empty;

				var fileId = 0;
				if (args.Length >= 2 && !Int32.TryParse(args[1].Trim(), out fileId))
					fileId = -1;

				if (String.IsNullOrWhiteSpace(project) || (!recompile && !clean && !compile) || fileId == -1)
				{
					Log.Error("The asset compiler must be invoked with the following arguments: the 'clean', 'compile', or " +
							  "'recompile' command followed by the non-negative unique file identifier and the path " +
							  " to the assets project that should be compiled.");
					return -1;
				}

				Configuration.XamlFilesOnly = ui;
				Configuration.AssetsProjectPath = project;
				Configuration.UniqueFileIdentifier = fileId;

				if (recompile)
				{
					compile = true;
					clean = true;
				}

				if (!Configuration.XamlFilesOnly)
					Configuration.CheckFxcAvailability();

				var success = true;
				using (var compilationUnit = new CompilationUnit())
				{
					compilationUnit.LoadAssets();

					if (!Configuration.XamlFilesOnly && (clean || !Configuration.CheckAssetFileVersion()))
						compilationUnit.Clean();

					if (compile)
						success = compilationUnit.Compile();

					var elapsedSeconds = watch.ElapsedMilliseconds / 1000.0;

					if (clean && !(recompile || compile))
						Log.Info("Done.");
					else
					{
						if (success)
							Configuration.StoreAssetFileVersion();

						Console.WriteLine();
						Log.Info("Asset compilation completed ({0:F2}s).", elapsedSeconds);
					}
				}

				return success ? 0 : -1;
			}
			catch (AggregateException e)
			{
				foreach (var exception in e.InnerExceptions)
					Log.Error("{0}", exception.Message);

				Log.Error("{0}", e.StackTrace);
				Log.Info(Environment.NewLine);
				return -1;
			}
			catch (PegasusException)
			{
				return -1;
			}
			catch (Exception e)
			{
				Log.Error("{0}", e.Message);
				Log.Error("{0}", e.StackTrace);

				if (e.InnerException != null)
				{
					Log.Error("Inner exception:");
					Log.Error("{0}", e.InnerException.Message);
					Log.Error("{0}", e.InnerException.StackTrace);
				}

				return -1;
			}
		}

		/// <summary>
		///     Wires up the events to write all logged messages to the console.
		/// </summary>
		private static void PrintToConsole()
		{
			Log.OnFatalError += entry => WriteToError(ConsoleColor.Red, entry.Message);
			Log.OnError += entry => WriteToError(ConsoleColor.Red, entry.Message);
			Log.OnWarning += entry => WriteToConsole(ConsoleColor.Yellow, entry.Message);
			Log.OnInfo += entry => WriteToConsole(ConsoleColor.White, entry.Message);
		}

		/// <summary>
		///     Writes a colored message to the console.
		/// </summary>
		/// <param name="color">The color of the message.</param>
		/// <param name="message">The message that should be written to the console.</param>
		private static void WriteToConsole(ConsoleColor color, string message)
		{
			WriteColored(color, () => Console.WriteLine(message));
			Debug.WriteLine(message);
		}

		/// <summary>
		///     Writes a colored message to the error output.
		/// </summary>
		/// <param name="color">The color of the message.</param>
		/// <param name="message">The message that should be written to the console.</param>
		private static void WriteToError(ConsoleColor color, string message)
		{
			WriteColored(color, () => Console.Error.WriteLine(message));
			Debug.WriteLine(message);
		}

		/// <summary>
		///     Writes a colored message to the console, ensuring that the color is reset.
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
	}
}