using System;

namespace Pegasus.Framework.Scripting
{
	using Platform;

	/// <summary>
	///   Provides access to all build-in cvars.
	/// </summary>
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
		public static readonly Cvar<string> AppName = new Cvar<string>("app_name", "Pegasus", "The name of the application.");

		/// <summary>
		///   The name of the player.
		/// </summary>
		public static readonly Cvar<string> PlayerName = new Cvar<string>("name", "Unnamed", "The name of the player.");

		/// <summary>
		///   A cvar that indicates whether network debugging is enabled.
		/// </summary>
		public static readonly Cvar<bool> NetworkDebugging = new Cvar<bool>("net_debug", PlatformInfo.IsDebug,
																			"If true, prints out network debugging information.");
	}
}