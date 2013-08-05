using System;

namespace Pegasus.Framework.Platform.Logging
{
	using System.Collections.Generic;
	using System.IO;
	using Memory;
	using Rendering.UserInterface;

	/// <summary>
	///   Captures all generated logs and outputs them to a log file.
	/// </summary>
	internal class LogFile : DisposableObject
	{
		/// <summary>
		///   The number of log messages that must be queued before the messages are written to the file system.
		/// </summary>
		private const int BatchSize = 250;

		/// <summary>
		///   The file the log is written to.
		/// </summary>
		private readonly AppFile _file;

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
			Log.OnFatalError += _logEntries.Enqueue;
			Log.OnError += _logEntries.Enqueue;
			Log.OnWarning += _logEntries.Enqueue;
			Log.OnInfo += _logEntries.Enqueue;
			Log.OnDebugInfo += _logEntries.Enqueue;

			_file = new AppFile(appName, String.Format("{0}.log", appName));
			_file.Delete(e => Log.Warn("Failed to delete the current contents of the log file: {0}", e.Message));
		}

		/// <summary>
		///   Gets the path of the log file.
		/// </summary>
		public string FilePath
		{
			get { return _file.AbsolutePath; }
		}

		/// <summary>
		///   Writes the generated log messages into the log file.
		/// </summary>
		/// <param name="force">If true, all unwritten messages are written; otherwise, writes are batched to improve performance.</param>
		public void WriteToFile(bool force = false)
		{
			if (!force && _logEntries.Count < BatchSize)
				return;

			if (_file.Append(WriteQueuedEntries, e => Log.Warn("Failed to append to log file: {0}", e.Message)))
				_logEntries.Clear();
		}

		/// <summary>
		///   Writes the queued log entries to the given stream.
		/// </summary>
		/// <param name="writer">The stream the entry should be written to.</param>
		private void WriteQueuedEntries(TextWriter writer)
		{
			Assert.ArgumentNotNull(writer);

			foreach (var entry in _logEntries)
			{
				writer.Write("[");
				writer.Write(entry.LogType.ToDisplayString());
				writer.Write("]   ");
				writer.Write(entry.Time.ToString("HH:mm:ss.ffff"));

				writer.Write("   ");
				Text.Write(writer, entry.Message);
				writer.WriteLine();
			}
		}

		/// <summary>
		///   Copies all generated log entries to the console. This method should only be invoked once in order to show all log
		///   entries on the console that have been generated before the console is initialized.
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
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Log.OnFatalError -= _logEntries.Enqueue;
			Log.OnError -= _logEntries.Enqueue;
			Log.OnWarning -= _logEntries.Enqueue;
			Log.OnInfo -= _logEntries.Enqueue;
			Log.OnDebugInfo -= _logEntries.Enqueue;

			WriteToFile(true);
		}
	}
}