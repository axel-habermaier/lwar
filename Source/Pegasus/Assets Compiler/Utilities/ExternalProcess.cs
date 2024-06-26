﻿namespace Pegasus.AssetsCompiler.Utilities
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading.Tasks;

	/// <summary>
	///     Represents external process.
	/// </summary>
	internal class ExternalProcess : IDisposable
	{
		/// <summary>
		///     The external process.
		/// </summary>
		private readonly Process _process;

		/// <summary>
		///     The log entries generated during the execution of the process.
		/// </summary>
		private ConcurrentQueue<LogEntry> _logEntries;

		/// <summary>
		///     Indicates whether the process is currently running.
		/// </summary>
		private bool _running;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="fileName">The file name of the external executable.</param>
		/// <param name="commandLine">The command line arguments that should be passed to the executable.</param>
		/// <param name="arguments">The arguments that should be copied into the command line.</param>
		public ExternalProcess(string fileName, string commandLine = "", params object[] arguments)
		{
			Assert.ArgumentNotNullOrWhitespace(fileName);
			Assert.ArgumentNotNull(commandLine);

			_process = new Process
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

			_process.OutputDataReceived += (o, e) => LogMessage(LogType.Info, e.Data);
			_process.ErrorDataReceived += (o, e) => LogMessage(LogType.Error, e.Data);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			_process.Dispose();
		}

		/// <summary>
		///     Adds the message to the log entry queue.
		/// </summary>
		/// <param name="type">The type of the log entry.</param>
		/// <param name="message">The message that should be added.</param>
		private void LogMessage(LogType type, string message)
		{
			if (String.IsNullOrWhiteSpace(message))
				return;

			_logEntries.Enqueue(new LogEntry(type, String.Format("{0}: {1}", _process.StartInfo.FileName, message)));
		}

		/// <summary>
		///     Runs the process.
		/// </summary>
		public IEnumerable<LogEntry> Run()
		{
			int exitCode;
			return Run(out exitCode);
		}

		/// <summary>
		///     Runs the process.
		/// </summary>
		/// <param name="exitCode">Returns the exit code of the process.</param>
		public IEnumerable<LogEntry> Run(out int exitCode)
		{
			Assert.That(!_running, "The process is already running.");

			_running = true;
			try
			{
				_logEntries = new ConcurrentQueue<LogEntry>();
				_process.Start();

				_process.BeginErrorReadLine();
				_process.BeginOutputReadLine();

				_process.WaitForExit();
				exitCode = _process.ExitCode;

				return _logEntries;
			}
			finally
			{
				_running = false;
			}
		}

		/// <summary>
		///     Asynchronously runs the process.
		/// </summary>
		public Task<IEnumerable<LogEntry>> RunAsync()
		{
			Assert.That(!_running, "The process is already running.");

			_running = true;
			try
			{
				_logEntries = new ConcurrentQueue<LogEntry>();
				var tcs = new TaskCompletionSource<IEnumerable<LogEntry>>();

				_process.Exited += (o, e) => tcs.SetResult(_logEntries);
				_process.Start();

				_process.BeginErrorReadLine();
				_process.BeginOutputReadLine();

				return tcs.Task;
			}
			finally
			{
				_running = false;
			}
		}
	}
}