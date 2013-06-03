using System;

namespace Pegasus.Framework
{
	using System.Globalization;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Platform;
	using Platform.Logging;
	using Rendering;
	using Scripting;
	using Scripting.Parsing;

	/// <summary>
	///   Starts up the application and handles command line arguments and fatal application exceptions.
	/// </summary>
	/// <typeparam name="TApp">The type of the application that should be run.</typeparam>
	public static class Bootstrapper<TApp>
		where TApp : App, new()
	{
		/// <summary>
		///   Runs the application. This method does not return until the application is shut down.
		/// </summary>
		/// <param name="appName">The name of the application.</param>
		/// <param name="defaultFontName">The name of the default font that is used to draw the console and the statistics.</param>
		/// <param name="spriteEffect">The sprite effect that should be used to draw the console and the statistics.</param>
		public static void Run(string appName, string defaultFontName, ISpriteEffect spriteEffect)
		{
			Assert.ArgumentNotNullOrWhitespace(appName);
			Assert.ArgumentNotNullOrWhitespace(defaultFontName);
			Assert.ArgumentNotNull(spriteEffect);

			TaskScheduler.UnobservedTaskException += (o, e) => { throw e.Exception.InnerException; };
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

			using (var logFile = new LogFile(appName))
			{
				try
				{
					PrintToConsole();

					Log.Info("Starting {0} ({1} x{2}, {3}).", appName, PlatformInfo.Platform, IntPtr.Size == 4 ? "32" : "64", PlatformInfo.GraphicsApi);

					Commands.Initialize();
					Cvars.Initialize();

					using (new Help())
					using (new Interpreter(appName))
					{
						Commands.Process(ConfigurationFile.AutoExec);
						ParseCommandLine();

						var app = new TApp();
						app.Run(logFile, appName, defaultFontName, spriteEffect);

						Commands.Persist(ConfigurationFile.AutoExec);
					}

					Log.Info("{0} has shut down.", appName);
				}
				catch (Exception e)
				{
					var message = "The application has been terminated after a fatal error. " +
								  "See the log file for further details.\n\nThe error was: {0}\n\nLog file: {1}";
					message = String.Format(message, e.Message, logFile.FilePath);
					Log.Error("{0}", message);
					Log.Error("Stack trace:\n{0}", e.StackTrace);
					Win32.ShowMessage(appName + " Fatal Error", message);
				}
			}
		}

		/// <summary>
		///   Parses the command line and sets all cvars to the requested values.
		/// </summary>
		private static void ParseCommandLine()
		{
			Log.Info("Parsing the command line arguments '{0}'...", Environment.CommandLine);
			var reply = new CommandLineParser().Parse(Environment.CommandLine);
			if (reply.Status != ReplyStatus.Success)
			{
				Log.Error("{0}", reply.Errors.ErrorMessage);
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
			Log.OnFatalError += entry => Console.WriteLine("[{1}] FATAL:   {0}", entry.Message, entry.Category.ToDisplayString());
			Log.OnError += entry => Console.WriteLine("[{1}] ERROR:   {0}", entry.Message, entry.Category.ToDisplayString());
			Log.OnWarning += entry => Console.WriteLine("[{1}] WARNING: {0}", entry.Message, entry.Category.ToDisplayString());
			Log.OnInfo += entry => Console.WriteLine("[{1}] INFO:    {0}", entry.Message, entry.Category.ToDisplayString());
			Log.OnDebugInfo += entry => Console.WriteLine("[{1}] DEBUG:   {0}", entry.Message, entry.Category.ToDisplayString());
		}
	}
}