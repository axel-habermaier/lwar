using Lwar.Network;
using Pegasus.Platform.Network;
using Pegasus.Scripting;
using Pegasus.Scripting.Validators;

// ReSharper disable CheckNamespace

/// <summary>
///     Declares the Lwar-specific commands.
/// </summary>
internal interface ICommands
{
	/// <summary>
	///     Starts up a new local server instance. If a local server is currently running, it is shut down before the new server
	///     is started.
	/// </summary>
	/// <param name="serverName">The name of the server that is displayed in the Join screen.</param>
	/// <param name="port">The port the server should use to communicate with the clients.</param>
	[Command]
	void StartServer([NotEmpty] string serverName, ushort port = NetworkProtocol.DefaultServerPort);

	/// <summary>
	///     Shuts down the currently running server.
	/// </summary>
	[Command]
	void StopServer();

	/// <summary>
	///     Connects to a game session on a remote or local server.
	/// </summary>
	/// <param name="ipAddress">The IP address of the server.</param>
	/// <param name="port">The port of the server.</param>
	[Command]
	void Connect(IPAddress ipAddress, ushort port = NetworkProtocol.DefaultServerPort);

	/// <summary>
	///     Disconnects from the current game session.
	/// </summary>
	[Command]
	void Disconnect();

	/// <summary>
	///     Sends a chat message to all peers.
	/// </summary>
	/// <param name="message">The message that should be sent.</param>
	[Command]
	void Say([NotEmpty, MaximumLength(NetworkProtocol.ChatMessageLength, true)] string message);
}