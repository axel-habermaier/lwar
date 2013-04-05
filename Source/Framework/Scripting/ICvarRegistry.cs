using System;

namespace Pegasus.Framework.Scripting
{
	/// <summary>
	///   Declares the cvars required by the framework.
	/// </summary>
	public interface ICvarRegistry
	{
		/// <summary>
		///   The name of the player that identifies the player in networked games.
		/// </summary>
		[Cvar("UnnamedPlayer"), Persistent]
		string PlayerName { get; set; }

		/// <summary>
		///   The scaling factor that is applied to all timing values.
		/// </summary>
		[Cvar(1.0)]
		double TimeScale { get; set; }
	}
}