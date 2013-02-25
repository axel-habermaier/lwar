using System;

namespace Pegasus.Framework
{
	using System.Globalization;
	using System.Threading;
	using System.Threading.Tasks;
	using Platform;
	using Platform.Assets.Compilation;
	using Scripting;

	/// <summary>
	///   Starts up the application and handles command line arguments and fatal application exceptions.
	/// </summary>
	/// <typeparam name="TApp">The type of the application that should be run.</typeparam>
	public abstract class Bootstrapper<TApp>
		where TApp : App, new()
	{
		/// <summary>
		///   Stores the command line arguments that have been passed to the application.
		/// </summary>
		private CommandLine _commandLine;

		/// <summary>
		///   Gets the name of the application.
		/// </summary>
		protected abstract string AppName { get; }

		/// <summary>
		///   Runs the application. This method does not return until the application is shut down.
		/// </summary>
		/// <param name="logFile">The log file that writes all generated log entries to the file system.</param>
		private void RunApplication(LogFile logFile)
		{
			foreach (var cvar in _commandLine.SetCvars)
				cvar.Execute();

			Log.PrintToConsole();
			using (var app = new TApp())
				app.Run(logFile);
		}

		/// <summary>
		///   Cleans, compiles, or recompiles the assets as requested.
		/// </summary>
		private void ManageAssets()
		{
			var recompile = _commandLine.RecompileAssets || (_commandLine.CleanAssets && _commandLine.CompileAssets);
			var compile = _commandLine.CompileAssets;
			var clean = _commandLine.CleanAssets;

			if (recompile)
			{
				compile = true;
				clean = true;
			}

			Log.PrintToConsole();
			Console.WriteLine();

			Log.Info("{0} asset management, version {1}.{2}.", Cvars.AppName.Value, Cvars.AppVersionMajor.Value,
					 Cvars.AppVersionMinor.Value);
			Log.Info("Running on {0} {1}bit.", PlatformInfo.Platform, IntPtr.Size == 4 ? "32" : "64");

			var compilationUnit = CompilationUnit.Create();
			if (clean)
				compilationUnit.Clean();

			if (compile)
				compilationUnit.Compile();

			Log.Info("Done.");
		}

		/// <summary>
		///   Runs the application. This method does not return until the application is shut down.
		/// </summary>
		protected void Run()
		{
			Win32.AttachConsole();

			TaskScheduler.UnobservedTaskException += (o, e) => { throw e.Exception.InnerException; };
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

			Cvars.AppName.Value = AppName;
			using (var logFile = new LogFile())
			{
				try
				{
					_commandLine = CommandLine.Parse();
					if (_commandLine.CompileAssets || _commandLine.CleanAssets || _commandLine.RecompileAssets)
						ManageAssets();
					else
						RunApplication(logFile);
				}
				catch (Exception e)
				{
					var message = "The application has been terminated after a fatal error. " +
								  "See the log file for further details.\n\nThe error was: {0}\n\nLog file: {1}";
					message = String.Format(message, e.Message, logFile.FilePath);
					Log.Error(message);
					Log.Error("Stack trace:\n" + e.StackTrace);
					Win32.ShowMessage(Cvars.AppName.Value + " Fatal Error", message);
				}
			}

			Win32.DetachConsole();
		}
	}
}