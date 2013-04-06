﻿using System;

namespace Pegasus.Framework.Scripting
{
	using Platform.Input;

	/// <summary>
	///   Declares the commands required by the framework.
	/// </summary>
	public interface ICommandRegistry
	{
		/// <summary>
		///   Immediately exits the application.
		/// </summary>
		[Command]
		void Exit();

		/// <summary>
		///   Describes the usage and the purpose of the the cvar or command with the given name. If no name is given, displays a
		///   help text about the usage of cvars and commands in general.
		/// </summary>
		/// <param name="name">The name of the cvar or the command for which the description should be displayed.</param>
		[Command]
		void Help(string name = "");

		/// <summary>
		///   Executes the given command.
		/// </summary>
		/// <param name="command">The command that should be executed, including its arguments.</param>
		[Command]
		void Execute([NotEmpty] string command);

		/// <summary>
		///   Processes the commands in the given file.
		/// </summary>
		/// <param name="fileName">The name of the file in the application's user directory that should be processed.</param>
		[Command]
		void Process([NotEmpty, FileName] string fileName);

		/// <summary>
		///   Saves the persistent cvars into the given file.
		/// </summary>
		/// <param name="fileName">The name of the file in the application's user directory the cvars should be written to.</param>
		[Command]
		void Persist([NotEmpty, FileName] string fileName);

		/// <summary>
		///   Binds a command invocation to a logical input. Whenever the input is triggered, the command is invoked with the
		///   specified arguments.
		/// </summary>
		/// <param name="trigger">The trigger that triggers the command.</param>
		/// <param name="command">The command (including the arguments) that should be executed when the trigger is fired.</param>
		[Command]
		void Bind(InputTrigger trigger, [NotEmpty] string command);

		/// <summary>
		///   Shows or hides the console.
		/// </summary>
		/// <param name="show">A value of 'true' indicates that the console should be shown.</param>
		[Command]
		void ShowConsole(bool show);

		/// <summary>
		///   Reloads all changed assets.
		/// </summary>
		[Command]
		void ReloadAssets();
	}
}