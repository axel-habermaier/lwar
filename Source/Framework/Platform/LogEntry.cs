using System;

namespace Pegasus.Framework.Platform
{
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
		public void RaiseLogEvent()
		{
			switch (LogType)
			{
				case LogType.FatalError:
					Log.Die(Message);
					break;
				case LogType.Error:
					Log.Error(Message);
					break;
				case LogType.Warning:
					Log.Warn(Message);
					break;
				case LogType.Info:
					Log.Info(Message);
					break;
				case LogType.DebugInfo:
					Log.DebugInfo(Message);
					break;
				default:
					throw new InvalidOperationException("Unknown log entry type.");
			}
		}
	}
}