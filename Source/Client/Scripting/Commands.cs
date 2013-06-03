﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Generated by the Pegasus Asset Compiler.
//     Tuesday, June 4, 2013, 1:48:27
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Lwar.Client.Scripting
{
	using Lwar.Client.Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform.Input;
	using Pegasus.Framework.Platform.Logging;
	using Pegasus.Framework.Scripting;
	using System.Net;

	internal static class Commands
	{
		/// <summary>
		///   Starts up a new local server instance. If a local server is currently running, it is shut down before the new server is started.
		/// </summary>
		public static Command StartServerCvar { get; private set; }

		/// <summary>
		///   Shuts down the currently running server.
		/// </summary>
		public static Command StopServerCvar { get; private set; }

		/// <summary>
		///   Connects to a game session on a remote or local server.
		/// </summary>
		public static Command<IPAddress, ushort> ConnectCvar { get; private set; }

		/// <summary>
		///   Disconnects from the current game session.
		/// </summary>
		public static Command DisconnectCvar { get; private set; }

		/// <summary>
		///   Sends a chat message to all peers.
		/// </summary>
		public static Command<string> SayCvar { get; private set; }

		/// <summary>
		///   Toggles between the game and the debugging camera.
		/// </summary>
		public static Command ToggleDebugCameraCvar { get; private set; }

		/// <summary>
		///   Immediately exits the application.
		/// </summary>
		public static Command ExitCvar { get; private set; }

		/// <summary>
		///   Describes the usage and the purpose of the the cvar or command with the given name. If no name is given, displays a help text about the usage of cvars and commands in general.
		/// </summary>
		public static Command<string> HelpCvar { get; private set; }

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
		///   Starts up a new local server instance. If a local server is currently running, it is shut down before the new server
		///   is started.
		/// </summary>
		[DebuggerHidden]
		public static void StartServer()
		{
			StartServerCvar.Invoke();
		}

		/// <summary>
		///   Shuts down the currently running server.
		/// </summary>
		[DebuggerHidden]
		public static void StopServer()
		{
			StopServerCvar.Invoke();
		}

		/// <summary>
		///   Connects to a game session on a remote or local server.
		/// </summary>
		/// <param name="ipAddress">
		///   The IP address of the server in either IPv4 or IPv6 format. For instance, either 127.0.0.1 or ::1 can be used to
		///   connect to a local server.
		/// </param>
		/// <param name="port">The port of the server.</param>
		[DebuggerHidden]
		public static void Connect(IPAddress ipAddress, ushort port = Specification.DefaultServerPort)
		{
			Assert.ArgumentNotNull((object)ipAddress);
			Assert.ArgumentNotNull((object)port);
			ConnectCvar.Invoke(ipAddress, port);
		}

		/// <summary>
		///   Disconnects from the current game session.
		/// </summary>
		[DebuggerHidden]
		public static void Disconnect()
		{
			DisconnectCvar.Invoke();
		}

		/// <summary>
		///   Sends a chat message to all peers.
		/// </summary>
		/// <param name="message">The message that should be sent.</param>
		[DebuggerHidden]
		public static void Say(string message)
		{
			Assert.ArgumentNotNull((object)message);
			SayCvar.Invoke(message);
		}

		/// <summary>
		///   Toggles between the game and the debugging camera.
		/// </summary>
		[DebuggerHidden]
		public static void ToggleDebugCamera()
		{
			ToggleDebugCameraCvar.Invoke();
		}

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
		///   Raised when the 'StartServer' command is invoked.
		/// </summary>
		public static event Action OnStartServer
		{
			add { StartServerCvar.Invoked += value; }
			remove { StartServerCvar.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'StopServer' command is invoked.
		/// </summary>
		public static event Action OnStopServer
		{
			add { StopServerCvar.Invoked += value; }
			remove { StopServerCvar.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'Connect' command is invoked.
		/// </summary>
		public static event Action<IPAddress, ushort> OnConnect
		{
			add { ConnectCvar.Invoked += value; }
			remove { ConnectCvar.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'Disconnect' command is invoked.
		/// </summary>
		public static event Action OnDisconnect
		{
			add { DisconnectCvar.Invoked += value; }
			remove { DisconnectCvar.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'Say' command is invoked.
		/// </summary>
		public static event Action<string> OnSay
		{
			add { SayCvar.Invoked += value; }
			remove { SayCvar.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the 'ToggleDebugCamera' command is invoked.
		/// </summary>
		public static event Action OnToggleDebugCamera
		{
			add { ToggleDebugCameraCvar.Invoked += value; }
			remove { ToggleDebugCameraCvar.Invoked -= value; }
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
			StartServerCvar = new Command("start_server", "Starts up a new local server instance. If a local server is currently running, it is shut down before the new server is started.");
			StopServerCvar = new Command("stop_server", "Shuts down the currently running server.");
			ConnectCvar = new Command<IPAddress, ushort>("connect", "Connects to a game session on a remote or local server.", 
				new CommandParameter("ipAddress", typeof(IPAddress), false, default(IPAddress), "The IP address of the server in either IPv4 or IPv6 format. For instance, either 127.0.0.1 or ::1 can be used to connect to a local server."),
				new CommandParameter("port", typeof(ushort), true, Specification.DefaultServerPort, "The port of the server."));
			DisconnectCvar = new Command("disconnect", "Disconnects from the current game session.");
			SayCvar = new Command<string>("say", "Sends a chat message to all peers.", 
				new CommandParameter("message", typeof(string), false, default(string), "The message that should be sent.", new NotEmptyAttribute(), new MaximumLengthAttribute(Specification.MaximumChatMessageLength, true)));
			ToggleDebugCameraCvar = new Command("toggle_debug_camera", "Toggles between the game and the debugging camera.");

			CommandRegistry.Register(StartServerCvar);
			CommandRegistry.Register(StopServerCvar);
			CommandRegistry.Register(ConnectCvar);
			CommandRegistry.Register(DisconnectCvar);
			CommandRegistry.Register(SayCvar);
			CommandRegistry.Register(ToggleDebugCameraCvar);
		}

		/// <summary>
		///   Initializes the instances imported by the registry.
		/// </summary>
		public static void Resolve()
		{
			ExitCvar = CommandRegistry.Resolve("exit");
			HelpCvar = CommandRegistry.Resolve<string>("help");
			ListCvarsCvar = CommandRegistry.Resolve<string>("list_cvars");
			ListCommandsCvar = CommandRegistry.Resolve<string>("list_commands");
			ExecuteCvar = CommandRegistry.Resolve<string>("execute");
			ProcessCvar = CommandRegistry.Resolve<string>("process");
			PersistCvar = CommandRegistry.Resolve<string>("persist");
			BindCvar = CommandRegistry.Resolve<InputTrigger, string>("bind");
			ShowConsoleCvar = CommandRegistry.Resolve<bool>("show_console");
			ToggleStatsCvar = CommandRegistry.Resolve("toggle_stats");
			ReloadAssetsCvar = CommandRegistry.Resolve("reload_assets");
			RestartGraphicsCvar = CommandRegistry.Resolve("restart_graphics");
		}
	}
}

