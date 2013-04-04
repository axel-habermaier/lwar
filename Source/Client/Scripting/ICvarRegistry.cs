using System;

namespace Lwar.Client.Scripting
{
	using Pegasus.Framework.Scripting;

	/// <summary>
	///   Declares the lwar-specific cvars.
	/// </summary>
	public interface ICvarRegistry
	{
		/// <summary>
		///   If true, all 3D geometry is drawn in wireframe mode.
		/// </summary>
		[Cvar(false)]
		bool DrawWireframe { get; set; }
	}
}