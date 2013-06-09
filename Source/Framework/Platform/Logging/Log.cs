using System;

namespace Pegasus.Framework.Platform.Logging
{
	using System.Diagnostics;

	/// <summary>
	///   Provides functions to log fatal errors, errors, warnings, informational messages, and debug-time only
	///   informational messages. An event is raised whenever one of these functions is invoked.
	/// </summary>
	public static class Log
	{
		/// <summary>
		///   Raised when a fatal error occurred. Typically, the program terminates after all event handlers have
		///   been executed.
		/// </summary>
		public static event Action<LogEntry> OnFatalError;

		/// <summary>
		///   Raised when an error occurred.
		/// </summary>
		public static event Action<LogEntry> OnError;

		/// <summary>
		///   Raised when a warning was generated.
		/// </summary>
		public static event Action<LogEntry> OnWarning;

		/// <summary>
		///   Raised when an informational message was generated.
		/// </summary>
		public static event Action<LogEntry> OnInfo;

		/// <summary>
		///   Raised when a debug informational message was generated.
		/// </summary>
		public static event Action<LogEntry> OnDebugInfo;

		/// <summary>
		///   Raises the OnFatalError event with the given message and terminates the application by throwing
		///   an InvalidOperationException.
		/// </summary>
		/// <param name="category">The category of the log entry.</param>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnFatalError event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[DebuggerHidden]
		[StringFormatMethod("message")]
		public static void Die(LogCategory category, string message, params object[] arguments)
		{
			Assert.InRange(category);
			Assert.ArgumentNotNullOrWhitespace(message);

			var formattedMessage = String.Format(message, arguments);
			if (OnFatalError != null)
				OnFatalError(new LogEntry(category, LogType.Fatal, formattedMessage));

			throw new AppException(formattedMessage);
		}

		/// <summary>
		///   Raises the OnFatalError event with the given message and terminates the application by throwing
		///   an InvalidOperationException.
		/// </summary>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnFatalError event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[DebuggerHidden]
		[StringFormatMethod("message")]
		public static void Die(string message, params object[] arguments)
		{
			Die(LogCategory.General, message, arguments);
		}

		/// <summary>
		///   Raises the OnError event with the given message.
		/// </summary>
		/// <param name="category">The category of the log entry.</param>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnError event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[StringFormatMethod("message")]
		public static void Error(LogCategory category, string message, params object[] arguments)
		{
			Assert.InRange(category);
			Assert.ArgumentNotNullOrWhitespace(message);

			if (OnError != null)
				OnError(new LogEntry(category, LogType.Error, String.Format(message, arguments)));
		}

		/// <summary>
		///   Raises the OnError event with the given message.
		/// </summary>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnError event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[StringFormatMethod("message")]
		public static void Error(string message, params object[] arguments)
		{
			Error(LogCategory.General, message, arguments);
		}

		/// <summary>
		///   Raises the OnWarning event with the given message.
		/// </summary>
		/// <param name="category">The category of the log entry.</param>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnWarning event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[StringFormatMethod("message")]
		public static void Warn(LogCategory category, string message, params object[] arguments)
		{
			Assert.InRange(category);
			Assert.ArgumentNotNullOrWhitespace(message);

			if (OnWarning != null)
				OnWarning(new LogEntry(category, LogType.Warning, String.Format(message, arguments)));
		}

		/// <summary>
		///   Raises the OnWarning event with the given message.
		/// </summary>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnWarning event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[StringFormatMethod("message")]
		public static void Warn(string message, params object[] arguments)
		{
			Warn(LogCategory.General, message, arguments);
		}

		/// <summary>
		///   Raises the OnInfo event with the given message.
		/// </summary>
		/// <param name="category">The category of the log entry.</param>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnInfo event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[StringFormatMethod("message")]
		public static void Info(LogCategory category, string message, params object[] arguments)
		{
			Assert.InRange(category);
			Assert.ArgumentNotNullOrWhitespace(message);

			if (OnInfo != null)
				OnInfo(new LogEntry(category, LogType.Info, String.Format(message, arguments)));
		}

		/// <summary>
		///   Raises the OnInfo event with the given message.
		/// </summary>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnInfo event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[StringFormatMethod("message")]
		public static void Info(string message, params object[] arguments)
		{
			Info(LogCategory.General, message, arguments);
		}

		/// <summary>
		///   In debug builds, raises the OnDebugInfo event with the given message.
		/// </summary>
		/// <param name="category">The category of the log entry.</param>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnDebugInfo event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[Conditional("DEBUG")]
		[StringFormatMethod("message")]
		public static void DebugInfo(LogCategory category, string message, params object[] arguments)
		{
			Assert.InRange(category);
			Assert.ArgumentNotNullOrWhitespace(message);

			if (OnDebugInfo != null)
				OnDebugInfo(new LogEntry(category, LogType.Debug, String.Format(message, arguments)));
		}

		/// <summary>
		///   In debug builds, raises the OnDebugInfo event with the given message.
		/// </summary>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnDebugInfo event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[Conditional("DEBUG")]
		[StringFormatMethod("message")]
		public static void DebugInfo(string message, params object[] arguments)
		{
			DebugInfo(LogCategory.General, message, arguments);
		}
	}
}