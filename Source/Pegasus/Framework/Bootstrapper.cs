namespace Pegasus.Framework
{
	using System;
	using System.Globalization;
	using System.Threading;
	using System.Threading.Tasks;
	using Platform;
	using Platform.Input;
	using Platform.Logging;
	using Scripting;
	using Scripting.Parsing.BasicParsers;
	using Scripting.Parsing.Combinators;
	using UserInterface;
	using UserInterface.Controls;

	/// <summary>
	///     Starts up the application and handles command line arguments and fatal application exceptions.
	/// </summary>
	/// <typeparam name="TApp">The type of the application that should be run.</typeparam>
	public static class Bootstrapper<TApp>
		where TApp : Application, new()
	{
		/// <summary>
		///     Runs the application. This method does not return until the application is shut down.
		/// </summary>
		/// <param name="arguments">The command line arguments that have been passed to the application.</param>
		/// <param name="appName">The name of the application.</param>
		public static void Run(string[] arguments, string appName)
		{
			Assert.ArgumentNotNullOrWhitespace(appName);

			TaskScheduler.UnobservedTaskException += (o, e) => { throw e.Exception.InnerException; };
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

			PrintToConsole();

			using (new NativeLibrary(appName))
			using (var logFile = new LogFile(appName))
			{
				try
				{
					Log.Info("Starting {0} ({1} x{2}, {3}).", appName, PlatformInfo.Platform, IntPtr.Size == 4 ? "32" : "64", PlatformInfo.GraphicsApi);

					InitializeTypeRegistry();
					ReflectionHelper.Validate();
					Commands.Initialize();
					Cvars.Initialize();

					using (new Help())
					using (new Interpreter())
					{
						// Process the autoexec.cfg first, then the command line, so that cvar values set via the command line overwrite
						// the autoexec.cfg. Afterwards, perform all deferred updates so that all cvars are set to their updated values
						Commands.Process(ConfigurationFile.AutoExec);
						CommandLineParser.Parse(arguments);
						CvarRegistry.ExecuteDeferredUpdates();

						var app = new TApp();
						app.Run(appName, logFile);

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

					logFile.WriteToFile(force: true);
					NativeLibrary.ShowMessageBox(appName + " Fatal Error", message);
				}
			}
		}

		/// <summary>
		///     Registers common Pegasus framework types on the type registry.
		/// </summary>
		private static void InitializeTypeRegistry()
		{
			TypeRegistry.Register(new IPAddressParser(), "IPv4 or IPv6 address", null, "::1", "127.0.0.1");
			TypeRegistry.Register(new IPEndPointParser(), "IPv4 or IPv6 address with optional port", null, "::1", "[::1]:8081", "127.0.0.1",
								  "127.0.0.1:8081");
			TypeRegistry.Register(new EnumerationLiteralParser<WindowMode>(ignoreCase: true), null, null);
			TypeRegistry.Register(new Vector2Parser(), null, v => String.Format("{0};{1}", v.X, v.Y), "0;0", "-10.0;10.5");
			TypeRegistry.Register(new Vector2IParser(), null, v => String.Format("{0};{1}", v.X, v.Y), "0;0", "-10;10");
			TypeRegistry.Register(new SizeParser(), null, s => String.Format("{0};{1}", s.Width, s.Height), "0;0", "-10;10");
			TypeRegistry.Register(new InputTriggerParser(), null, i => String.Format("[{0}]", i), "[Key(Return,WentDown)]", "[Key(A,Pressed)]",
								  "[Key(Left,Repeated)]", "[Mouse(Left,Pressed) | Mouse(Right,Pressed)]");
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
			Console.Out.Write("[");
			Console.Out.Write(entry.LogType.ToDisplayString());
			Console.Out.Write("] ");

			TextString.Write(Console.Out, entry.Message);
			Console.Out.WriteLine();
		}
	}
}