using System;

namespace Lwar.Client
{
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Scripting;

	/// <summary>
	///   Provides access to lwar-specific cvars.
	/// </summary>
	[ForceInitialization]
	public static class LwarCvars
	{
		/// <summary>
		///   If true, all 3D geometry is drawn in wireframe mode.
		/// </summary>
		public static Cvar<bool> DrawWireframe = new Cvar<bool>("r_wireframe", false,
																"If true, all 3D geometry is drawn in wireframe mode.");
	}
}