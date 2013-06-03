using System;
using Lwar.Client.Network;
using Pegasus.Framework.Platform.Input;
using Pegasus.Framework.Scripting;

// ReSharper disable CheckNamespace

/// <summary>
///   Declares the lwar-specific cvars.
/// </summary>
internal interface ICvars
{
	/// <summary>
	///   The name of the player.
	/// </summary>
	[Cvar("UnnamedPlayer"), Persistent, NotEmpty, MaximumLength(Specification.MaximumPlayerNameLength, true)]
	string PlayerName { get; set; }

	/// <summary>
	///   If true, all 3D geometry is drawn in wireframe mode.
	/// </summary>
	[Cvar(false)]
	bool DrawWireframe { get; set; }

	/// <summary>
	/// The key that show s us
	/// </summary>
	[Cvar(DefaultExpression = "Key.Tab.WentDown()")]
	InputTrigger KeyShowScoreboard { get; set; }
}