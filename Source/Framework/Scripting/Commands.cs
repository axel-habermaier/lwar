﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Generated by the Pegasus Asset Compiler.
//     Wednesday, 05 June 2013, 18:45:52
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Pegasus.Framework.Scripting
{
	using Pegasus.Framework;
	using Pegasus.Framework.Platform.Input;
	using Pegasus.Framework.Platform.Logging;
	using Pegasus.Framework.Scripting;
	using Pegasus.Framework.Scripting.Validators;

	internal static class Commands
	{
		/// <summary>
		///   Immediately exits the application.
		/// </summary>
		public static Command ExitCvar { get; private set; }

		/// <summary>
		///   Describes the usage and the purpose of the the cvar or command with the given name. If no name is given, displays a help text about the usage of cvars and commands in general.
		/// </summary>
		public static Command<string> HelpCvar { get; private set; }

		/// <summary>
		///   Resets the given cvar to its default value.
		/// </summary>
		public static Command<string> ResetCvar { get; private set; }

		/// <summary>
		///   Lists all cvars with names that match the search pattern.
		/// </summary>
		public static Command<string> ListCvarsCvar { get; private set; }

		/// <summary>
		///   Lists all commands with names that match the search pattern.
		/// </summary>
		public static Command<string> ListCommandsCvar { get; private set; }

		/// <summary>
		///   Executes the given command.
		/// </summary>
		public static Command<string> ExecuteCvar { get; private set; }

		/// <summary>
		///   Processes the commands in the given file.
		/// </summary>
		public static Command<string> ProcessCvar { get; private set; }

		/// <summary>
		///   Saves the persistent cvars into the given file.
		/// </summary>
		public static Command<string> PersistCvar { get; private set; }

		/// <summary>
		///   Binds a command invocation to a logical input. Whenever the input is triggered, the command is invoked with the specified arguments.
		/// </summary>
		public static Command<InputTrigger, string> BindCvar { get; private set; }

		/// <summary>
		///   Shows or hides the console.
		/// </summary>
		public static Command<bool> ShowConsoleCvar { get; private set; }

		/// <summary>
		///   Toggles the visiblity of the statistics.
		/// </summary>
		public static Command ToggleStatsCvar { get; private set; }

		/// <summary>
		///   Reloads all changed assets.
		/// </summary>
		public static Command ReloadAssetsCvar { get; private set; }

		/// <summary>
		///   Restarts the graphics subsystem after a resolution or video mode change.
		/// </summary>
		public static Command RestartGraphicsCvar { get; private set; }

		/// <summary>
		///   Immediately exits the application.
		/// </summary>
		[DebuggerHidden]
		public static void Exit()
		{
			ExitCvar.Invoke();
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
			HelpCvar.Invoke(name);
		}

		/// <summary>
		///   Resets the given cvar to its default value.
		/// </summary>
		/// <param name="cvar">The name of the cvar that should be reset to its default value.</param>
		[DebuggerHidden]
		public static void Reset(string cvar)
		{
			Assert.ArgumentNotNull((object)cvar);
			ResetCvar.Invoke(cvar);
		}

		/// <summary>
		///   Lists all cvars with names that match the search pattern.
		/// </summary>
		/// <param name="pattern">
		///   The search pattern of the cvars that should be listed. For instance, 'draw' lists all cvars that have the string
		///   'draw' in their name. The pattern is case-insensitive; use '*' to list all cvars.
		/// </param>
		[DebuggerHidden]
		public static void ListCvars(string pattern = "*")
		{
			Assert.ArgumentNotNull((object)pattern);
			ListCvarsCvar.Invoke(pattern);
		}

		/// <summary>
		///   Lists all commands with names that match the search pattern.
		/// </summary>
		/// <param name="pattern">
		///   The search pattern of the commands that should be listed. For instance, 'draw' lists all commands that have the
		///   string 'draw' in their name. The pattern is case-insensitive; use '*' to list all commands.
		/// </param>
		[DebuggerHidden]
		public static void ListCommands(string pattern = "*")
		{
			Assert.ArgumentNotNull((object)pattern);
			ListCommandsCvar.Invoke(pattern);
		}

		/// <summary>
		///   Executes the given command.
		/// </summary>
		/// <param name="command">The command that should be executed, including its arguments.</param>
		[DebuggerHidden]
		public static void Execute(string command)
		{
			Assert.ArgumentNotNull((object)command);
			ExecuteCvar.Invoke(command);
		}

		/// <summary>
		///   Processes the commands in the given file.
		/// </summary>
		/// <param name="fileName">The name of the file in the application's user directory that should be processed.</param>
		[DebuggerHidden]
		public static void Process(string fileName)
		{
			Assert.ArgumentNotNull((object)fileName);
			ProcessCvar.Invoke(fileName);
		}

		/// <summary>
		///   Saves the persistent cvars into the given file.
		/// </summary>
		/// <param name="fileName">The name of the file in the application's user directory the cvars should be written to.</param>
		[DebuggerHidden]
		public static void Persist(string fileName)
		{
			Assert.ArgumentNotNull((object)fileName);
			PersistCvar.Invoke(fileName);
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
			BindCvar.Invoke(trigger, command);
		}

		/// <summary>
		///   Shows or hides the console.
		/// </summary>
		/// <param name="show">A value of 'true' indicates that the console should be shown.</param>
		[DebuggerHidden]
		public static void ShowConsole(bool show)
		{
			Assert.ArgumentNotNull((object)show);
			ShowConsoleCvar.Invoke(show);
		}

		/// <summary>
		///   Toggles the visiblity of the statistics.
		/// </summary>
		[DebuggerHidden]
		public static void ToggleStats()
		{
			ToggleStatsCvar.Invoke();
		}

		/// <summary>
		///   Reloads all changed assets.
		/// </summary>
		[DebuggerHidden]
		public static void ReloadAssets()
		{
			ReloadAssetsCvar.Invoke();
		}

		/// <summary>
		///   Restarts the graphics subsystem after a resolution or video mode change.
		/// </summary>
		[DebuggerHidden]
		public static void RestartGraphics()
		{
			RestartGraphicsCvar.Invoke();
		}

		/// <summary>
		///   Raised when the 'Exit' command is invoked.
		/// </summary>
		public static event Action OnExit
		{
			add { ExitCvar.Invoked += value; }
			remove { ExitCvar.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'Help' command is invoked.
		/// </summary>
		public static event Action<string> OnHelp
		{
			add { HelpCvar.Invoked += value; }
			remove { HelpCvar.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'Reset' command is invoked.
		/// </summary>
		public static event Action<string> OnReset
		{
			add { ResetCvar.Invoked += value; }
			remove { ResetCvar.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'ListCvars' command is invoked.
		/// </summary>
		public static event Action<string> OnListCvars
		{
			add { ListCvarsCvar.Invoked += value; }
			remove { ListCvarsCvar.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'ListCommands' command is invoked.
		/// </summary>
		public static event Action<string> OnListCommands
		{
			add { ListCommandsCvar.Invoked += value; }
			remove { ListCommandsCvar.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'Execute' command is invoked.
		/// </summary>
		public static event Action<string> OnExecute
		{
			add { ExecuteCvar.Invoked += value; }
			remove { ExecuteCvar.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'Process' command is invoked.
		/// </summary>
		public static event Action<string> OnProcess
		{
			add { ProcessCvar.Invoked += value; }
			remove { ProcessCvar.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'Persist' command is invoked.
		/// </summary>
		public static event Action<string> OnPersist
		{
			add { PersistCvar.Invoked += value; }
			remove { PersistCvar.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'Bind' command is invoked.
		/// </summary>
		public static event Action<InputTrigger, string> OnBind
		{
			add { BindCvar.Invoked += value; }
			remove { BindCvar.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'ShowConsole' command is invoked.
		/// </summary>
		public static event Action<bool> OnShowConsole
		{
			add { ShowConsoleCvar.Invoked += value; }
			remove { ShowConsoleCvar.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'ToggleStats' command is invoked.
		/// </summary>
		public static event Action OnToggleStats
		{
			add { ToggleStatsCvar.Invoked += value; }
			remove { ToggleStatsCvar.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'ReloadAssets' command is invoked.
		/// </summary>
		public static event Action OnReloadAssets
		{
			add { ReloadAssetsCvar.Invoked += value; }
			remove { ReloadAssetsCvar.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'RestartGraphics' command is invoked.
		/// </summary>
		public static event Action OnRestartGraphics
		{
			add { RestartGraphicsCvar.Invoked += value; }
			remove { RestartGraphicsCvar.Invoked -= value; }
		}

		/// <summary>
		///   Initializes the instances declared by the registry.
		/// </summary>
		public static void Initialize()
		{
			ExitCvar = new Command("exit", "Immediately exits the application.");
			HelpCvar = new Command<string>("help", "Describes the usage and the purpose of the the cvar or command with the given name. If no name is given, displays a help text about the usage of cvars and commands in general.", 
				new CommandParameter("name", typeof(string), true, "", "The name of the cvar or the command for which the description should be displayed."));
			ResetCvar = new Command<string>("reset", "Resets the given cvar to its default value.", 
				new CommandParameter("cvar", typeof(string), false, default(string), "The name of the cvar that should be reset to its default value.", new NotEmptyAttribute()));
			ListCvarsCvar = new Command<string>("list_cvars", "Lists all cvars with names that match the search pattern.", 
				new CommandParameter("pattern", typeof(string), true, "*", "The search pattern of the cvars that should be listed. For instance, 'draw' lists all cvars that have the string 'draw' in their name. The pattern is case-insensitive; use '*' to list all cvars.", new NotEmptyAttribute()));
			ListCommandsCvar = new Command<string>("list_commands", "Lists all commands with names that match the search pattern.", 
				new CommandParameter("pattern", typeof(string), true, "*", "The search pattern of the commands that should be listed. For instance, 'draw' lists all commands that have the string 'draw' in their name. The pattern is case-insensitive; use '*' to list all commands.", new NotEmptyAttribute()));
			ExecuteCvar = new Command<string>("execute", "Executes the given command.", 
				new CommandParameter("command", typeof(string), false, default(string), "The command that should be executed, including its arguments.", new NotEmptyAttribute()));
			ProcessCvar = new Command<string>("process", "Processes the commands in the given file.", 
				new CommandParameter("fileName", typeof(string), false, default(string), "The name of the file in the application's user directory that should be processed.", new NotEmptyAttribute(), new FileNameAttribute()));
			PersistCvar = new Command<string>("persist", "Saves the persistent cvars into the given file.", 
				new CommandParameter("fileName", typeof(string), false, default(string), "The name of the file in the application's user directory the cvars should be written to.", new NotEmptyAttribute(), new FileNameAttribute()));
			BindCvar = new Command<InputTrigger, string>("bind", "Binds a command invocation to a logical input. Whenever the input is triggered, the command is invoked with the specified arguments.", 
				new CommandParameter("trigger", typeof(InputTrigger), false, default(InputTrigger), "The trigger that triggers the command."),
				new CommandParameter("command", typeof(string), false, default(string), "The command (including the arguments) that should be executed when the trigger is fired.", new NotEmptyAttribute()));
			ShowConsoleCvar = new Command<bool>("show_console", "Shows or hides the console.", 
				new CommandParameter("show", typeof(bool), false, default(bool), "A value of 'true' indicates that the console should be shown."));
			ToggleStatsCvar = new Command("toggle_stats", "Toggles the visiblity of the statistics.");
			ReloadAssetsCvar = new Command("reload_assets", "Reloads all changed assets.");
			RestartGraphicsCvar = new Command("restart_graphics", "Restarts the graphics subsystem after a resolution or video mode change.");

			CommandRegistry.Register(ExitCvar);
			CommandRegistry.Register(HelpCvar);
			CommandRegistry.Register(ResetCvar);
			CommandRegistry.Register(ListCvarsCvar);
			CommandRegistry.Register(ListCommandsCvar);
			CommandRegistry.Register(ExecuteCvar);
			CommandRegistry.Register(ProcessCvar);
			CommandRegistry.Register(PersistCvar);
			CommandRegistry.Register(BindCvar);
			CommandRegistry.Register(ShowConsoleCvar);
			CommandRegistry.Register(ToggleStatsCvar);
			CommandRegistry.Register(ReloadAssetsCvar);
			CommandRegistry.Register(RestartGraphicsCvar);
		}

		/// <summary>
		///   Initializes the instances imported by the registry.
		/// </summary>
		public static void Resolve()
		{
		}
	}
}

