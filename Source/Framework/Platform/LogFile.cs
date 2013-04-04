using System;

namespace Pegasus.Framework.Platform
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Rendering.UserInterface;

	/// <summary>
	///   Captures all generated logs and outputs them to a log file.
	/// </summary>
	internal class LogFile : DisposableObject
	{
		/// <summary>
		///   The number of log messages that must be queued before the messages are written to the file system.
		/// </summary>
		private const int BatchSize = 100;

		/// <summary>
		///   The the unwritten log entries that have been generated.
		/// </summary>
		private readonly Queue<LogEntry> _logEntries = new Queue<LogEntry>();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="appName">The name of the application.</param>
		public LogFile(string appName)
		{
			Assert.ArgumentNotNullOrWhitespace(appName, () => appName);

			Log.OnFatalError += OnFatalError;
			Log.OnError += OnError;
			Log.OnWarning += OnWarning;
			Log.OnInfo += OnInfo;
			Log.OnDebugInfo += OnDebugInfo;

			FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appName, appName + ".log");
			File.Delete(FilePath);
		}

		/// <summary>
		///   Gets the path of the log file.
		/// </summary>
		public string FilePath { get; private set; }

		/// <summary>
		///   Invoked when a debug information message has been generated.
		/// </summary>
		/// <param name="s">The message that has been logged.</param>
		private void OnDebugInfo(string s)
		{
			_logEntries.Enqueue(new LogEntry(LogType.DebugInfo, s));
		}

		/// <summary>
		///   Invoked when a information message has been generated.
		/// </summary>
		/// <param name="s">The message that has been logged.</param>
		private void OnInfo(string s)
		{
			_logEntries.Enqueue(new LogEntry(LogType.Info, s));
		}

		/// <summary>
		///   Invoked when a warning has been generated.
		/// </summary>
		/// <param name="s">The message that has been logged.</param>
		private void OnWarning(string s)
		{
			_logEntries.Enqueue(new LogEntry(LogType.Warning, s));
		}

		/// <summary>
		///   Invoked when an error has been generated.
		/// </summary>
		/// <param name="s">The message that has been logged.</param>
		private void OnError(string s)
		{
			_logEntries.Enqueue(new LogEntry(LogType.Error, s));
		}

		/// <summary>
		///   Invoked when a fatal error has been generated.
		/// </summary>
		/// <param name="s">The message that has been logged.</param>
		private void OnFatalError(string s)
		{
			_logEntries.Enqueue(new LogEntry(LogType.FatalError, s));
		}

		/// <summary>
		///   Writes the generated log messages into the log file.
		/// </summary>
		/// <param name="force">If true, all unwritten messages are written; otherwise, writes are batched to improve performance.</param>
		public void WriteToFile(bool force = false)
		{
			if (!force && _logEntries.Count < BatchSize)
				return;

			var logs =
				_logEntries.Select(l => String.Format("[{0}] ({1}): {2}", l.Time.ToString("HH:mm:ss.ffff"), l.LogType, l.Message));

			var directory = Path.GetDirectoryName(FilePath);
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);

			File.AppendAllLines(FilePath, logs);
			_logEntries.Clear();
		}

		/// <summary>
		///   Copies all generated log entries to the console. This method should only be invoked once in order to show all log
		///   entries on the console that have been generated before the console is initialized.
		/// </summary>
		public void WriteToConsole(Console console)
		{
			Assert.ArgumentNotNull(console, () => console);

			foreach (var logEntry in _logEntries)
			{
				switch (logEntry.LogType)
				{
					case LogType.FatalError:
						console.ShowError(logEntry.Message);
						break;
					case LogType.Error:
						console.ShowError(logEntry.Message);
						break;
					case LogType.Warning:
						console.ShowWarning(logEntry.Message);
						break;
					case LogType.Info:
						console.ShowInfo(logEntry.Message);
						break;
					case LogType.DebugInfo:
						console.ShowDebugInfo(logEntry.Message);
						break;
					default:
						throw new InvalidOperationException("Unknown log entry type.");
				}
			}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Log.OnFatalError -= OnFatalError;
			Log.OnError -= OnError;
			Log.OnWarning -= OnWarning;
			Log.OnInfo -= OnInfo;
			Log.OnDebugInfo -= OnDebugInfo;

			WriteToFile(true);
		}
	}
}