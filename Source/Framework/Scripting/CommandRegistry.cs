﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Generated by the Pegasus Asset Compiler.
//     Tuesday, May 28, 2013, 15:24:27
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
	using Platform.Input;

	public class CommandRegistry : Registry<ICommand>
	{
		/// <summary>
		///   Provides access to the actual instances of the cvars or commands managed by the registry.
		/// </summary>
		public new InstanceList Instances { get; private set; }

		/// <summary>
		///   Immediately exits the application.
		/// </summary>
		[DebuggerHidden]
		public void Exit()
		{
			Instances.Exit.Invoke();
		}

		/// <summary>
		///   Describes the usage and the purpose of the the cvar or command with the given name. If no name is given, displays a
		///   help text about the usage of cvars and commands in general.
		/// </summary>
		/// <param name="name">The name of the cvar or the command for which the description should be displayed.</param>
		[DebuggerHidden]
		public void Help(string name = "")
		{
			Assert.ArgumentNotNull((object)name);
			Instances.Help.Invoke(name);
		}

		/// <summary>
		///   Lists all cvars with names that match the search pattern.
		/// </summary>
		/// <param name="pattern">
		///   The search pattern of the cvars that should be listed. For instance, 'draw' lists all cvars that have the string
		///   'draw' in their name. The pattern is case-insensitive; use '*' to list all cvars.
		/// </param>
		[DebuggerHidden]
		public void Cvars(string pattern = "*")
		{
			Assert.ArgumentNotNull((object)pattern);
			Instances.Cvars.Invoke(pattern);
		}

		/// <summary>
		///   Lists all commands with names that match the search pattern.
		/// </summary>
		/// <param name="pattern">
		///   The search pattern of the commands that should be listed. For instance, 'draw' lists all commands that have the
		///   string 'draw' in their name. The pattern is case-insensitive; use '*' to list all commands.
		/// </param>
		[DebuggerHidden]
		public void Commands(string pattern = "*")
		{
			Assert.ArgumentNotNull((object)pattern);
			Instances.Commands.Invoke(pattern);
		}

		/// <summary>
		///   Executes the given command.
		/// </summary>
		/// <param name="command">The command that should be executed, including its arguments.</param>
		[DebuggerHidden]
		public void Execute(string command)
		{
			Assert.ArgumentNotNull((object)command);
			Instances.Execute.Invoke(command);
		}

		/// <summary>
		///   Processes the commands in the given file.
		/// </summary>
		/// <param name="fileName">The name of the file in the application's user directory that should be processed.</param>
		[DebuggerHidden]
		public void Process(string fileName)
		{
			Assert.ArgumentNotNull((object)fileName);
			Instances.Process.Invoke(fileName);
		}

		/// <summary>
		///   Saves the persistent cvars into the given file.
		/// </summary>
		/// <param name="fileName">The name of the file in the application's user directory the cvars should be written to.</param>
		[DebuggerHidden]
		public void Persist(string fileName)
		{
			Assert.ArgumentNotNull((object)fileName);
			Instances.Persist.Invoke(fileName);
		}

		/// <summary>
		///   Binds a command invocation to a logical input. Whenever the input is triggered, the command is invoked with the
		///   specified arguments.
		/// </summary>
		/// <param name="trigger">The trigger that triggers the command.</param>
		/// <param name="command">The command (including the arguments) that should be executed when the trigger is fired.</param>
		[DebuggerHidden]
		public void Bind(InputTrigger trigger, string command)
		{
			Assert.ArgumentNotNull((object)trigger);
			Assert.ArgumentNotNull((object)command);
			Instances.Bind.Invoke(trigger, command);
		}

		/// <summary>
		///   Shows or hides the console.
		/// </summary>
		/// <param name="show">A value of 'true' indicates that the console should be shown.</param>
		[DebuggerHidden]
		public void ShowConsole(bool show)
		{
			Assert.ArgumentNotNull((object)show);
			Instances.ShowConsole.Invoke(show);
		}

		/// <summary>
		///   Reloads all changed assets.
		/// </summary>
		[DebuggerHidden]
		public void ReloadAssets()
		{
			Instances.ReloadAssets.Invoke();
		}

		/// <summary>
		///   Restarts the graphics subsystem after a resolution or video mode change.
		/// </summary>
		[DebuggerHidden]
		public void RestartGraphics()
		{
			Instances.RestartGraphics.Invoke();
		}

		/// <summary>
		///   Raised when the Exit command is invoked.
		/// </summary>
		public event Action OnExit
		{
			add { Instances.Exit.Invoked += value; }
			remove { Instances.Exit.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the Help command is invoked.
		/// </summary>
		public event Action<string> OnHelp
		{
			add { Instances.Help.Invoked += value; }
			remove { Instances.Help.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the Cvars command is invoked.
		/// </summary>
		public event Action<string> OnCvars
		{
			add { Instances.Cvars.Invoked += value; }
			remove { Instances.Cvars.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the Commands command is invoked.
		/// </summary>
		public event Action<string> OnCommands
		{
			add { Instances.Commands.Invoked += value; }
			remove { Instances.Commands.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the Execute command is invoked.
		/// </summary>
		public event Action<string> OnExecute
		{
			add { Instances.Execute.Invoked += value; }
			remove { Instances.Execute.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the Process command is invoked.
		/// </summary>
		public event Action<string> OnProcess
		{
			add { Instances.Process.Invoked += value; }
			remove { Instances.Process.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the Persist command is invoked.
		/// </summary>
		public event Action<string> OnPersist
		{
			add { Instances.Persist.Invoked += value; }
			remove { Instances.Persist.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the Bind command is invoked.
		/// </summary>
		public event Action<InputTrigger, string> OnBind
		{
			add { Instances.Bind.Invoked += value; }
			remove { Instances.Bind.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the ShowConsole command is invoked.
		/// </summary>
		public event Action<bool> OnShowConsole
		{
			add { Instances.ShowConsole.Invoked += value; }
			remove { Instances.ShowConsole.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the ReloadAssets command is invoked.
		/// </summary>
		public event Action OnReloadAssets
		{
			add { Instances.ReloadAssets.Invoked += value; }
			remove { Instances.ReloadAssets.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the RestartGraphics command is invoked.
		/// </summary>
		public event Action OnRestartGraphics
		{
			add { Instances.RestartGraphics.Invoked += value; }
			remove { Instances.RestartGraphics.Invoked -= value; }
		}

		/// <summary>
		///   Initializes the registry.
		/// </summary>
		protected override void Initialize(object instances)
		{
			if (instances == null)
				instances = new InstanceList();

			Instances = (InstanceList)instances;
			base.Initialize(instances);

			Register(Instances.Exit, "exit");
			Register(Instances.Help, "help");
			Register(Instances.Cvars, "cvars");
			Register(Instances.Commands, "commands");
			Register(Instances.Execute, "execute");
			Register(Instances.Process, "process");
			Register(Instances.Persist, "persist");
			Register(Instances.Bind, "bind");
			Register(Instances.ShowConsole, "show_console");
			Register(Instances.ReloadAssets, "reload_assets");
			Register(Instances.RestartGraphics, "restart_graphics");
		}

		/// <summary>
		///   Stores the actual instances of the cvars or commands managed by the registry.
		/// </summary>
		public new class InstanceList : Registry<ICommand>.InstanceList
		{
			/// <summary>
			///   Initializes a new instance.
			/// </summary>
			public InstanceList()
			{
				Exit = new Command("exit", "Immediately exits the application.");
				Help = new Command<string>("help", "Describes the usage and the purpose of the the cvar or command with the given name. If no name is given, displays a help text about the usage of cvars and commands in general.", 
					new CommandParameter("name", typeof(string), true, "", "The name of the cvar or the command for which the description should be displayed."));
				Cvars = new Command<string>("cvars", "Lists all cvars with names that match the search pattern.", 
					new CommandParameter("pattern", typeof(string), true, "*", "The search pattern of the cvars that should be listed. For instance, 'draw' lists all cvars that have the string 'draw' in their name. The pattern is case-insensitive; use '*' to list all cvars.", new NotEmptyAttribute()));
				Commands = new Command<string>("commands", "Lists all commands with names that match the search pattern.", 
					new CommandParameter("pattern", typeof(string), true, "*", "The search pattern of the commands that should be listed. For instance, 'draw' lists all commands that have the string 'draw' in their name. The pattern is case-insensitive; use '*' to list all commands.", new NotEmptyAttribute()));
				Execute = new Command<string>("execute", "Executes the given command.", 
					new CommandParameter("command", typeof(string), false, default(string), "The command that should be executed, including its arguments.", new NotEmptyAttribute()));
				Process = new Command<string>("process", "Processes the commands in the given file.", 
					new CommandParameter("fileName", typeof(string), false, default(string), "The name of the file in the application's user directory that should be processed.", new NotEmptyAttribute(), new FileNameAttribute()));
				Persist = new Command<string>("persist", "Saves the persistent cvars into the given file.", 
					new CommandParameter("fileName", typeof(string), false, default(string), "The name of the file in the application's user directory the cvars should be written to.", new NotEmptyAttribute(), new FileNameAttribute()));
				Bind = new Command<InputTrigger, string>("bind", "Binds a command invocation to a logical input. Whenever the input is triggered, the command is invoked with the specified arguments.", 
					new CommandParameter("trigger", typeof(InputTrigger), false, default(InputTrigger), "The trigger that triggers the command."),
					new CommandParameter("command", typeof(string), false, default(string), "The command (including the arguments) that should be executed when the trigger is fired.", new NotEmptyAttribute()));
				ShowConsole = new Command<bool>("show_console", "Shows or hides the console.", 
					new CommandParameter("show", typeof(bool), false, default(bool), "A value of 'true' indicates that the console should be shown."));
				ReloadAssets = new Command("reload_assets", "Reloads all changed assets.");
				RestartGraphics = new Command("restart_graphics", "Restarts the graphics subsystem after a resolution or video mode change.");
			}

			/// <summary>
			///   Immediately exits the application.
			/// </summary>
			public Command Exit { get; private set; }

			/// <summary>
			///   Describes the usage and the purpose of the the cvar or command with the given name. If no name is given, displays a help text about the usage of cvars and commands in general.
			/// </summary>
			public Command<string> Help { get; private set; }

			/// <summary>
			///   Lists all cvars with names that match the search pattern.
			/// </summary>
			public Command<string> Cvars { get; private set; }

			/// <summary>
			///   Lists all commands with names that match the search pattern.
			/// </summary>
			public Command<string> Commands { get; private set; }

			/// <summary>
			///   Executes the given command.
			/// </summary>
			public Command<string> Execute { get; private set; }

			/// <summary>
			///   Processes the commands in the given file.
			/// </summary>
			public Command<string> Process { get; private set; }

			/// <summary>
			///   Saves the persistent cvars into the given file.
			/// </summary>
			public Command<string> Persist { get; private set; }

			/// <summary>
			///   Binds a command invocation to a logical input. Whenever the input is triggered, the command is invoked with the specified arguments.
			/// </summary>
			public Command<InputTrigger, string> Bind { get; private set; }

			/// <summary>
			///   Shows or hides the console.
			/// </summary>
			public Command<bool> ShowConsole { get; private set; }

			/// <summary>
			///   Reloads all changed assets.
			/// </summary>
			public Command ReloadAssets { get; private set; }

			/// <summary>
			///   Restarts the graphics subsystem after a resolution or video mode change.
			/// </summary>
			public Command RestartGraphics { get; private set; }
		}
	}
}

