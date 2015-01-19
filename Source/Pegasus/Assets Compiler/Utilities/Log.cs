namespace Pegasus.AssetsCompiler.Utilities
{
	using System;
	using System.Diagnostics;

	/// <summary>
	///     Provides functions to log fatal errors, errors, warnings, informational messages, and debug-time only
	///     informational messages. An event is raised whenever one of these functions is invoked.
	/// </summary>
	public static class Log
	{
		/// <summary>
		///     Used for thread synchronization.
		/// </summary>
		private static readonly Object SyncObject = new object();

		/// <summary>
		///     Raises the OnFatalError event with the given message and terminates the application by throwing
		///     an InvalidOperationException.
		/// </summary>
		/// <param name="message">The message that should be formatted and passed as an argument of the OnFatalError event.</param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[DebuggerHidden, StringFormatMethod("message"), ContractAnnotation("=> halt")]
		public static void Die(string message, params object[] arguments)
		{
			Assert.ArgumentNotNullOrWhitespace(message);

			var formattedMessage = String.Format(message, arguments);
			WriteToConsole(ConsoleColor.Red, "[Error  ] " + String.Format(message, arguments));

			throw new PegasusException(formattedMessage);
		}

		/// <summary>
		///     Raises the OnError event with the given message.
		/// </summary>
		/// <param name="message">The message that should be formatted and passed as an argument of the OnError event.</param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[StringFormatMethod("message")]
		public static void Error(string message, params object[] arguments)
		{
			Assert.ArgumentNotNullOrWhitespace(message);
			WriteToConsole(ConsoleColor.Red, "[Error  ] " + String.Format(message, arguments));
		}

		/// <summary>
		///     Raises the OnWarning event with the given message.
		/// </summary>
		/// <param name="message">The message that should be formatted and passed as an argument of the OnWarning event.</param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[StringFormatMethod("message")]
		public static void Warn(string message, params object[] arguments)
		{
			Assert.ArgumentNotNullOrWhitespace(message);
			WriteToConsole(ConsoleColor.Yellow, "[Warning] " + String.Format(message, arguments));
		}

		/// <summary>
		///     Raises the OnInfo event with the given message.
		/// </summary>
		/// <param name="message">The message that should be formatted and passed as an argument of the OnInfo event.</param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[StringFormatMethod("message")]
		public static void Info(string message, params object[] arguments)
		{
			Assert.ArgumentNotNullOrWhitespace(message);
			WriteToConsole(ConsoleColor.White, String.Format(message, arguments));
		}

		/// <summary>
		///     In debug builds, raises the OnDebugInfo event with the given message.
		/// </summary>
		/// <param name="message">The message that should be formatted and passed as an argument of the OnDebugInfo event.</param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[Conditional("DEBUG"), StringFormatMethod("message")]
		public static void Debug(string message, params object[] arguments)
		{
			Assert.ArgumentNotNullOrWhitespace(message);
			WriteToConsole(ConsoleColor.White, String.Format(message, arguments));
		}

		/// <summary>
		///     In debug builds, raises the OnDebugInfo event with the given message if the given condition is true.
		/// </summary>
		/// <param name="condition">The condition that must be true for the message to be displayed.</param>
		/// <param name="message">The message that should be formatted and passed as an argument of the OnDebugInfo event.</param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		[Conditional("DEBUG"), StringFormatMethod("message")]
		public static void DebugIf(bool condition, string message, params object[] arguments)
		{
			Assert.ArgumentNotNullOrWhitespace(message);

			if (condition)
				Debug(message, arguments);
		}

		/// <summary>
		///     Writes a colored message to the console.
		/// </summary>
		/// <param name="color">The color of the message.</param>
		/// <param name="message">The message that should be written to the console.</param>
		private static void WriteToConsole(ConsoleColor color, string message)
		{
			lock (SyncObject)
			{
				WriteColored(color, () => Console.WriteLine(message));
				System.Diagnostics.Debug.WriteLine(message);
			}
		}

		/// <summary>
		///     Writes a colored message to the console, ensuring that the color is reset.
		/// </summary>
		/// <param name="color">The color of the message.</param>
		/// <param name="action">Writes the message to the console.</param>
		private static void WriteColored(ConsoleColor color, Action action)
		{
			Console.ForegroundColor = color;
			action();
			Console.ResetColor();
		}
	}
}