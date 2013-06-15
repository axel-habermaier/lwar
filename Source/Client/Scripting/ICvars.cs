using System;
using Lwar.Client.Network;
using Pegasus.Framework.Platform.Input;
using Pegasus.Framework.Scripting;
using Pegasus.Framework.Scripting.Validators;

// ReSharper disable CheckNamespace

/// <summary>
///   Declares the lwar-specific cvars.
/// </summary>
internal interface ICvars
{
	/// <summary>
	///   The name of the player.
	/// </summary>
	[Cvar("UnnamedPlayer"), Persistent, NotEmpty, MaximumLength(Specification.PlayerNameLength, true)]
	string PlayerName { get; set; }

	/// <summary>
	///   The display time (in seconds) of event messages such as 'X killed Y', 'X joined the game', etc.
	/// </summary>
	[Cvar(3), Persistent, Range(0.5, 60.0)]
	double EventMessageDisplayTime { get; set; }

	/// <summary>
	///   The display time (in seconds) of chat messages.
	/// </summary>
	[Cvar(6), Persistent, Range(0.5, 60.0)]
	double ChatMessageDisplayTime { get; set; }

	/// <summary>
	///   When triggered in an active game session, shows the scoreboard.
	/// </summary>
	[Cvar("Key.Tab.IsPressed()"), Persistent]
	InputTrigger InputShowScoreboard { get; set; }

	/// <summary>
	///   When triggered in an active game session, moves the player forwards.
	/// </summary>
	[Cvar("Key.W.IsPressed() | Key.Up.IsPressed()"), Persistent]
	InputTrigger InputForward { get; set; }

	/// <summary>
	///   When triggered in an active game session, moves the player backwards.
	/// </summary>
	[Cvar("Key.S.IsPressed() | Key.Down.IsPressed()"), Persistent]
	InputTrigger InputBackward { get; set; }

	/// <summary>
	///   When triggered in an active game session, turns the player to the left.
	/// </summary>
	[Cvar("Key.Q.IsPressed()"), Persistent]
	InputTrigger InputTurnLeft { get; set; }

	/// <summary>
	///   When triggered in an active game session, turns the player to the right.
	/// </summary>
	[Cvar("Key.E.IsPressed()"), Persistent]
	InputTrigger InputTurnRight { get; set; }

	/// <summary>
	///   When triggered in an active game session, moves the player to the left.
	/// </summary>
	[Cvar("Key.A.IsPressed()"), Persistent]
	InputTrigger InputStrafeLeft { get; set; }

	/// <summary>
	///   When triggered in an active game session, moves the player to the right.
	/// </summary>
	[Cvar("Key.D.IsPressed()"), Persistent]
	InputTrigger InputStrafeRight { get; set; }

	/// <summary>
	///   When triggered in an active game session, fires the player's primary weapon.
	/// </summary>
	[Cvar("MouseButton.Left.IsPressed()"), Persistent]
	InputTrigger InputPrimaryWeapon { get; set; }

	/// <summary>
	///   When triggered in an active game session, fires the player's secondary weapon.
	/// </summary>
	[Cvar("MouseButton.Right.IsPressed()"), Persistent]
	InputTrigger InputSecondaryWeapon { get; set; }

	/// <summary>
	///   When triggered in an active game session, fires the player's tertiary weapon.
	/// </summary>
	[Cvar("Key.Num1.IsPressed()"), Persistent]
	InputTrigger InputTertiaryWeapon { get; set; }

	/// <summary>
	///   When triggered in an active game session, fires the player's quaternary weapon.
	/// </summary>
	[Cvar("Key.Num2.IsPressed()"), Persistent]
	InputTrigger InputQuaternaryWeapon { get; set; }

	/// <summary>
	///   When triggered in an active game session, opens the chat input.
	/// </summary>
	[Cvar("Key.Return.WentDown() | Key.NumpadEnter.WentDown()"), Persistent]
	InputTrigger InputChat { get; set; }
}