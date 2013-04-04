using System;

namespace Pegasus.Framework.Scripting
{
	using Platform;

	/// <summary>
	///   Declares the cvars required by the framework.
	/// </summary>
	public interface ICvarRegistry
	{
		/// <summary>
		///   The name of the player that identifies the player in networked games.
		/// </summary>
		[Cvar("UnnamedPlayer"), Persistent, UserChangeable]
		string PlayerName { get; set; }

		/// <summary>
		///   If enabled, network debug output is printed to the console.
		/// </summary>
		[Cvar(PlatformInfo.IsDebug), Persistent, UserChangeable]
		bool NetworkDebugging { get; set; }

		/// <summary>
		///   The scaling factor that is applied to all timing values.
		/// </summary>
		[Cvar(1.0), UserChangeable]
		double TimeScale { get; set; }
	}
}