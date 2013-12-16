﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Generated by the Pegasus Asset Compiler.
//     Monday, 16 December 2013, 14:14:14
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Pegasus.Scripting
{
	using Pegasus;
	using Pegasus.Platform.Input;
	using Pegasus.Platform.Logging;
	using Pegasus.Scripting;
	using Pegasus.Scripting.Validators;

	internal static class Commands
	{
		/// <summary>
		///   Immediately exits the application.
		/// </summary>
		public static Command ExitCommand { get; private set; }

		/// <summary>
		///   Describes the usage and the purpose of the the cvar or command with the given name. If no name is given, displays a help text about the usage of cvars and commands in general.
		/// </summary>
		public static Command<string> HelpCommand { get; private set; }

		/// <summary>
		///   Resets the given cvar to its default value.
		/// </summary>
		public static Command<string> ResetCommand { get; private set; }

		/// <summary>
		///   Lists all cvars with names that match the search pattern.
		/// </summary>
		public static Command<string> ListCvarsCommand { get; private set; }

		/// <summary>
		///   Lists all commands with names that match the search pattern.
		/// </summary>
		public static Command<string> ListCommandsCommand { get; private set; }

		/// <summary>
		///   Executes the given command.
		/// </summary>
		public static Command<string> ExecuteCommand { get; private set; }

		/// <summary>
		///   Processes the commands in the given file.
		/// </summary>
		public static Command<string> ProcessCommand { get; private set; }

		/// <summary>
		///   Saves the persistent cvars into the given file.
		/// </summary>
		public static Command<string> PersistCommand { get; private set; }

		/// <summary>
		///   Prints information about the application.
		/// </summary>
		public static Command PrintAppInfoCommand { get; private set; }

		/// <summary>
		///   Binds a command invocation to a logical input. Whenever the input is triggered, the command is invoked with the specified arguments.
		/// </summary>
		public static Command<InputTrigger, string> BindCommand { get; private set; }

		/// <summary>
		///   Unbinds all commands currently bound to a logical input.
		/// </summary>
		public static Command<InputTrigger> UnbindCommand { get; private set; }

		/// <summary>
		///   Removes all command bindings.
		/// </summary>
		public static Command UnbindAllCommand { get; private set; }

		/// <summary>
		///   Lists all active bindings.
		/// </summary>
		public static Command ListBindingsCommand { get; private set; }

		/// <summary>
		///   Shows or hides the console.
		/// </summary>
		public static Command<bool> ShowConsoleCommand { get; private set; }

		/// <summary>
		///   Reloads all changed assets.
		/// </summary>
		public static Command ReloadAssetsCommand { get; private set; }

		/// <summary>
		///   Restarts the graphics subsystem after a resolution or video mode change.
		/// </summary>
		public static Command RestartGraphicsCommand { get; private set; }

		/// <summary>
		///   Toggles the value of a Boolean console variable.
		/// </summary>
		public static Command<string> ToggleCommand { get; private set; }

		/// <summary>
		///   Immediately exits the application.
		/// </summary>
		[DebuggerHidden]
		public static void Exit()
		{
			ExitCommand.Invoke();
		}

		/// <summary>
		///   Describes the usage and the purpose of the the cvar or command with the given name. If no name is given, displays a
		///   help text about the usage of cvars and commands in general.
		/// </summary>
		/// <param name="name">The name of the cvar or the command for which the description should be displayed.</param>
		[DebuggerHidden]
		public static void Help(string name = "")
		{
			Assert.ArgumentNotNull((object)name);
			HelpCommand.Invoke(name);
		}

		/// <summary>
		///   Resets the given cvar to its default value.
		/// </summary>
		/// <param name="cvar">The name of the cvar that should be reset to its default value.</param>
		[DebuggerHidden]
		public static void Reset(string cvar)
		{
			Assert.ArgumentNotNull((object)cvar);
			ResetCommand.Invoke(cvar);
		}

		/// <summary>
		///   Lists all cvars with names that match the search pattern.
		/// </summary>
		/// <param name="pattern">
		///   The search pattern of the cvars that should be listed. For instance, "draw" lists all cvars that have the string
		///   "draw" in their name. The pattern is case-insensitive; use "*" to list all cvars.
		/// </param>
		[DebuggerHidden]
		public static void ListCvars(string pattern = "*")
		{
			Assert.ArgumentNotNull((object)pattern);
			ListCvarsCommand.Invoke(pattern);
		}

		/// <summary>
		///   Lists all commands with names that match the search pattern.
		/// </summary>
		/// <param name="pattern">
		///   The search pattern of the commands that should be listed. For instance, "draw" lists all commands that have the
		///   string "draw" in their name. The pattern is case-insensitive; use "*" to list all commands.
		/// </param>
		[DebuggerHidden]
		public static void ListCommands(string pattern = "*")
		{
			Assert.ArgumentNotNull((object)pattern);
			ListCommandsCommand.Invoke(pattern);
		}

		/// <summary>
		///   Executes the given command.
		/// </summary>
		/// <param name="command">The command that should be executed, including its arguments.</param>
		[DebuggerHidden]
		public static void Execute(string command)
		{
			Assert.ArgumentNotNull((object)command);
			ExecuteCommand.Invoke(command);
		}

		/// <summary>
		///   Processes the commands in the given file.
		/// </summary>
		/// <param name="fileName">The name of the file in the application's user directory that should be processed.</param>
		[DebuggerHidden]
		public static void Process(string fileName)
		{
			Assert.ArgumentNotNull((object)fileName);
			ProcessCommand.Invoke(fileName);
		}

		/// <summary>
		///   Saves the persistent cvars into the given file.
		/// </summary>
		/// <param name="fileName">The name of the file in the application's user directory the cvars should be written to.</param>
		[DebuggerHidden]
		public static void Persist(string fileName)
		{
			Assert.ArgumentNotNull((object)fileName);
			PersistCommand.Invoke(fileName);
		}

		/// <summary>
		///   Prints information about the application.
		/// </summary>
		[DebuggerHidden]
		public static void PrintAppInfo()
		{
			PrintAppInfoCommand.Invoke();
		}

		/// <summary>
		///   Binds a command invocation to a logical input. Whenever the input is triggered, the command is invoked with the
		///   specified arguments.
		/// </summary>
		/// <param name="trigger">The trigger that triggers the command.</param>
		/// <param name="command">The command (including the arguments) that should be executed when the trigger is fired.</param>
		[DebuggerHidden]
		public static void Bind(InputTrigger trigger, string command)
		{
			Assert.ArgumentNotNull((object)trigger);
			Assert.ArgumentNotNull((object)command);
			BindCommand.Invoke(trigger, command);
		}

		/// <summary>
		///   Unbinds all commands currently bound to a logical input.
		/// </summary>
		/// <param name="trigger">The trigger that should be unbound.</param>
		[DebuggerHidden]
		public static void Unbind(InputTrigger trigger)
		{
			Assert.ArgumentNotNull((object)trigger);
			UnbindCommand.Invoke(trigger);
		}

		/// <summary>
		///   Removes all command bindings.
		/// </summary>
		[DebuggerHidden]
		public static void UnbindAll()
		{
			UnbindAllCommand.Invoke();
		}

		/// <summary>
		///   Lists all active bindings.
		/// </summary>
		[DebuggerHidden]
		public static void ListBindings()
		{
			ListBindingsCommand.Invoke();
		}

		/// <summary>
		///   Shows or hides the console.
		/// </summary>
		/// <param name="show">A value of 'true' indicates that the console should be shown.</param>
		[DebuggerHidden]
		public static void ShowConsole(bool show)
		{
			Assert.ArgumentNotNull((object)show);
			ShowConsoleCommand.Invoke(show);
		}

		/// <summary>
		///   Reloads all changed assets.
		/// </summary>
		[DebuggerHidden]
		public static void ReloadAssets()
		{
			ReloadAssetsCommand.Invoke();
		}

		/// <summary>
		///   Restarts the graphics subsystem after a resolution or video mode change.
		/// </summary>
		[DebuggerHidden]
		public static void RestartGraphics()
		{
			RestartGraphicsCommand.Invoke();
		}

		/// <summary>
		///   Toggles the value of a Boolean console variable.
		/// </summary>
		/// <param name="cvar">The name of console variable whose value should be toggled.</param>
		[DebuggerHidden]
		public static void Toggle(string cvar)
		{
			Assert.ArgumentNotNull((object)cvar);
			ToggleCommand.Invoke(cvar);
		}

		/// <summary>
		///   Raised when the 'Exit' command is invoked.
		/// </summary>
		public static event Action OnExit
		{
			add { ExitCommand.Invoked += value; }
			remove { ExitCommand.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'Help' command is invoked.
		/// </summary>
		public static event Action<string> OnHelp
		{
			add { HelpCommand.Invoked += value; }
			remove { HelpCommand.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'Reset' command is invoked.
		/// </summary>
		public static event Action<string> OnReset
		{
			add { ResetCommand.Invoked += value; }
			remove { ResetCommand.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'ListCvars' command is invoked.
		/// </summary>
		public static event Action<string> OnListCvars
		{
			add { ListCvarsCommand.Invoked += value; }
			remove { ListCvarsCommand.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'ListCommands' command is invoked.
		/// </summary>
		public static event Action<string> OnListCommands
		{
			add { ListCommandsCommand.Invoked += value; }
			remove { ListCommandsCommand.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'Execute' command is invoked.
		/// </summary>
		public static event Action<string> OnExecute
		{
			add { ExecuteCommand.Invoked += value; }
			remove { ExecuteCommand.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'Process' command is invoked.
		/// </summary>
		public static event Action<string> OnProcess
		{
			add { ProcessCommand.Invoked += value; }
			remove { ProcessCommand.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'Persist' command is invoked.
		/// </summary>
		public static event Action<string> OnPersist
		{
			add { PersistCommand.Invoked += value; }
			remove { PersistCommand.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'PrintAppInfo' command is invoked.
		/// </summary>
		public static event Action OnPrintAppInfo
		{
			add { PrintAppInfoCommand.Invoked += value; }
			remove { PrintAppInfoCommand.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'Bind' command is invoked.
		/// </summary>
		public static event Action<InputTrigger, string> OnBind
		{
			add { BindCommand.Invoked += value; }
			remove { BindCommand.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'Unbind' command is invoked.
		/// </summary>
		public static event Action<InputTrigger> OnUnbind
		{
			add { UnbindCommand.Invoked += value; }
			remove { UnbindCommand.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'UnbindAll' command is invoked.
		/// </summary>
		public static event Action OnUnbindAll
		{
			add { UnbindAllCommand.Invoked += value; }
			remove { UnbindAllCommand.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'ListBindings' command is invoked.
		/// </summary>
		public static event Action OnListBindings
		{
			add { ListBindingsCommand.Invoked += value; }
			remove { ListBindingsCommand.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'ShowConsole' command is invoked.
		/// </summary>
		public static event Action<bool> OnShowConsole
		{
			add { ShowConsoleCommand.Invoked += value; }
			remove { ShowConsoleCommand.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'ReloadAssets' command is invoked.
		/// </summary>
		public static event Action OnReloadAssets
		{
			add { ReloadAssetsCommand.Invoked += value; }
			remove { ReloadAssetsCommand.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'RestartGraphics' command is invoked.
		/// </summary>
		public static event Action OnRestartGraphics
		{
			add { RestartGraphicsCommand.Invoked += value; }
			remove { RestartGraphicsCommand.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'Toggle' command is invoked.
		/// </summary>
		public static event Action<string> OnToggle
		{
			add { ToggleCommand.Invoked += value; }
			remove { ToggleCommand.Invoked -= value; }
		}

		/// <summary>
		///   Initializes the instances declared by the registry.
		/// </summary>
		public static void Initialize()
		{
			ExitCommand = new Command("exit", "Immediately exits the application.", false);
			HelpCommand = new Command<string>("help", "Describes the usage and the purpose of the the cvar or command with the given name. If no name is given, displays a help text about the usage of cvars and commands in general.", false, 
				new CommandParameter("name", typeof(string), true, "", "The name of the cvar or the command for which the description should be displayed."));
			ResetCommand = new Command<string>("reset", "Resets the given cvar to its default value.", false, 
				new CommandParameter("cvar", typeof(string), false, default(string), "The name of the cvar that should be reset to its default value.", new NotEmptyAttribute()));
			ListCvarsCommand = new Command<string>("list_cvars", "Lists all cvars with names that match the search pattern.", false, 
				new CommandParameter("pattern", typeof(string), true, "*", "The search pattern of the cvars that should be listed. For instance, \"draw\" lists all cvars that have the string \"draw\" in their name. The pattern is case-insensitive; use \"*\" to list all cvars.", new NotEmptyAttribute()));
			ListCommandsCommand = new Command<string>("list_commands", "Lists all commands with names that match the search pattern.", false, 
				new CommandParameter("pattern", typeof(string), true, "*", "The search pattern of the commands that should be listed. For instance, \"draw\" lists all commands that have the string \"draw\" in their name. The pattern is case-insensitive; use \"*\" to list all commands.", new NotEmptyAttribute()));
			ExecuteCommand = new Command<string>("execute", "Executes the given command.", false, 
				new CommandParameter("command", typeof(string), false, default(string), "The command that should be executed, including its arguments.", new NotEmptyAttribute()));
			ProcessCommand = new Command<string>("process", "Processes the commands in the given file.", false, 
				new CommandParameter("fileName", typeof(string), false, default(string), "The name of the file in the application's user directory that should be processed.", new NotEmptyAttribute(), new FileNameAttribute()));
			PersistCommand = new Command<string>("persist", "Saves the persistent cvars into the given file.", false, 
				new CommandParameter("fileName", typeof(string), false, default(string), "The name of the file in the application's user directory the cvars should be written to.", new NotEmptyAttribute(), new FileNameAttribute()));
			PrintAppInfoCommand = new Command("print_app_info", "Prints information about the application.", false);
			BindCommand = new Command<InputTrigger, string>("bind", "Binds a command invocation to a logical input. Whenever the input is triggered, the command is invoked with the specified arguments.", false, 
				new CommandParameter("trigger", typeof(InputTrigger), false, default(InputTrigger), "The trigger that triggers the command."),
				new CommandParameter("command", typeof(string), false, default(string), "The command (including the arguments) that should be executed when the trigger is fired.", new NotEmptyAttribute()));
			UnbindCommand = new Command<InputTrigger>("unbind", "Unbinds all commands currently bound to a logical input.", false, 
				new CommandParameter("trigger", typeof(InputTrigger), false, default(InputTrigger), "The trigger that should be unbound."));
			UnbindAllCommand = new Command("unbind_all", "Removes all command bindings.", false);
			ListBindingsCommand = new Command("list_bindings", "Lists all active bindings.", false);
			ShowConsoleCommand = new Command<bool>("show_console", "Shows or hides the console.", false, 
				new CommandParameter("show", typeof(bool), false, default(bool), "A value of 'true' indicates that the console should be shown."));
			ReloadAssetsCommand = new Command("reload_assets", "Reloads all changed assets.", false);
			RestartGraphicsCommand = new Command("restart_graphics", "Restarts the graphics subsystem after a resolution or video mode change.", false);
			ToggleCommand = new Command<string>("toggle", "Toggles the value of a Boolean console variable.", false, 
				new CommandParameter("cvar", typeof(string), false, default(string), "The name of console variable whose value should be toggled.", new NotEmptyAttribute()));

			CommandRegistry.Register(ExitCommand);
			CommandRegistry.Register(HelpCommand);
			CommandRegistry.Register(ResetCommand);
			CommandRegistry.Register(ListCvarsCommand);
			CommandRegistry.Register(ListCommandsCommand);
			CommandRegistry.Register(ExecuteCommand);
			CommandRegistry.Register(ProcessCommand);
			CommandRegistry.Register(PersistCommand);
			CommandRegistry.Register(PrintAppInfoCommand);
			CommandRegistry.Register(BindCommand);
			CommandRegistry.Register(UnbindCommand);
			CommandRegistry.Register(UnbindAllCommand);
			CommandRegistry.Register(ListBindingsCommand);
			CommandRegistry.Register(ShowConsoleCommand);
			CommandRegistry.Register(ReloadAssetsCommand);
			CommandRegistry.Register(RestartGraphicsCommand);
			CommandRegistry.Register(ToggleCommand);
		}

		/// <summary>
		///   Initializes the instances imported by the registry.
		/// </summary>
		public static void Resolve()
		{
		}
	}
}

