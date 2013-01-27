using System;

namespace Pegasus.Framework.Scripting.Requests
{
	using Platform;

	/// <summary>
	///   A user request that instructs the system to show a cvar's help description.
	/// </summary>
	internal sealed class DescribeCvar : CvarRequest
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="cvar">The cvar that should be described.</param>
		public DescribeCvar(ICvar cvar)
			: base(cvar)
		{
		}

		/// <summary>
		///   Executes the user command.
		/// </summary>
		public override void Execute()
		{
			Log.Info("'{0}': {1}", Cvar.Name, Cvar.Description);
		}
	}
}