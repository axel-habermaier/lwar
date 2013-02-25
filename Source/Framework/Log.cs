using System;

namespace Pegasus.Framework
{
	using System.Diagnostics;
	using Scripting;

	/// <summary>
	///   Provides functions to log fatal errors, errors, warnings, informational messages, and debug-time only
	///   informational messages. An event is raised whenever one of these functions is invoked.
	/// </summary>
	public static class Log
	{
		/// <summary>
		///   Wires up the events to write all logged messages to the console.
		/// </summary>
		public static void PrintToConsole()
		{
			OnFatalError += message => WriteToConsole(ConsoleColor.Red, message);
			OnError += message => WriteToConsole(ConsoleColor.Red, message);
			OnWarning += message => WriteToConsole(ConsoleColor.Yellow, message);
			OnInfo += message => WriteToConsole(ConsoleColor.White, message);
			OnDebugInfo += message => WriteToConsole(ConsoleColor.Magenta, message);
		}

		/// <summary>
		/// Writes a colored message to the console.
		/// </summary>
		/// <param name="color">The color of the message.</param>
		/// <param name="message">The message that should be written to the console.</param>
		private static void WriteToConsole(ConsoleColor color, string message)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(message);
		}

		/// <summary>
		///   Raised when a fatal error occurred. Typically, the program terminates after all event handlers have
		///   been executed.
		/// </summary>
		public static event Action<string> OnFatalError;

		/// <summary>
		///   Raised when an error occurred.
		/// </summary>
		public static event Action<string> OnError;

		/// <summary>
		///   Raised when a warning was generated.
		/// </summary>
		public static event Action<string> OnWarning;

		/// <summary>
		///   Raised when an informational message was generated.
		/// </summary>
		public static event Action<string> OnInfo;

		/// <summary>
		///   Raised when a debug informational message was generated.
		/// </summary>
		public static event Action<string> OnDebugInfo;

		/// <summary>
		///   Raises the OnFatalError event with the given message and terminates the application by throwing
		///   an InvalidOperationException.
		/// </summary>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnFatalError event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[DebuggerHidden]
		public static void Die(string message, params object[] arguments)
		{
			Assert.ArgumentNotNullOrWhitespace(message, () => message);
			var formattedMessage = String.Format(message, arguments);

			if (OnFatalError != null)
				OnFatalError(formattedMessage);

			throw new InvalidOperationException(formattedMessage);
		}

		/// <summary>
		///   Raises the OnError event with the given message.
		/// </summary>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnError event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		public static void Error(string message, params object[] arguments)
		{
			Assert.ArgumentNotNullOrWhitespace(message, () => message);

			if (OnError != null)
				OnError(String.Format(message, arguments));
		}

		/// <summary>
		///   Raises the OnWarning event with the given message.
		/// </summary>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnWarning event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		public static void Warn(string message, params object[] arguments)
		{
			Assert.ArgumentNotNullOrWhitespace(message, () => message);

			if (OnWarning != null)
				OnWarning(String.Format(message, arguments));
		}

		/// <summary>
		///   Raises the OnInfo event with the given message.
		/// </summary>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnInfo event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		public static void Info(string message, params object[] arguments)
		{
			Assert.ArgumentNotNullOrWhitespace(message, () => message);

			if (OnInfo != null)
				OnInfo(String.Format(message, arguments));
		}

		/// <summary>
		///   In debug builds, raises the OnDebugInfo event with the given message.
		/// </summary>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnDebugInfo event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[Conditional("DEBUG")]
		public static void DebugInfo(string message, params object[] arguments)
		{
			Assert.ArgumentNotNullOrWhitespace(message, () => message);

			if (OnDebugInfo != null)
				OnDebugInfo(String.Format(message, arguments));
		}

		/// <summary>
		///   Raises the OnDebugInfo event with the given message if the given cvar is set to true.
		/// </summary>
		/// <param name="enabled">The cvar that determines whether the message should be logged.</param>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnDebugInfo event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		public static void DebugInfo(Cvar<bool> enabled, string message, params object[] arguments)
		{
			Assert.ArgumentNotNull(enabled, () => enabled);
			Assert.ArgumentNotNullOrWhitespace(message, () => message);

			if (enabled.Value && OnDebugInfo != null)
				OnDebugInfo(String.Format(message, arguments));
		}
	}
}