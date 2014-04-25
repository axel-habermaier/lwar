namespace Lwar
{
	using System;
	using Pegasus.Framework;
	using Pegasus.Platform.Logging;
	using Pegasus.Rendering.UserInterface;
	using Scripting;

	/// <summary>
	///     Starts up and configures the application.
	/// </summary>
	internal static class Program
	{
		/// <summary>
		///     The entry point of the application.
		/// </summary>
		private static void Main(string[] args)
		{
			PrintToConsole();

			Commands.Initialize();
			Cvars.Initialize();

			Bootstrapper<App>.Run(args, "lwar");
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

			Text.Write(Console.Out, entry.Message);
			Console.Out.WriteLine();
		}
	}
}