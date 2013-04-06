using System;

namespace Lwar.Client.Scripting
{
	using Network;
	using Pegasus.Framework.Scripting;

	/// <summary>
	///   Declares the lwar-specific cvars.
	/// </summary>
	public interface ICvarRegistry
	{
		/// <summary>
		///   The name of the player that identifies the player in networked games.
		/// </summary>
		[Cvar("UnnamedPlayer"), Persistent, NotEmpty, MaximumLength(Specification.MaximumPlayerNameLength, true)]
		string PlayerName { get; set; }

		/// <summary>
		///   If true, all 3D geometry is drawn in wireframe mode.
		/// </summary>
		[Cvar(false)]
		bool DrawWireframe { get; set; }
	}
}