using System;

namespace Pegasus.Framework.Scripting.Requests
{
	using Platform;

	/// <summary>
	///   A user request that instructs the system to display the cvar's current value.
	/// </summary>
	internal sealed class DisplayCvar : CvarRequest
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="cvar">The cvar that should be displayed.</param>
		public DisplayCvar(ICvar cvar)
			: base(cvar)
		{
		}

		/// <summary>
		///   Executes the user command.
		/// </summary>
		public override void Execute()
		{
			Log.Info(Cvar.ToString());
		}
	}
}