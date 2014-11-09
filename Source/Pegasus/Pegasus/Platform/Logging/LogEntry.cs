namespace Pegasus.Platform.Logging
{
	using System;
	using System.Diagnostics;
	using Utilities;

	/// <summary>
	///     Represents a log entry with a specific type and message.
	/// </summary>
	public struct LogEntry : IEquatable<LogEntry>
	{
		/// <summary>
		///     Used to measure the time since the start of the application.
		/// </summary>
		private static readonly Stopwatch Stopwatch = new Stopwatch();

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static LogEntry()
		{
			Stopwatch.Start();
		}

		/// <summary>
		///     Initializes a new instance.
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
			Time = Stopwatch.Elapsed.TotalSeconds;
		}

		/// <summary>
		///     Gets the type of the log entry.
		/// </summary>
		public LogType LogType { get; private set; }

		/// <summary>
		///     Gets the message of the log entry.
		/// </summary>
		public string Message { get; private set; }

		/// <summary>
		///     Gets the date and time of the creation of the log entry.
		/// </summary>
		public double Time { get; private set; }

		/// <summary>
		///     Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(LogEntry other)
		{
			return LogType == other.LogType && string.Equals(Message, other.Message) && Time.Equals(other.Time);
		}

		/// <summary>
		///     Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to. </param>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			return obj is LogEntry && Equals((LogEntry)obj);
		}

		/// <summary>
		///     Returns the hash code for this instance.
		/// </summary>
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (int)LogType;
				hashCode = (hashCode * 397) ^ (Message != null ? Message.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ Time.GetHashCode();
				return hashCode;
			}
		}

		/// <summary>
		///     Checks whether the given log entries are equal.
		/// </summary>
		public static bool operator ==(LogEntry left, LogEntry right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///     Checks whether the given log entries are not equal.
		/// </summary>
		public static bool operator !=(LogEntry left, LogEntry right)
		{
			return !left.Equals(right);
		}

		/// <summary>
		///     Raises the appropriate log event for the log entries.
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