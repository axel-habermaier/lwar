namespace Pegasus.AssetsCompiler
{
	using System;
	using System.Diagnostics;
	using System.Globalization;
	using System.Linq;
	using System.Threading;
	using Framework;
	using Platform;
	using Platform.Logging;

	/// <summary>
	///     Compiles, recompiles, or cleans the assets and monitors asset changes.
	/// </summary>
	internal class Program
	{
		/// <summary>
		///     The asset projects compiled by the compiler.
		/// </summary>
		private readonly AssetProject[] _assetProjects;

		/// <summary>
		///     The mode the asset compiler is run in.
		/// </summary>
		private readonly Mode _mode;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="assetProjects">The paths to the asset projects that should be compiled.</param>
		/// <param name="mode">The mode the asset compiler should be run in.</param>
		private Program(string[] assetProjects, Mode mode)
		{
			Assert.ArgumentNotNull(assetProjects);
			Assert.ArgumentInRange(mode);

			_mode = mode;
			_assetProjects = assetProjects.Select(p => new AssetProject(p, mode)).ToArray();
		}

		/// <summary>
		///     Runs the asset compiler, returning true in case of success.
		/// </summary>
		private bool Run()
		{
			if (_mode != Mode.Monitor)
			{
				if (_mode == Mode.Clean || _mode == Mode.Recompile)
				{
					Log.Info("Cleaning compiled assets and temporary files...");

					foreach (var project in _assetProjects)
						project.Clean();

					Log.Info("Done.");
				}

				if (_mode == Mode.Compile || _mode == Mode.Recompile)
				{
					var watch = new Stopwatch();
					watch.Start();

					foreach (var project in _assetProjects)
						project.Compile();

					Console.WriteLine();
					Log.Info("Asset compilation completed ({0:F2}s).", watch.ElapsedMilliseconds / 1000.0);
				}
			}

			return false;
		}

		/// <summary>
		///     The entry point of the asset compiler.
		/// </summary>
		/// <param name="args">The command line arguments passed by the user.</param>
		[STAThread]
		private static int Main(string[] args)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

			try
			{
				PrintToConsole();

				Log.Info("Pegasus Asset Compiler ({0} x{1})", PlatformInfo.Platform, IntPtr.Size == 4 ? "86" : "64");
				Console.WriteLine();

				var assetProjects = args.Skip(1).ToArray();
				var modeArgument = args.Length >= 1 ? args[0].Trim().ToLower() : String.Empty;

				Mode mode;
				if (assetProjects.Length == 0 || !Enum.TryParse(modeArgument, true, out mode))
				{
					Log.Error("The asset compiler must be invoked with the following arguments: the 'monitor', 'clean', 'compile', " +
							  "or 'recompile' command followed by at least one asset project XML file.");
					return -1;
				}

				var program = new Program(assetProjects, mode);
				return program.Run() ? 0 : -1;
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