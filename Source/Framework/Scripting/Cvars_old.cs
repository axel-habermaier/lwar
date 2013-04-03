using System;

namespace Pegasus.Framework.Scripting
{
	using Platform;
	using Platform.Input;

	/// <summary>
	///   Provides access to all build-in cvars.
	/// </summary>
	[ForceInitialization]
	public static class Cvars
	{
		/// <summary>
		///   The applications major version number.
		/// </summary>
		public static readonly Cvar<int> AppVersionMajor = new Cvar<int>("app_versionMajor", 0,
																		 "The major version of the application.");

		/// <summary>
		///   The applications minor version number.
		/// </summary>
		public static readonly Cvar<int> AppVersionMinor = new Cvar<int>("app_versionMinor", 1,
																		 "The minor version of the application.");

		/// <summary>
		///   The application's name.
		/// </summary>
		public static readonly Cvar<string> AppName = new Cvar<string>("app_name", "", "The name of the application.");

		/// <summary>
		///   The name of the player.
		/// </summary>
		public static readonly Cvar<string> PlayerName = new Cvar<string>("name", "UnnamedPlayer", "The name of the player.");

		/// <summary>
		///   A cvar that indicates whether network debugging is enabled.
		/// </summary>
		public static readonly Cvar<bool> NetworkDebugging = new Cvar<bool>("net_debug", PlatformInfo.IsDebug,
																			"If true, prints out network debugging information.");

		/// <summary>
		///   The scaling factor that is applied to all timing values.
		/// </summary>
		public static readonly Cvar<double> TimeScaleFactor = new Cvar<double>("time_scale", 1.0,
																			   "The scaling factor that is applied to all timing values.");
	}

	internal interface IOtherIdea
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
		[Cvar("UnnamedPlayer"), Persistent, UserChangable]
		string PlayerName { get; set; }

		/// <summary>
		///   A cvar that indicates whether network debugging is enabled.
		/// </summary>
		[Cvar(PlatformInfo.IsDebug), Persistent, UserChangable]
		bool NetworkDebugging { get; set; }

		/// <summary>
		///   The scaling factor that is applied to all timing values.
		/// </summary>
		[Cvar(1.0), UserChangable]
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


	[MeansImplicitUse]
	internal class CommandAttribute : Attribute
	{
		public CommandAttribute(string name = "")
		{
		}
	}

	internal enum CvarFlags
	{
		NoUserChange,
		Persist
	}

	[MeansImplicitUse]
	internal class Cvar : Attribute
	{
		public Cvar(object o)
		{
		}

		public object DefaultValue { get; set; }
	}

	internal class ParameterDescriptions : Attribute
	{
		public ParameterDescriptions(params string[] descs)
		{
		}
	}

	internal class ParameterNames : Attribute
	{
		public ParameterNames(params string[] descs)
		{
		}
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	internal class Parameter : Attribute
	{
		public Parameter(int i, string n, string d)
		{
		}

		public object DefaultValue { get; set; }
	}

	internal class Persistent : Attribute
	{
	}

	internal class UserChangable : Attribute
	{
	}

	internal class Hidden : Attribute
	{
	}

	internal class Range : Attribute
	{
		public Range(object from, object to)
		{
		}
	}
}