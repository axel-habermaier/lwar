using System;

namespace Pegasus.Framework.Scripting
{
	using Platform;
	using Platform.Input;

	/// <summary>
	///   Provides access to cvars.
	/// </summary>
	public class CvarRegistry2 : CvarRegistry
	{
		/// <summary>
		///   The applications major version number.
		/// </summary>
		private readonly Cvar<int> _appVersionMajor;

		/// <summary>
		///   The applications minor version number.
		/// </summary>
		private readonly Cvar<int> _appVersionMinor;

		/// <summary>
		///   The application's name.
		/// </summary>
		private readonly Cvar<string> _appName;

		/// <summary>
		///   The name of the player.
		/// </summary>
		private readonly Cvar<string> _playerName;

		/// <summary>
		///   A cvar that indicates whether network debugging is enabled.
		/// </summary>
		private readonly Cvar<bool> _networkDebugging;

		/// <summary>
		///   The scaling factor that is applied to all timing values.
		/// </summary>
		private readonly Cvar<double> _timeScaleFactor;

		/// <summary>
		///   Exits the application when invoked.
		/// </summary>
		private readonly Command _exit;

		/// <summary>
		///   Executes the given argument.
		/// </summary>
		private readonly Command<string> _execute;

		/// <summary>
		///   Binds a command invocation to a logical input. Whenever the input is triggered, the command is invoked with the
		///   specified arguments.
		/// </summary>
		private readonly Command<InputTrigger, string> _bind;

		/// <summary>
		///   Shows or hides the console.
		/// </summary>
		private readonly Command<bool> _showConsole;

		/// <summary>
		///   Reloads all changed assets.
		/// </summary>
		private readonly Command _reloadAssets;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public CvarRegistry2()
		{
			_appVersionMajor = new Cvar<int>("app_version_major", 0, "The applications major version number.");
			_appVersionMinor = new Cvar<int>("app_version_minor", 1, "The applications minor version number.");
			_appName = new Cvar<string>("app_name", "", "The application's name.");
			_playerName = new Cvar<string>("player_name", "UnnamedPlayer", "The name of the player.");
			_networkDebugging = new Cvar<bool>("network_debugging", PlatformInfo.IsDebug, "A cvar that indicates whether network debugging is enabled.");
			_timeScaleFactor = new Cvar<double>("time_scale_factor", 1.0, "The scaling factor that is applied to all timing values.");

			_exit = new Command("exit", "Exits the application when invoked.");
			_execute = new Command<string>("execute", "Executes the given argument.");
			_bind = new Command<InputTrigger, string>("bind", "Binds a command invocation to a logical input. Whenever the input is triggered, the command is invoked with the specified arguments.");
			_showConsole = new Command<bool>("show_console", "Shows or hides the console.");
			_reloadAssets = new Command("reload_assets", "Reloads all changed assets.");

			Register(_appVersionMajor);
			Register(_appVersionMinor);
			Register(_appName);
			Register(_playerName);
			Register(_networkDebugging);
			Register(_timeScaleFactor);

			Register(_exit);
			Register(_execute);
			Register(_bind);
			Register(_showConsole);
			Register(_reloadAssets);
		}

		/// <summary>
		///   The applications major version number.
		/// </summary>
		public int AppVersionMajor
		{
			get { return _appVersionMajor.Value; }
			set { _appVersionMajor.Value = value; }
		}

		/// <summary>
		///   The applications minor version number.
		/// </summary>
		public int AppVersionMinor
		{
			get { return _appVersionMinor.Value; }
			set { _appVersionMinor.Value = value; }
		}

		/// <summary>
		///   The application's name.
		/// </summary>
		public string AppName
		{
			get { return _appName.Value; }
			set { _appName.Value = value; }
		}

		/// <summary>
		///   The name of the player.
		/// </summary>
		public string PlayerName
		{
			get { return _playerName.Value; }
			set { _playerName.Value = value; }
		}

		/// <summary>
		///   A cvar that indicates whether network debugging is enabled.
		/// </summary>
		public bool NetworkDebugging
		{
			get { return _networkDebugging.Value; }
			set { _networkDebugging.Value = value; }
		}

		/// <summary>
		///   The scaling factor that is applied to all timing values.
		/// </summary>
		public double TimeScaleFactor
		{
			get { return _timeScaleFactor.Value; }
			set { _timeScaleFactor.Value = value; }
		}

		/// <summary>
		///   Exits the application when invoked.
		/// </summary>
		public void Exit()
		{
			_exit.Invoke();
		}

		/// <summary>
		///   Executes the given argument.
		/// </summary>
		/// <param name="command">The command that should be executed.</param>
		public void Execute(string command)
		{
			_execute.Invoke(command);
		}

		/// <summary>
		///   Binds a command invocation to a logical input. Whenever the input is triggered, the command is invoked with the
		///   specified arguments.
		/// </summary>
		/// <param name="trigger">The trigger that triggers the command.</param>
		/// <param name="command">The command (including the arguments) that should be executed when the trigger is fired.</param>
		public void Bind(InputTrigger trigger, string command)
		{
			_bind.Invoke(trigger, command);
		}

		/// <summary>
		///   Shows or hides the console.
		/// </summary>
		/// <param name="show">A value of 'true' indicates that the console should be shown.</param>
		public void ShowConsole(bool show)
		{
			_showConsole.Invoke(show);
		}

		/// <summary>
		///   Reloads all changed assets.
		/// </summary>
		public void ReloadAssets()
		{
			_reloadAssets.Invoke();
		}

		/// <summary>
		///   Exits the application when invoked.
		/// </summary>
		public event Action OnExit
		{
			add { _exit.Invoked += value; }
			remove { _exit.Invoked -= value; }
		}

		/// <summary>
		///   Executes the given argument.
		/// </summary>
		public event Action<string> OnExecute
		{
			add { _execute.Invoked += value; }
			remove { _execute.Invoked -= value; }
		}

		/// <summary>
		///   Binds a command invocation to a logical input. Whenever the input is triggered, the command is invoked with the
		///   specified arguments.
		/// </summary>
		public event Action<InputTrigger, string> OnBind
		{
			add { _bind.Invoked += value; }
			remove { _bind.Invoked -= value; }
		}

		/// <summary>
		///   Shows or hides the console.
		/// </summary>
		public event Action<bool> OnShowConsole
		{
			add { _showConsole.Invoked += value; }
			remove { _showConsole.Invoked -= value; }
		}

		/// <summary>
		///   Reloads all changed assets.
		/// </summary>
		public event Action OnReloadAssets
		{
			add { _reloadAssets.Invoked += value; }
			remove { _reloadAssets.Invoked -= value; }
		}
	}
}

