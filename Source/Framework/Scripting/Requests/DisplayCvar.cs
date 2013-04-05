using System;

namespace Pegasus.Framework.Scripting.Requests
{
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
			Log.Info("'{0}' is '{1}', default '{2}' [{3}]", Cvar.Name, Cvar.StringValue, Cvar.DefaultValue,
					 TypeDescription.GetDescription(Cvar.ValueType));
		}
	}
}