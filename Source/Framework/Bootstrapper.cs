using System;

namespace Pegasus.Framework
{
	using System.Globalization;
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
			var reply = new CommandLineParser().Parse(Environment.CommandLine);
			if (reply.Status != ReplyStatus.Success)
				Log.Die(reply.Errors.ErrorMessage);
			else
			{
				foreach (var cvar in reply.Result)
					cvar.Execute();
			}
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
					Log.PrintToConsoleUncolored(false);
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