namespace Pegasus.Platform.Logging
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using Memory;
	using UserInterface;
	using Utilities;

	/// <summary>
	///     Captures all generated logs and outputs them to a log file.
	/// </summary>
	internal class LogFile : DisposableObject
	{
		/// <summary>
		///     The number of log messages that must be queued before the messages are written to the file system.
		/// </summary>
		private const int BatchSize = 200;

		/// <summary>
		///     The file the log is written to.
		/// </summary>
		private readonly AppFile _file;

		/// <summary>
		///     The unwritten log entries that have been generated.
		/// </summary>
		private readonly Queue<LogEntry> _logEntries = new Queue<LogEntry>();

		/// <summary>
		///     The task that writes the log entries to the file system.
		/// </summary>
		private Task _writeTask;

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

			try
			{
				// Clear the log file, if it already exists...
				_file = new AppFile(String.Format("{0}.log", appName));
				_file.Write("");
			}
			catch (FileSystemException e)
			{
				Log.Warn("Failed to clear the current contents of the log file: {0}", e.Message);
			}
		}

		/// <summary>
		///     Gets the path of the log file.
		/// </summary>
		public string FilePath
		{
			get { return Path.Combine(FileSystem.UserDirectory, _file.FileName).Replace("\\", "/"); }
		}

		/// <summary>
		///     Enqueues the given log entry.
		/// </summary>
		/// <param name="entry">The log entry that should be enqueued.</param>
		internal void Enqueue(LogEntry entry)
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

			if (_logEntries.Count == 0)
				return;

			try
			{
				WaitForCompletion();

				var logEntries = _logEntries.ToArray();
				_logEntries.Clear();

				_writeTask = Task.Run(() => _file.Append(ToString(logEntries)));

				if (force)
					WaitForCompletion();
			}
			catch (Exception e)
			{
				Log.Error("Failed to append to log file: {0}", e.Message);
			}
		}

		/// <summary>
		///     Waits for the completion of the write task. Any exceptions that have been thrown during the execution of the write task
		///     are collected into a new exception.
		/// </summary>
		private void WaitForCompletion()
		{
			if (_writeTask == null)
				return;

			try
			{
				if (!_writeTask.IsCompleted)
					_writeTask.Wait();
				else if (_writeTask.Exception != null)
					throw _writeTask.Exception;
			}
			catch (AggregateException e)
			{
				throw new InvalidOperationException(String.Join("\n", e.InnerExceptions.Select(inner => inner.Message)));
			}
			finally
			{
				_writeTask = null;
			}
		}

		/// <summary>
		///     Generates the string representation for the given log entries.
		/// </summary>
		/// <param name="logEntries">The log entries that should be converted to a string.</param>
		private static string ToString(LogEntry[] logEntries)
		{
			var builder = new StringBuilder();
			foreach (var entry in logEntries)
			{
				builder.Append("[");
				builder.Append(entry.LogTypeString);
				builder.Append("]   ");
				builder.Append(entry.Time.ToString("F4").PadLeft(9));

				builder.Append("   ");
				TextString.Write(builder, entry.Message);
				builder.Append("\n");
			}

			return builder.ToString();
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