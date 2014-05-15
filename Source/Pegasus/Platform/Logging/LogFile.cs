namespace Pegasus.Platform.Logging
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using Memory;
	using Rendering.UserInterface;
	using Console = Rendering.UserInterface.Console;

	/// <summary>
	///     Captures all generated logs and outputs them to a log file.
	/// </summary>
	internal class LogFile : DisposableObject
	{
		/// <summary>
		///     The number of log messages that must be queued before the messages are written to the file system.
		/// </summary>
		private const int BatchSize = 250;

		/// <summary>
		///     A cached string builder instance used when writing queued log entries.
		/// </summary>
		private readonly StringBuilder _builder = new StringBuilder(1024);

		/// <summary>
		///     The file the log is written to.
		/// </summary>
		private readonly AppFile _file;

		/// <summary>
		///     The the unwritten log entries that have been generated.
		/// </summary>
		private readonly Queue<LogEntry> _logEntries = new Queue<LogEntry>();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="appName">The name of the application.</param>
		public LogFile(string appName)
		{
			Assert.ArgumentNotNullOrWhitespace(appName);

			Log.OnFatalError += Enqueue;
			Log.OnError += Enqueue;
			Log.OnWarning += Enqueue;
			Log.OnInfo += Enqueue;
			Log.OnDebugInfo += Enqueue;

			_file = new AppFile(String.Format("{0}.log", appName));
			_file.Delete(e => Log.Warn("Failed to delete the current contents of the log file: {0}", e.Message));
		}

		/// <summary>
		///     Gets the path of the log file.
		/// </summary>
		public string FilePath
		{
			get { return String.Format("{0}/{1}", FileSystem.UserDirectory, _file.FileName); }
		}

		/// <summary>
		///     Enqueues the given log entry.
		/// </summary>
		/// <param name="entry">The log entry that should be enqueued.</param>
		private void Enqueue(LogEntry entry)
		{
			_logEntries.Enqueue(entry);
			WriteToFile();
		}

		/// <summary>
		///     Writes the generated log messages into the log file.
		/// </summary>
		/// <param name="force">If true, all unwritten messages are written; otherwise, writes are batched to improve performance.</param>
		internal void WriteToFile(bool force = false)
		{
			if (!force && _logEntries.Count < BatchSize)
				return;

			if (_file.Append(GenerateLogEntryString(), e => Log.Warn("Failed to append to log file: {0}", e.Message)))
				_logEntries.Clear();
		}

		/// <summary>
		///     Generates the string for the queued log entries that must be appended to the log file.
		/// </summary>
		private string GenerateLogEntryString()
		{
			_builder.Clear();

			foreach (var entry in _logEntries)
			{
				_builder.Append("[");
				_builder.Append(entry.LogType.ToDisplayString());
				_builder.Append("]   ");
				_builder.Append(entry.Time.ToString("F4").PadLeft(9));

				_builder.Append("   ");
				Text.Write(_builder, entry.Message);
				_builder.Append("\n");
			}

			return _builder.ToString();
		}

		/// <summary>
		///     Copies all generated log entries to the console. This method should only be invoked once in order to show all log
		///     entries on the console that have been generated before the console is initialized.
		/// </summary>
		public void WriteToConsole(Console console)
		{
			Assert.ArgumentNotNull(console);

			foreach (var logEntry in _logEntries)
			{
				switch (logEntry.LogType)
				{
					case LogType.Fatal:
						console.ShowError(logEntry);
						break;
					case LogType.Error:
						console.ShowError(logEntry);
						break;
					case LogType.Warning:
						console.ShowWarning(logEntry);
						break;
					case LogType.Info:
						console.ShowInfo(logEntry);
						break;
					case LogType.Debug:
						console.ShowDebugInfo(logEntry);
						break;
					default:
						throw new InvalidOperationException("Unknown log entry type.");
				}
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Log.OnFatalError -= Enqueue;
			Log.OnError -= Enqueue;
			Log.OnWarning -= Enqueue;
			Log.OnInfo -= Enqueue;
			Log.OnDebugInfo -= Enqueue;

			WriteToFile(true);
		}
	}
}