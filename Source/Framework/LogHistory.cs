using System;

namespace Pegasus.Framework
{
	using System.Collections.Generic;

	/// <summary>
	///   Keeps a list of all errors, warnings, informational messages, and debug messages that
	///   have been generated.
	/// </summary>
	internal class LogHistory : DisposableObject
	{
		/// <summary>
		///   Describes the type of a log entry.
		/// </summary>
		public enum LogType
		{
			Error,
			Warning,
			Info,
			DebugInfo
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public LogHistory()
		{
			LogEntries = new List<LogEntry>();

			Log.OnError += OnError;
			Log.OnWarning += OnWarning;
			Log.OnInfo += OnInfo;
			Log.OnDebugInfo += OnDebugInfo;
		}

		/// <summary>
		///   Gets the log entries that have been generated.
		/// </summary>
		public List<LogEntry> LogEntries { get; private set; }

		/// <summary>
		///   Invoked when a debug information message has been generated.
		/// </summary>
		/// <param name="s">The message that has been logged.</param>
		private void OnDebugInfo(string s)
		{
			LogEntries.Add(new LogEntry(LogType.DebugInfo, s));
		}

		/// <summary>
		///   Invoked when a information message has been generated.
		/// </summary>
		/// <param name="s">The message that has been logged.</param>
		private void OnInfo(string s)
		{
			LogEntries.Add(new LogEntry(LogType.Info, s));
		}

		/// <summary>
		///   Invoked when a warning has been generated.
		/// </summary>
		/// <param name="s">The message that has been logged.</param>
		private void OnWarning(string s)
		{
			LogEntries.Add(new LogEntry(LogType.Warning, s));
		}

		/// <summary>
		///   Invoked when an error has been generated.
		/// </summary>
		/// <param name="s">The message that has been logged.</param>
		private void OnError(string s)
		{
			LogEntries.Add(new LogEntry(LogType.Error, s));
		}

		/// <summary>
		///   Stops the recording of the log history.
		/// </summary>
		public void StopRecording()
		{
			Log.OnError -= OnError;
			Log.OnWarning -= OnWarning;
			Log.OnInfo -= OnInfo;
			Log.OnDebugInfo -= OnDebugInfo;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			StopRecording();
		}

		/// <summary>
		///   Represents a log entry with a specific type and message.
		/// </summary>
		public struct LogEntry
		{
			/// <summary>
			///   The type of the log entry.
			/// </summary>
			public LogType LogType;

			/// <summary>
			///   The message of the log entry.
			/// </summary>
			public string Message;

			/// <summary>
			///   Initializes a new instance.
			/// </summary>
			/// <param name="logType">The type of the log entry.</param>
			/// <param name="message">The message of the log entry.</param>
			public LogEntry(LogType logType, string message)
			{
				LogType = logType;
				Message = message;
			}
		}
	}
}