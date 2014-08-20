namespace Pegasus.Platform.Logging
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using Memory;

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
			StringBuilder builder;
			using (StringBuilderPool.Allocate(out builder))
			{
				foreach (var entry in _logEntries)
				{
					builder.Append("[");
					builder.Append(entry.LogType.ToDisplayString());
					builder.Append("]   ");
					builder.Append(entry.Time.ToString("F4").PadLeft(9));

					builder.Append("   ");
					TextString.Write(builder, entry.Message);
					builder.Append("\n");
				}

				return builder.ToString();
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