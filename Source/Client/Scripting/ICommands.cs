using System;
using System.Net;
using Lwar.Client.Network;
using Pegasus.Framework.Scripting;

// ReSharper disable CheckNamespace

/// <summary>
///   Declares the lwar-specific commands.
/// </summary>
internal interface ICommands
{
	/// <summary>
	///   Starts up a new local server instance. If a local server is currently running, it is shut down before the new server
	///   is started.
	/// </summary>
	[Command]
	void StartServer();

	/// <summary>
	///   Shuts down the currently running server.
	/// </summary>
	[Command]
	void StopServer();

	/// <summary>
	///   Connects to a game session on a remote or local server.
	/// </summary>
	/// <param name="ipAddress">
	///   The IP address of the server in either IPv4 or IPv6 format. For instance, either 127.0.0.1 or ::1 can be used to
	///   connect to a local server.
	/// </param>
	/// <param name="port">The port of the server.</param>
	[Command]
	void Connect(IPAddress ipAddress, ushort port = Specification.DefaultServerPort);

	/// <summary>
	///   Disconnects from the current game session.
	/// </summary>
	[Command]
	void Disconnect();

	/// <summary>
	///   Sends a chat message to all peers.
	/// </summary>
	/// <param name="message">The message that should be sent.</param>
	[Command]
	void Say([NotEmpty, MaximumLength(Specification.MaximumChatMessageLength, true)] string message);

	/// <summary>
	///   Toggles between the game and the debugging camera.
	/// </summary>
	[Command]
	void ToggleDebugCamera();

	/// <summary>
	///   Shows or hides the scoreboard during an active game session.
	/// </summary>
	/// <param name="show">A value of 'true' indicates that the scoreboard should be shown.</param>
	[Command]
	void ShowScoreboard(bool show);
}