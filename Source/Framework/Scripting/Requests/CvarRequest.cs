using System;

namespace Pegasus.Framework.Scripting.Requests
{
	/// <summary>
	///   A user request that affects a cvar.
	/// </summary>
	internal abstract class CvarRequest : IRequest
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="cvar"> The cvar that should be affected by the request.</param>
		protected CvarRequest(ICvar cvar)
		{
			Assert.ArgumentNotNull(cvar, () => cvar);
			Cvar = cvar;
		}

		/// <summary>
		///   Gets the cvar that is affected by the command.
		/// </summary>
		public ICvar Cvar { get; private set; }

		/// <summary>
		///   Executes the user command.
		/// </summary>
		public abstract void Execute();
	}
}