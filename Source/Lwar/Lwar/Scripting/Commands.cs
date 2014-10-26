namespace Lwar.Scripting
{
	using System;
	using System.Diagnostics;
	using Pegasus.Utilities;
	using Lwar.Network;
	using Pegasus;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Network;
	using Pegasus.Scripting;
	using Pegasus.Scripting.Validators;
	using Pegasus.UserInterface.Input;

	internal static class Commands
	{
		/// <summary>
		///     Starts up a new local server instance. If a local server is currently running, it is shut down before the new server is started.
		/// </summary>
		public static Command StartServerCommand { get; private set; }

		/// <summary>
		///     Shuts down the currently running server.
		/// </summary>
		public static Command StopServerCommand { get; private set; }

		/// <summary>
		///     Connects to a game session on a remote or local server.
		/// </summary>
		public static Command<IPAddress, ushort> ConnectCommand { get; private set; }

		/// <summary>
		///     Disconnects from the current game session.
		/// </summary>
		public static Command DisconnectCommand { get; private set; }

		/// <summary>
		///     Sends a chat message to all peers.
		/// </summary>
		public static Command<string> SayCommand { get; private set; }

		/// <summary>
		///     Toggles between the game and the debugging camera.
		/// </summary>
		public static Command ToggleDebugCameraCommand { get; private set; }

		/// <summary>
		///     Immediately exits the application.
		/// </summary>
		public static Command ExitCommand { get; private set; }

		/// <summary>
		///     Describes the usage and the purpose of the the cvar or command with the given name. If no name is given, displays a help text about the usage of cvars and commands in general.
		/// </summary>
		public static Command<string> HelpCommand { get; private set; }

		/// <summary>
		///     Resets the given cvar to its default value.
		/// </summary>
		public static Command<string> ResetCommand { get; private set; }

		/// <summary>
		///     Lists all cvars with names that match the search pattern.
		/// </summary>
		public static Command<string> ListCvarsCommand { get; private set; }

		/// <summary>
		///     Lists all commands with names that match the search pattern.
		/// </summary>
		public static Command<string> ListCommandsCommand { get; private set; }

		/// <summary>
		///     Executes the given command.
		/// </summary>
		public static Command<string> ExecuteCommand { get; private set; }

		/// <summary>
		///     Processes the commands in the given file.
		/// </summary>
		public static Command<string> ProcessCommand { get; private set; }

		/// <summary>
		///     Saves the persistent cvars into the given file.
		/// </summary>
		public static Command<string> PersistCommand { get; private set; }

		/// <summary>
		///     Prints information about the application.
		/// </summary>
		public static Command PrintAppInfoCommand { get; private set; }

		/// <summary>
		///     Binds a command invocation to a logical input. Whenever the input is triggered, the command is invoked with the specified arguments.
		/// </summary>
		public static Command<InputTrigger, string> BindCommand { get; private set; }

		/// <summary>
		///     Unbinds all commands currently bound to a logical input.
		/// </summary>
		public static Command<InputTrigger> UnbindCommand { get; private set; }

		/// <summary>
		///     Removes all command bindings.
		/// </summary>
		public static Command UnbindAllCommand { get; private set; }

		/// <summary>
		///     Lists all active bindings.
		/// </summary>
		public static Command ListBindingsCommand { get; private set; }

		/// <summary>
		///     Shows or hides the console.
		/// </summary>
		public static Command<bool> ShowConsoleCommand { get; private set; }

		/// <summary>
		///     Shows the particle effect viewer.
		/// </summary>
		public static Command ShowParticleEffectViewerCommand { get; private set; }

		/// <summary>
		///     Reloads all currently loaded assets.
		/// </summary>
		public static Command ReloadAssetsCommand { get; private set; }

		/// <summary>
		///     Restarts the graphics subsystem after a resolution or video mode change.
		/// </summary>
		public static Command RestartGraphicsCommand { get; private set; }

		/// <summary>
		///     Toggles the value of a Boolean console variable.
		/// </summary>
		public static Command<string> ToggleCommand { get; private set; }

		/// <summary>
		///     Starts up a new local server instance. If a local server is currently running, it is shut down before the new server
		///     is started.
		/// </summary>
		[DebuggerHidden]
		public static void StartServer()
		{
			StartServerCommand.Invoke();
		}

		/// <summary>
		///     Shuts down the currently running server.
		/// </summary>
		[DebuggerHidden]
		public static void StopServer()
		{
			StopServerCommand.Invoke();
		}

		/// <summary>
		///     Connects to a game session on a remote or local server.
		/// </summary>
		/// <param name="ipAddress">The IP address of the server.</param>
		/// <param name="port">The port of the server.</param>
		[DebuggerHidden]
		public static void Connect(IPAddress ipAddress, ushort port = NetworkProtocol.DefaultServerPort)
		{
			Assert.ArgumentNotNull((object)ipAddress);
			Assert.ArgumentNotNull((object)port);
			ConnectCommand.Invoke(ipAddress, port);
		}

		/// <summary>
		///     Disconnects from the current game session.
		/// </summary>
		[DebuggerHidden]
		public static void Disconnect()
		{
			DisconnectCommand.Invoke();
		}

		/// <summary>
		///     Sends a chat message to all peers.
		/// </summary>
		/// <param name="message">The message that should be sent.</param>
		[DebuggerHidden]
		public static void Say(string message)
		{
			Assert.ArgumentNotNull((object)message);
			SayCommand.Invoke(message);
		}

		/// <summary>
		///     Toggles between the game and the debugging camera.
		/// </summary>
		[DebuggerHidden]
		public static void ToggleDebugCamera()
		{
			ToggleDebugCameraCommand.Invoke();
		}

		/// <summary>
		///     Immediately exits the application.
		/// </summary>
		[DebuggerHidden]
		public static void Exit()
		{
			ExitCommand.Invoke();
		}

		/// <summary>
		///     Describes the usage and the purpose of the the cvar or command with the given name. If no name is given, displays a
		///     help text about the usage of cvars and commands in general.
		/// </summary>
		/// <param name="name">The name of the cvar or the command for which the description should be displayed.</param>
		[DebuggerHidden]
		public static void Help(string name = "")
		{
			Assert.ArgumentNotNull((object)name);
			HelpCommand.Invoke(name);
		}

		/// <summary>
		///     Resets the given cvar to its default value.
		/// </summary>
		/// <param name="cvar">The name of the cvar that should be reset to its default value.</param>
		[DebuggerHidden]
		public static void Reset(string cvar)
		{
			Assert.ArgumentNotNull((object)cvar);
			ResetCommand.Invoke(cvar);
		}

		/// <summary>
		///     Lists all cvars with names that match the search pattern.
		/// </summary>
		/// <param name="pattern">
		///     The search pattern of the cvars that should be listed. For instance, "draw" lists all cvars that have the string
		///     "draw" in their name. The pattern is case-insensitive; use "*" to list all cvars.
		/// </param>
		[DebuggerHidden]
		public static void ListCvars(string pattern = "*")
		{
			Assert.ArgumentNotNull((object)pattern);
			ListCvarsCommand.Invoke(pattern);
		}

		/// <summary>
		///     Lists all commands with names that match the search pattern.
		/// </summary>
		/// <param name="pattern">
		///     The search pattern of the commands that should be listed. For instance, "draw" lists all commands that have the
		///     string "draw" in their name. The pattern is case-insensitive; use "*" to list all commands.
		/// </param>
		[DebuggerHidden]
		public static void ListCommands(string pattern = "*")
		{
			Assert.ArgumentNotNull((object)pattern);
			ListCommandsCommand.Invoke(pattern);
		}

		/// <summary>
		///     Executes the given command.
		/// </summary>
		/// <param name="command">The command that should be executed, including its arguments.</param>
		[DebuggerHidden]
		public static void Execute(string command)
		{
			Assert.ArgumentNotNull((object)command);
			ExecuteCommand.Invoke(command);
		}

		/// <summary>
		///     Processes the commands in the given file.
		/// </summary>
		/// <param name="fileName">The name of the file in the application's user directory that should be processed.</param>
		[DebuggerHidden]
		public static void Process(string fileName)
		{
			Assert.ArgumentNotNull((object)fileName);
			ProcessCommand.Invoke(fileName);
		}

		/// <summary>
		///     Saves the persistent cvars into the given file.
		/// </summary>
		/// <param name="fileName">The name of the file in the application's user directory the cvars should be written to.</param>
		[DebuggerHidden]
		public static void Persist(string fileName)
		{
			Assert.ArgumentNotNull((object)fileName);
			PersistCommand.Invoke(fileName);
		}

		/// <summary>
		///     Prints information about the application.
		/// </summary>
		[DebuggerHidden]
		public static void PrintAppInfo()
		{
			PrintAppInfoCommand.Invoke();
		}

		/// <summary>
		///     Binds a command invocation to a logical input. Whenever the input is triggered, the command is invoked with the
		///     specified arguments.
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
		///     Unbinds all commands currently bound to a logical input.
		/// </summary>
		/// <param name="trigger">The trigger that should be unbound.</param>
		[DebuggerHidden]
		public static void Unbind(InputTrigger trigger)
		{
			Assert.ArgumentNotNull((object)trigger);
			UnbindCommand.Invoke(trigger);
		}

		/// <summary>
		///     Removes all command bindings.
		/// </summary>
		[DebuggerHidden]
		public static void UnbindAll()
		{
			UnbindAllCommand.Invoke();
		}

		/// <summary>
		///     Lists all active bindings.
		/// </summary>
		[DebuggerHidden]
		public static void ListBindings()
		{
			ListBindingsCommand.Invoke();
		}

		/// <summary>
		///     Shows or hides the console.
		/// </summary>
		/// <param name="show">A value of 'true' indicates that the console should be shown.</param>
		[DebuggerHidden]
		public static void ShowConsole(bool show)
		{
			Assert.ArgumentNotNull((object)show);
			ShowConsoleCommand.Invoke(show);
		}

		/// <summary>
		///     Shows the particle effect viewer.
		/// </summary>
		[DebuggerHidden]
		public static void ShowParticleEffectViewer()
		{
			ShowParticleEffectViewerCommand.Invoke();
		}

		/// <summary>
		///     Reloads all currently loaded assets.
		/// </summary>
		[DebuggerHidden]
		public static void ReloadAssets()
		{
			ReloadAssetsCommand.Invoke();
		}

		/// <summary>
		///     Restarts the graphics subsystem after a resolution or video mode change.
		/// </summary>
		[DebuggerHidden]
		public static void RestartGraphics()
		{
			RestartGraphicsCommand.Invoke();
		}

		/// <summary>
		///     Toggles the value of a Boolean console variable.
		/// </summary>
		/// <param name="cvar">The name of console variable whose value should be toggled.</param>
		[DebuggerHidden]
		public static void Toggle(string cvar)
		{
			Assert.ArgumentNotNull((object)cvar);
			ToggleCommand.Invoke(cvar);
		}

		/// <summary>
		///     Raised when the 'StartServer' command is invoked.
		/// </summary>
		public static event Action OnStartServer
		{
			add { StartServerCommand.Invoked += value; }
			remove { StartServerCommand.Invoked -= value; }
		}

		/// <summary>
		///     Raised when the 'StopServer' command is invoked.
		/// </summary>
		public static event Action OnStopServer
		{
			add { StopServerCommand.Invoked += value; }
			remove { StopServerCommand.Invoked -= value; }
		}

		/// <summary>
		///     Raised when the 'Connect' command is invoked.
		/// </summary>
		public static event Action<IPAddress, ushort> OnConnect
		{
			add { ConnectCommand.Invoked += value; }
			remove { ConnectCommand.Invoked -= value; }
		}

		/// <summary>
		///     Raised when the 'Disconnect' command is invoked.
		/// </summary>
		public static event Action OnDisconnect
		{
			add { DisconnectCommand.Invoked += value; }
			remove { DisconnectCommand.Invoked -= value; }
		}

		/// <summary>
		///     Raised when the 'Say' command is invoked.
		/// </summary>
		public static event Action<string> OnSay
		{
			add { SayCommand.Invoked += value; }
			remove { SayCommand.Invoked -= value; }
		}

		/// <summary>
		///     Raised when the 'ToggleDebugCamera' command is invoked.
		/// </summary>
		public static event Action OnToggleDebugCamera
		{
			add { ToggleDebugCameraCommand.Invoked += value; }
			remove { ToggleDebugCameraCommand.Invoked -= value; }
		}

		/// <summary>
		///     Raised when the 'Exit' command is invoked.
		/// </summary>
		public static event Action OnExit
		{
			add { ExitCommand.Invoked += value; }
			remove { ExitCommand.Invoked -= value; }
		}

		/// <summary>
		///     Raised when the 'Help' command is invoked.
		/// </summary>
		public static event Action<string> OnHelp
		{
			add { HelpCommand.Invoked += value; }
			remove { HelpCommand.Invoked -= value; }
		}

		/// <summary>
		///     Raised when the 'Reset' command is invoked.
		/// </summary>
		public static event Action<string> OnReset
		{
			add { ResetCommand.Invoked += value; }
			remove { ResetCommand.Invoked -= value; }
		}

		/// <summary>
		///     Raised when the 'ListCvars' command is invoked.
		/// </summary>
		public static event Action<string> OnListCvars
		{
			add { ListCvarsCommand.Invoked += value; }
			remove { ListCvarsCommand.Invoked -= value; }
		}

		/// <summary>
		///     Raised when the 'ListCommands' command is invoked.
		/// </summary>
		public static event Action<string> OnListCommands
		{
			add { ListCommandsCommand.Invoked += value; }
			remove { ListCommandsCommand.Invoked -= value; }
		}

		/// <summary>
		///     Raised when the 'Execute' command is invoked.
		/// </summary>
		public static event Action<string> OnExecute
		{
			add { ExecuteCommand.Invoked += value; }
			remove { ExecuteCommand.Invoked -= value; }
		}

		/// <summary>
		///     Raised when the 'Process' command is invoked.
		/// </summary>
		public static event Action<string> OnProcess
		{
			add { ProcessCommand.Invoked += value; }
			remove { ProcessCommand.Invoked -= value; }
		}

		/// <summary>
		///     Raised when the 'Persist' command is invoked.
		/// </summary>
		public static event Action<string> OnPersist
		{
			add { PersistCommand.Invoked += value; }
			remove { PersistCommand.Invoked -= value; }
		}

		/// <summary>
		///     Raised when the 'PrintAppInfo' command is invoked.
		/// </summary>
		public static event Action OnPrintAppInfo
		{
			add { PrintAppInfoCommand.Invoked += value; }
			remove { PrintAppInfoCommand.Invoked -= value; }
		}

		/// <summary>
		///     Raised when the 'Bind' command is invoked.
		/// </summary>
		public static event Action<InputTrigger, string> OnBind
		{
			add { BindCommand.Invoked += value; }
			remove { BindCommand.Invoked -= value; }
		}

		/// <summary>
		///     Raised when the 'Unbind' command is invoked.
		/// </summary>
		public static event Action<InputTrigger> OnUnbind
		{
			add { UnbindCommand.Invoked += value; }
			remove { UnbindCommand.Invoked -= value; }
		}

		/// <summary>
		///     Raised when the 'UnbindAll' command is invoked.
		/// </summary>
		public static event Action OnUnbindAll
		{
			add { UnbindAllCommand.Invoked += value; }
			remove { UnbindAllCommand.Invoked -= value; }
		}

		/// <summary>
		///     Raised when the 'ListBindings' command is invoked.
		/// </summary>
		public static event Action OnListBindings
		{
			add { ListBindingsCommand.Invoked += value; }
			remove { ListBindingsCommand.Invoked -= value; }
		}

		/// <summary>
		///     Raised when the 'ShowConsole' command is invoked.
		/// </summary>
		public static event Action<bool> OnShowConsole
		{
			add { ShowConsoleCommand.Invoked += value; }
			remove { ShowConsoleCommand.Invoked -= value; }
		}

		/// <summary>
		///     Raised when the 'ShowParticleEffectViewer' command is invoked.
		/// </summary>
		public static event Action OnShowParticleEffectViewer
		{
			add { ShowParticleEffectViewerCommand.Invoked += value; }
			remove { ShowParticleEffectViewerCommand.Invoked -= value; }
		}

		/// <summary>
		///     Raised when the 'ReloadAssets' command is invoked.
		/// </summary>
		public static event Action OnReloadAssets
		{
			add { ReloadAssetsCommand.Invoked += value; }
			remove { ReloadAssetsCommand.Invoked -= value; }
		}

		/// <summary>
		///     Raised when the 'RestartGraphics' command is invoked.
		/// </summary>
		public static event Action OnRestartGraphics
		{
			add { RestartGraphicsCommand.Invoked += value; }
			remove { RestartGraphicsCommand.Invoked -= value; }
		}

		/// <summary>
		///     Raised when the 'Toggle' command is invoked.
		/// </summary>
		public static event Action<string> OnToggle
		{
			add { ToggleCommand.Invoked += value; }
			remove { ToggleCommand.Invoked -= value; }
		}

		/// <summary>
		///     Initializes the instances declared by the registry.
		/// </summary>
		public static void Initialize()
		{
			StartServerCommand = new Command("start_server", "Starts up a new local server instance. If a local server is currently running, it is shut down before the new server is started.", false);
			StopServerCommand = new Command("stop_server", "Shuts down the currently running server.", false);
			ConnectCommand = new Command<IPAddress, ushort>("connect", "Connects to a game session on a remote or local server.", false, 
				new CommandParameter("ipAddress", typeof(IPAddress), false, default(IPAddress), "The IP address of the server."),
				new CommandParameter("port", typeof(ushort), true, NetworkProtocol.DefaultServerPort, "The port of the server."));
			DisconnectCommand = new Command("disconnect", "Disconnects from the current game session.", false);
			SayCommand = new Command<string>("say", "Sends a chat message to all peers.", false, 
				new CommandParameter("message", typeof(string), false, default(string), "The message that should be sent.", new NotEmptyAttribute(), new MaximumLengthAttribute(NetworkProtocol.ChatMessageLength, true)));
			ToggleDebugCameraCommand = new Command("toggle_debug_camera", "Toggles between the game and the debugging camera.", false);

			CommandRegistry.Register(StartServerCommand);
			CommandRegistry.Register(StopServerCommand);
			CommandRegistry.Register(ConnectCommand);
			CommandRegistry.Register(DisconnectCommand);
			CommandRegistry.Register(SayCommand);
			CommandRegistry.Register(ToggleDebugCameraCommand);
		}

		/// <summary>
		///     Initializes the instances imported by the registry.
		/// </summary>
		public static void Resolve()
		{
			ExitCommand = CommandRegistry.Resolve("exit");
			HelpCommand = CommandRegistry.Resolve<string>("help");
			ResetCommand = CommandRegistry.Resolve<string>("reset");
			ListCvarsCommand = CommandRegistry.Resolve<string>("list_cvars");
			ListCommandsCommand = CommandRegistry.Resolve<string>("list_commands");
			ExecuteCommand = CommandRegistry.Resolve<string>("execute");
			ProcessCommand = CommandRegistry.Resolve<string>("process");
			PersistCommand = CommandRegistry.Resolve<string>("persist");
			PrintAppInfoCommand = CommandRegistry.Resolve("print_app_info");
			BindCommand = CommandRegistry.Resolve<InputTrigger, string>("bind");
			UnbindCommand = CommandRegistry.Resolve<InputTrigger>("unbind");
			UnbindAllCommand = CommandRegistry.Resolve("unbind_all");
			ListBindingsCommand = CommandRegistry.Resolve("list_bindings");
			ShowConsoleCommand = CommandRegistry.Resolve<bool>("show_console");
			ShowParticleEffectViewerCommand = CommandRegistry.Resolve("show_particle_effect_viewer");
			ReloadAssetsCommand = CommandRegistry.Resolve("reload_assets");
			RestartGraphicsCommand = CommandRegistry.Resolve("restart_graphics");
			ToggleCommand = CommandRegistry.Resolve<string>("toggle");
		}
	}
}

