using System;

namespace Pegasus.Framework.Scripting
{
	using System.Linq;

	public partial class CvarRegistry
	{
		/// <summary>
		///   Executes all deferred cvar updates.
		/// </summary>
		/// <param name="mode">The mode of the cvars that should be updated.</param>
		public void ExecuteDeferredUpdates(UpdateMode mode)
		{
			Assert.InRange(mode);

			foreach (var cvar in AllInstances.Where(cvar => cvar.UpdateMode == mode && cvar.HasDeferredValue))
				cvar.SetDeferredValue();
		}
	}
}