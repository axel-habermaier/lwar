namespace lwar.Launchers.WinRT
{
	using System;
	using System.Diagnostics;
	using Lwar;
	using Pegasus.Platform.Logging;
	using Pegasus.Rendering.UserInterface;

	/// <summary>
	///     Launches lwar as a desktop application.
	/// </summary>
	internal static class Program
	{
		/// <summary>
		///     The entry point of the application.
		/// </summary>
		private static void Main(string[] args)
		{
			PrintDebugOutput();
			App.Launch(args);
		}

		/// <summary>
		///     Wires up the log events to write all logged messages to the debug output.
		/// </summary>
		[Conditional("DEBUG")]
		private static void PrintDebugOutput()
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
			//Debug.WriteLine("[{0}]");
			//Debug.Write(entry.LogType.ToDisplayString());
			//Debug.Write("] ");

			//Text.Write(Console.Out, entry.Message);
			//Console.Out.WriteLine();
		}
	}
}