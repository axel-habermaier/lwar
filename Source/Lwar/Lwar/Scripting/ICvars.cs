﻿using Lwar.Network;
using Pegasus.Scripting;
using Pegasus.Scripting.Validators;
using Pegasus.UserInterface.Input;

// ReSharper disable CheckNamespace

/// <summary>
///     Declares the lwar-specific cvars.
/// </summary>
internal interface ICvars
{
	/// <summary>
	///     The name of the player.
	/// </summary>
	[Cvar("UnnamedPlayer"), Persistent, NotEmpty, MaximumLength(NetworkProtocol.PlayerNameLength, true)]
	string PlayerName { get; set; }

	/// <summary>
	///     The display time (in seconds) of event messages such as 'X killed Y', 'X joined the game', etc.
	/// </summary>
	[Cvar(3), Persistent, Range(0.5, 60.0)]
	double EventMessageDisplayTime { get; set; }

	/// <summary>
	///     The display time (in seconds) of chat messages.
	/// </summary>
	[Cvar(6), Persistent, Range(0.5, 60.0)]
	double ChatMessageDisplayTime { get; set; }

	/// <summary>
	///     When triggered in an active game session, shows the scoreboard.
	/// </summary>
	[Cvar("Key.Tab"), Persistent]
	ConfigurableInput InputShowScoreboard { get; set; }

	/// <summary>
	///     When triggered in an active game session, respawns the player after death.
	/// </summary>
	[Cvar("MouseButton.Left"), Persistent]
	ConfigurableInput InputRespawn { get; set; }

	/// <summary>
	///     When triggered in an active game session, moves the player forwards.
	/// </summary>
	[Cvar("Key.W"), Persistent]
	ConfigurableInput InputForward { get; set; }

	/// <summary>
	///     When triggered in an active game session, moves the player backwards.
	/// </summary>
	[Cvar("Key.S"), Persistent]
	ConfigurableInput InputBackward { get; set; }

	/// <summary>
	///     When triggered in an active game session, moves the player to the left.
	/// </summary>
	[Cvar("Key.A"), Persistent]
	ConfigurableInput InputStrafeLeft { get; set; }

	/// <summary>
	///     When triggered in an active game session, moves the player to the right.
	/// </summary>
	[Cvar("Key.D"), Persistent]
	ConfigurableInput InputStrafeRight { get; set; }

	/// <summary>
	///     When triggered in an active game session, triggers the player's after burner.
	/// </summary>
	[Cvar("Key.LeftShift"), Persistent]
	ConfigurableInput InputAfterBurner { get; set; }

	/// <summary>
	///     When triggered in an active game session, fires the player's primary weapon.
	/// </summary>
	[Cvar("MouseButton.Left"), Persistent]
	ConfigurableInput InputPrimaryWeapon { get; set; }

	/// <summary>
	///     When triggered in an active game session, fires the player's secondary weapon.
	/// </summary>
	[Cvar("MouseButton.Right"), Persistent]
	ConfigurableInput InputSecondaryWeapon { get; set; }

	/// <summary>
	///     When triggered in an active game session, fires the player's tertiary weapon.
	/// </summary>
	[Cvar("Key.Num1"), Persistent]
	ConfigurableInput InputTertiaryWeapon { get; set; }

	/// <summary>
	///     When triggered in an active game session, fires the player's quaternary weapon.
	/// </summary>
	[Cvar("Key.Num2"), Persistent]
	ConfigurableInput InputQuaternaryWeapon { get; set; }

	/// <summary>
	///     When triggered in an active game session, opens the chat input.
	/// </summary>
	[Cvar("Key.Return"), Persistent]
	ConfigurableInput InputChat { get; set; }

	/// <summary>
	///     When true, the native server is used instead of the C# server.
	/// </summary>
	[Cvar(false), Persistent]
	bool UseNativeServer { get; set; }
}