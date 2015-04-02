namespace Pegasus
{
	using System;
	using System.Diagnostics;
	using System.Globalization;
	using System.Reflection;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;
	using Platform;
	using Platform.Logging;
	using Platform.Memory;
	using Scripting;
	using UserInterface;
	using UserInterface.ViewModels;
	using Utilities;

	/// <summary>
	///     Starts up the application and handles command line arguments and fatal application exceptions.
	/// </summary>
	public static class Bootstrapper
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static Bootstrapper()
		{
			MainThreadId = Thread.CurrentThread.ManagedThreadId;

			// Set the thread name if it hasn't been set already by the test runner
			if (Thread.CurrentThread.Name == null)
				Thread.CurrentThread.Name = "Main Thread";

			TaskScheduler.UnobservedTaskException += (o, e) => { throw e.Exception.InnerException; };
			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
		}

		/// <summary>
		///     Gets the identifier of the main thread input handling and rendering has to be performed on.
		/// </summary>
		internal static int MainThreadId { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the bootstrapping process is completed.
		/// </summary>
		internal static bool Completed { get; private set; }

		/// <summary>
		///     Runs the application. This method does not return until the application is shut down.
		/// </summary>
		/// <typeparam name="TApp">The type of the application that should be run.</typeparam>
		/// <param name="arguments">The command line arguments that have been passed to the application.</param>
		/// <param name="appName">The name of the application.</param>
		public static void Run<TApp>(string[] arguments, string appName)
			where TApp : Application, new()
		{
			Assert.ArgumentNotNullOrWhitespace(appName);

			try
			{
				AssemblyCache.Register(typeof(TApp).GetTypeInfo().Assembly);
				AssemblyCache.Register(typeof(Bootstrapper).GetTypeInfo().Assembly);

				// Start printing to the console and initialize the file system
				FileSystem.SetAppDirectory(appName);
				PrintToConsole();

				// Initialize the commands and cvars
				Commands.Initialize();
				Cvars.Initialize();

				// Initialize the console view model here, as we don't want to miss
				// any log entries while the application initializes itself...
				using (var consoleViewModel = new ConsoleViewModel())
				using (new PlatformLibrary())
				using (var logFile = new LogFile(appName))
				{
					try
					{
						// Setup restart handling: when the app should restart itself, set the restart flag and
						// invoke the exit command to trigger a restart
						var restart = true;
						Commands.OnRestart += () =>
						{
							restart = true;
							Commands.Exit();
						};

						using (new Help())
						using (new Interpreter())
						{
							// Process the autoexec.cfg first, then the command line, so that cvar values set via the command line overwrite
							// the autoexec.cfg. Afterwards, perform all deferred updates so that all cvars are set to their updated values
							Commands.Process(ConfigurationFile.AutoExec);
							CommandLineParser.Parse(arguments);

							// We're done with the bootstrapping process, now let's finally run the app!
							Completed = true;

							while (restart)
							{
								// Make sure we don't restart again, unless explicitly requested; also, apply all
								// deferred cvar updates, as that might very well be the point of restarting the app...
								restart = false;
								CvarRegistry.ExecuteDeferredUpdates();

								Log.Info("Starting {0}...", appName);
								var application = new TApp { Name = appName };
								application.Run(consoleViewModel);
							}

							Commands.Persist(ConfigurationFile.AutoExec);
						}

						Log.Info("{0} has shut down.", appName);
					}
					catch (TargetInvocationException e)
					{
						ReportException(e.InnerException, logFile, appName);
					}
					catch (Exception e)
					{
						ReportException(e, logFile, appName);
					}
				}
			}
			catch (Exception e)
			{
				var message = String.Format("Application startup failed. {0} ({1})", e.Message, e.GetType().FullName);
				MessageBox.ShowNativeError(appName + " Fatal Error", message);
				Console.WriteLine(message);
			}
			finally
			{
				ObjectPool.DisposeGlobalPools();
			}
		}

		/// <summary>
		///     Reports the given exception.
		/// </summary>
		/// <param name="exception">The exception that should be reported.</param>
		/// <param name="logFile">The log file the exception should be reported to.</param>
		/// <param name="appName">The name of the application.</param>
		private static void ReportException(Exception exception, LogFile logFile, string appName)
		{
			var message = "The application has been terminated after a fatal error. " +
						  "See the log file for further details.\n\nThe error was: {0}\n\nLog file: {1}";

			logFile.Enqueue(new LogEntry(LogType.Error, String.Format("Exception type: {0}", exception.GetType().FullName)));
			logFile.Enqueue(new LogEntry(LogType.Error, String.Format("Exception message: {0}", exception.Message)));
			logFile.Enqueue(new LogEntry(LogType.Error, String.Format("Stack trace:\n{0}", exception.StackTrace)));
			logFile.WriteToFile(force: true);

			message = String.Format(message, exception.Message, logFile.FilePath);
			MessageBox.ShowNativeError(appName + " Fatal Error", message);

			if (!(exception is PegasusException))
			{
				Log.Error("Exception type: {0}", exception.GetType().FullName);
				Log.Error("Exception message: {0}", exception.Message);
			}

			Log.Error("The application has been terminated after a fatal error. The log file is located at '{0}'.", logFile.FilePath);
		}

		/// <summary>
		///     Wires up the log events to write all logged messages to the console.
		/// </summary>
		private static void PrintToConsole()
		{
			Log.OnFatalError += WriteToConsole;
			Log.OnError += WriteToConsole;
			Log.OnWarning += WriteToConsole;
			Log.OnInfo += WriteToConsole;
			Log.OnDebugInfo += WriteToConsole;
		}

		/// <summary>
		///     Writes the given log entry to the given text writer.
		/// </summary>
		/// <param name="entry">The log entry that should be written.</param>
		private static void WriteToConsole(LogEntry entry)
		{
#if DEBUG
			var builder = new StringBuilder();
			TextString.Write(builder, entry.Message);

			Debug.WriteLine("[{0}] {1}", entry.LogTypeString, builder);
#else
			Console.Out.Write("[");
			Console.Out.Write(entry.LogTypeString);
			Console.Out.Write("] ");

			TextString.Write(Console.Out, entry.Message);
			Console.Out.WriteLine();
#endif
		}
	}
}