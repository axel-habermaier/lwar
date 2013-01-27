using System;

namespace Pegasus.Framework.Scripting
{
	using Platform.Input;

	/// <summary>
	///   Provides access to all build-in command instances.
	/// </summary>
	public static class Commands
	{
		/// <summary>
		///   Exits the application when invoked.
		/// </summary>
		public static readonly Command Exit = new Command("exit", "Exits the application.");

		/// <summary>
		///   Executes the given argument.
		/// </summary>
		public static readonly Command<string> Execute = new Command<string>("exec", "Executes the given argument.");

		/// <summary>
		///   Binds a command invocation to a logical input.
		/// </summary>
		public static Command<InputTrigger, string> Bind = new Command<InputTrigger, string>("bind",
																							 "Binds a command invocation to a logical input. " +
																							 "Whenever the input is triggered, the command is " +
																							 "invoked with the specified arguments.");
	}
}