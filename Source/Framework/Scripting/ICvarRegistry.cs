using System;

namespace Pegasus.Framework.Scripting
{
	using Platform;
	using Platform.Input;

	public interface ICvarRegistry
	{
		/// <summary>
		///   The applications major version number.
		/// </summary>
		[Cvar(0)]
		int AppVersionMajor { get; set; }

		/// <summary>
		///   The applications minor version number.
		/// </summary>
		[Cvar(1)]
		int AppVersionMinor { get; set; }

		/// <summary>
		///   The application's name.
		/// </summary>
		[Cvar("")]
		string AppName { get; set; }

		/// <summary>
		///   The name of the player.
		/// </summary>
		[Cvar("UnnamedPlayer"), Persistent, UserChangeable]
		string PlayerName { get; set; }

		/// <summary>
		///   A cvar that indicates whether network debugging is enabled.
		/// </summary>
		[Cvar(PlatformInfo.IsDebug), Persistent, UserChangeable]
		bool NetworkDebugging { get; set; }

		/// <summary>
		///   The scaling factor that is applied to all timing values.
		/// </summary>
		[Cvar(1.0), UserChangeable]
		double TimeScaleFactor { get; set; }

		/// <summary>
		///   Exits the application when invoked.
		/// </summary>
		[Command]
		void Exit();

		/// <summary>
		///   Executes the given argument.
		/// </summary>
		/// <param name="command">The command that should be executed.</param>
		[Command]
		void Execute(string command);

		/// <summary>
		///   Binds a command invocation to a logical input. Whenever the input is triggered, the command is invoked with the
		///   specified arguments.
		/// </summary>
		/// <param name="trigger">The trigger that triggers the command.</param>
		/// <param name="command">The command (including the arguments) that should be executed when the trigger is fired.</param>
		[Command]
		void Bind(InputTrigger trigger, string command);

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