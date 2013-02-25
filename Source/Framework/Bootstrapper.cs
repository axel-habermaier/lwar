using System;

namespace Pegasus.Framework
{
	using System.Globalization;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Platform;
	using Scripting;
	using Scripting.Parsing;

	/// <summary>
	///   Starts up the application and handles command line arguments and fatal application exceptions.
	/// </summary>
	/// <typeparam name="TApp">The type of the application that should be run.</typeparam>
	public abstract class Bootstrapper<TApp>
		where TApp : App, new()
	{
		/// <summary>
		///   Gets the name of the application.
		/// </summary>
		protected abstract string AppName { get; }

		/// <summary>
		///   Parses the command line and sets all cvars to the requested values.
		/// </summary>
		public void ParseCommandLine()
		{
			Log.Info("Parsing the command line arguments '{0}'...", Environment.CommandLine);
			var reply = new CommandLineParser().Parse(Environment.CommandLine);
			if (reply.Status != ReplyStatus.Success)
			{
				Log.Error(reply.Errors.ErrorMessage);
				Log.Warn("All cvar values set via the command line were discarded.");
			}
			else
			{
				if (reply.Result.Any())
					Log.Info("Setting cvar values passed via the command line...");
				else
					Log.Info("No cvar values have been provided via the command line.");

				foreach (var cvar in reply.Result)
					cvar.Execute();
			}
		}

		/// <summary>
		///   Wires up the log events to write all logged messages to the console.
		/// </summary>
		private static void PrintToConsole()
		{
			Log.OnFatalError += message => Console.WriteLine("FATAL ERROR: {0}", message);
			Log.OnError += message => Console.WriteLine("ERROR: {0}", message);
			Log.OnWarning += message => Console.WriteLine("WARNING: {0}", message);
			Log.OnInfo += message => Console.WriteLine("INFO: {0}", message);
			Log.OnDebugInfo += message => Console.WriteLine("DEBUG: {0}", message);
		}

		/// <summary>
		///   Runs the application. This method does not return until the application is shut down.
		/// </summary>
		protected void Run()
		{
			TaskScheduler.UnobservedTaskException += (o, e) => { throw e.Exception.InnerException; };
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

			Cvars.AppName.Value = AppName;
			using (var logFile = new LogFile())
			{
				try
				{
					PrintToConsole();

					Log.Info("Starting {0}, version {1}.{2} ({3} x{4}, {5}).",
							 Cvars.AppName.Value,
							 Cvars.AppVersionMajor.Value,
							 Cvars.AppVersionMinor.Value,
							 PlatformInfo.Platform, IntPtr.Size == 4 ? "32" : "64",
							 PlatformInfo.GraphicsApi);

					ParseCommandLine();

					using (var app = new TApp())
						app.Run(logFile);
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
		}
	}
}