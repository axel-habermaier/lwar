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
	public static class Bootstrapper<TApp>
		where TApp : App, new()
	{
		/// <summary>
		///   Runs the application. This method does not return until the application is shut down.
		/// </summary>
		/// <param name="context">
		///   The application context that provides the default instances and values that the framework relies on.
		/// </param>
		public static void Run(AppContext context)
		{
			Assert.ArgumentNotNull(context, () => context);
			Assert.ArgumentSatisfies(!String.IsNullOrWhiteSpace(context.AppName), () => context, "The application name has not been set.");
			Assert.ArgumentSatisfies(!String.IsNullOrWhiteSpace(context.DefaultFontName), () => context, "The default font name has not been set.");
			Assert.ArgumentSatisfies(context.Commands != null, () => context, "The command registry has not been set.");
			Assert.ArgumentSatisfies(context.Cvars != null, () => context, "The cvar registry has not been set.");
			Assert.ArgumentSatisfies(context.SpriteEffect != null, () => context, "The sprite effect adapter has not been set.");
			Assert.ArgumentSatisfies(context.Statistics != null, () => context, "The statistics instance adapter has not been set.");
			Assert.ArgumentSatisfies(context.Cvars.AllInstances.Select(cvar => cvar.Name)
											.Concat(context.Commands.AllInstances.Select(command => command.Name))
											.GroupBy(name => name)
											.Where(group => group.Count() > 1)
											.Select(group => group.First())
											.FirstOrDefault() == null, () => context, "There is a cvar and a command with the same name.");

			TaskScheduler.UnobservedTaskException += (o, e) => { throw e.Exception.InnerException; };
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

			using (var logFile = new LogFile(context.AppName))
			{
				try
				{
					PrintToConsole();

					Log.Info("Starting {0} ({1} x{2}, {3}).",
							 context.AppName,
							 PlatformInfo.Platform,
							 IntPtr.Size == 4 ? "32" : "64",
							 PlatformInfo.GraphicsApi);

					using (new Help(context.Commands, context.Cvars))
					using (new Interpreter(context.AppName, context.Commands, context.Cvars))
					{
						context.Commands.Process(ConfigurationFile.AutoExec);
						ParseCommandLine(context.Cvars);

						var app = new TApp();
						app.Run(context, logFile);

						context.Commands.Persist(ConfigurationFile.AutoExec);
					}

					Log.Info("{0} has shut down.", context.AppName);
				}
				catch (Exception e)
				{
					var message = "The application has been terminated after a fatal error. " +
								  "See the log file for further details.\n\nThe error was: {0}\n\nLog file: {1}";
					message = String.Format(message, e.Message, logFile.FilePath);
					Log.Error("{0}", message);
					Log.Error("Stack trace:\n{0}", e.StackTrace);
					Win32.ShowMessage(context.AppName + " Fatal Error", message);
				}
			}
		}

		/// <summary>
		///   Parses the command line and sets all cvars to the requested values.
		/// </summary>
		/// <param name="cvarRegistry">
		///   The cvar registry that should be used to look up cvars referenced by a command line argument.
		/// </param>
		private static void ParseCommandLine(CvarRegistry cvarRegistry)
		{
			Log.Info("Parsing the command line arguments '{0}'...", Environment.CommandLine);
			var reply = new CommandLineParser(cvarRegistry).Parse(Environment.CommandLine);
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
			Log.OnFatalError += message => Console.WriteLine("FATAL ERROR: {0}", message);
			Log.OnError += message => Console.WriteLine("ERROR: {0}", message);
			Log.OnWarning += message => Console.WriteLine("WARNING: {0}", message);
			Log.OnInfo += message => Console.WriteLine("INFO: {0}", message);
			Log.OnDebugInfo += message => Console.WriteLine("DEBUG: {0}", message);
		}
	}
}