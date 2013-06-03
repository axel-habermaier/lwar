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
	///   When triggered, shows the scoreboard in an active game session.
	/// </summary>
	[Cvar(DefaultExpression = "Key.Tab.IsPressed()"), Persistent]
	InputTrigger InputShowScoreboard { get; set; }

	/// <summary>
	///   When triggered, moves the player forwards in an active game session.
	/// </summary>
	[Cvar(DefaultExpression = "Key.W.IsPressed() | Key.Up.IsPressed()"), Persistent]
	InputTrigger InputForward { get; set; }

	/// <summary>
	///   When triggered, moves the player backwards in an active game session.
	/// </summary>
	[Cvar(DefaultExpression = "Key.S.IsPressed() | Key.Down.IsPressed()"), Persistent]
	InputTrigger InputBackward { get; set; }

	/// <summary>
	///   When triggered, turns the player to the left in an active game session.
	/// </summary>
	[Cvar(DefaultExpression = "Key.A.IsPressed() | Key.Left.IsPressed()"), Persistent]
	InputTrigger InputTurnLeft { get; set; }

	/// <summary>
	///   When triggered, turns the player to the right in an active game session.
	/// </summary>
	[Cvar(DefaultExpression = "Key.D.IsPressed() | Key.Right.IsPressed()"), Persistent]
	InputTrigger InputTurnRight { get; set; }

	/// <summary>
	///   When triggered, moves the player to the left in an active game session.
	/// </summary>
	[Cvar(DefaultExpression = "Key.Q.IsPressed()"), Persistent]
	InputTrigger InputStrafeLeft { get; set; }

	/// <summary>
	///   When triggered, moves the player to the right in an active game session.
	/// </summary>
	[Cvar(DefaultExpression = "Key.E.IsPressed()"), Persistent]
	InputTrigger InputStrafeRight { get; set; }

	/// <summary>
	///   When triggered, fires the player's primary weapon in an active game session.
	/// </summary>
	[Cvar(DefaultExpression = "MouseButton.Left.IsPressed()"), Persistent]
	InputTrigger InputPrimaryWeapon { get; set; }

	/// <summary>
	///   When triggered, fires the player's secondary weapon in an active game session.
	/// </summary>
	[Cvar(DefaultExpression = "MouseButton.Right.IsPressed()"), Persistent]
	InputTrigger InputSecondaryWeapon { get; set; }

	/// <summary>
	///   When triggered, fires the player's tertiary weapon in an active game session.
	/// </summary>
	[Cvar(DefaultExpression = "Key.Num1.IsPressed()"), Persistent]
	InputTrigger InputTertiaryWeapon { get; set; }

	/// <summary>
	///   When triggered, fires the player's quaternary weapon in an active game session.
	/// </summary>
	[Cvar(DefaultExpression = "Key.Num2.IsPressed()"), Persistent]
	InputTrigger InputQuaternaryWeapon { get; set; }
}