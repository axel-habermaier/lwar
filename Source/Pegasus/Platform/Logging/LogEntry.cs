using System;

namespace Pegasus.Platform.Logging
{
	using System.Diagnostics;

	/// <summary>
	///   Represents a log entry with a specific type and message.
	/// </summary>
	public struct LogEntry
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="logType">The type of the log entry.</param>
		/// <param name="message">The message of the log entry.</param>
		public LogEntry(LogType logType, string message)
			: this()
		{
			Assert.InRange(logType);
			Assert.ArgumentNotNullOrWhitespace(message);

			LogType = logType;
			Message = message;
			Time = DateTime.Now;
		}

		/// <summary>
		///   Gets the type of the log entry.
		/// </summary>
		public LogType LogType { get; private set; }

		/// <summary>
		///   Gets the message of the log entry.
		/// </summary>
		public string Message { get; private set; }

		/// <summary>
		///   Gets the date and time of the creation of the log entry.
		/// </summary>
		public DateTime Time { get; private set; }

		/// <summary>
		///   Raises the appropriate log event for the log entries.
		/// </summary>
		[DebuggerHidden]
		public void RaiseLogEvent()
		{
			switch (LogType)
			{
				case LogType.Fatal:
					Log.Die("{0}", Message);
					break;
				case LogType.Error:
					Log.Error("{0}", Message);
					break;
				case LogType.Warning:
					Log.Warn("{0}", Message);
					break;
				case LogType.Info:
					Log.Info("{0}", Message);
					break;
				case LogType.Debug:
					Log.Debug("{0}", Message);
					break;
				default:
					throw new InvalidOperationException("Unknown log entry type.");
			}
		}
	}
}