using System;

namespace Pegasus.Framework.Platform
{
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Diagnostics;

	/// <summary>
	///   Starts and manages an external process.
	/// </summary>
	public static class ExternalProcess
	{
		/// <summary>
		///   Runs an external tool process.
		/// </summary>
		/// <param name="fileName">The file name of the external tool executable.</param>
		/// <param name="commandLine">The command line arguments that should be passed to the tool.</param>
		/// <param name="arguments">The arguments that should be copied into the command line.</param>
		public static IEnumerable<LogEntry> Run(string fileName, string commandLine = "", params object[] arguments)
		{
			Assert.ArgumentNotNullOrWhitespace(fileName, () => fileName);
			Assert.ArgumentNotNull(commandLine, () => commandLine);

			var process = new Process
			{
				EnableRaisingEvents = true,
				StartInfo = new ProcessStartInfo(fileName, String.Format(commandLine, arguments))
				{
					UseShellExecute = false,
					RedirectStandardError = true,
					RedirectStandardInput = true,
					RedirectStandardOutput = true
				}
			};

			var logEntries = new ConcurrentQueue<LogEntry>();
			process.OutputDataReceived += (o, e) =>
				{
					if (!String.IsNullOrWhiteSpace(e.Data))
						logEntries.Enqueue(new LogEntry(LogType.Info, e.Data));
				};
			process.ErrorDataReceived += (o, e) =>
				{
					if (!String.IsNullOrWhiteSpace(e.Data))
						logEntries.Enqueue(new LogEntry(LogType.Error, e.Data));
				};

			process.Start();

			process.BeginErrorReadLine();
			process.BeginOutputReadLine();
			process.WaitForExit();

			return logEntries;
		}
	}
}