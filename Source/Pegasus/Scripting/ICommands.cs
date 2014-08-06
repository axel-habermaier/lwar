using Pegasus.Framework.UserInterface.Input;
using Pegasus.Scripting;
using Pegasus.Scripting.Validators;

// ReSharper disable CheckNamespace

/// <summary>
///     Declares the commands required by the framework.
/// </summary>
internal interface ICommands
{
	/// <summary>
	///     Immediately exits the application.
	/// </summary>
	[Command]
	void Exit();

	/// <summary>
	///     Describes the usage and the purpose of the the cvar or command with the given name. If no name is given, displays a
	///     help text about the usage of cvars and commands in general.
	/// </summary>
	/// <param name="name">The name of the cvar or the command for which the description should be displayed.</param>
	[Command]
	void Help(string name = "");

	/// <summary>
	///     Resets the given cvar to its default value.
	/// </summary>
	/// <param name="cvar">The name of the cvar that should be reset to its default value.</param>
	[Command]
	void Reset([NotEmpty] string cvar);

	/// <summary>
	///     Lists all cvars with names that match the search pattern.
	/// </summary>
	/// <param name="pattern">
	///     The search pattern of the cvars that should be listed. For instance, "draw" lists all cvars that have the string
	///     "draw" in their name. The pattern is case-insensitive; use "*" to list all cvars.
	/// </param>
	[Command]
	void ListCvars([NotEmpty] string pattern = "*");

	/// <summary>
	///     Lists all commands with names that match the search pattern.
	/// </summary>
	/// <param name="pattern">
	///     The search pattern of the commands that should be listed. For instance, "draw" lists all commands that have the
	///     string "draw" in their name. The pattern is case-insensitive; use "*" to list all commands.
	/// </param>
	[Command]
	void ListCommands([NotEmpty] string pattern = "*");

	/// <summary>
	///     Executes the given command.
	/// </summary>
	/// <param name="command">The command that should be executed, including its arguments.</param>
	[Command]
	void Execute([NotEmpty] string command);

	/// <summary>
	///     Processes the commands in the given file.
	/// </summary>
	/// <param name="fileName">The name of the file in the application's user directory that should be processed.</param>
	[Command]
	void Process([NotEmpty, FileName] string fileName);

	/// <summary>
	///     Saves the persistent cvars into the given file.
	/// </summary>
	/// <param name="fileName">The name of the file in the application's user directory the cvars should be written to.</param>
	[Command]
	void Persist([NotEmpty, FileName] string fileName);

	/// <summary>
	///     Prints information about the application.
	/// </summary>
	[Command]
	void PrintAppInfo();

	/// <summary>
	///     Binds a command invocation to a logical input. Whenever the input is triggered, the command is invoked with the
	///     specified arguments.
	/// </summary>
	/// <param name="trigger">The trigger that triggers the command.</param>
	/// <param name="command">The command (including the arguments) that should be executed when the trigger is fired.</param>
	[Command]
	void Bind(InputTrigger trigger, [NotEmpty] string command);

	/// <summary>
	///     Unbinds all commands currently bound to a logical input.
	/// </summary>
	/// <param name="trigger">The trigger that should be unbound.</param>
	[Command]
	void Unbind(InputTrigger trigger);

	/// <summary>
	///     Removes all command bindings.
	/// </summary>
	[Command]
	void UnbindAll();

	/// <summary>
	///     Lists all active bindings.
	/// </summary>
	[Command]
	void ListBindings();

	/// <summary>
	///     Shows or hides the console.
	/// </summary>
	/// <param name="show">A value of 'true' indicates that the console should be shown.</param>
	[Command]
	void ShowConsole(bool show);

	/// <summary>
	///     Reloads all currently loaded assets.
	/// </summary>
	[Command]
	void ReloadAssets();

	/// <summary>
	///     Restarts the graphics subsystem after a resolution or video mode change.
	/// </summary>
	[Command]
	void RestartGraphics();

	/// <summary>
	///     Toggles the value of a Boolean console variable.
	/// </summary>
	/// <param name="cvar">The name of console variable whose value should be toggled.</param>
	[Command]
	void Toggle([NotEmpty] string cvar);
}