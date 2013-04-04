using System;

namespace Lwar.Client.Scripting
{
	using System.Net;
	using Pegasus.Framework.Scripting;

	/// <summary>
	///   Declares the lwar-specific commands.
	/// </summary>
	public interface ICommandRegistry
	{
		/// <summary>
		///   Starts up a new server instance.
		/// </summary>
		[Command]
		void StartServer();

		/// <summary>
		///   Shuts down the currently running server.
		/// </summary>
		[Command]
		void StopServer();

		/// <summary>
		///   Connects to a game session on a server.
		/// </summary>
		[Command]
		void Connect(IPEndPoint endPoint);

		/// <summary>
		///   Disconnects from the current game session.
		/// </summary>
		[Command]
		void Disconnect();

		/// <summary>
		///   Sends a chat message to all peers.
		/// </summary>
		[Command]
		void Chat(string message);

		/// <summary>
		///   Toggles between the game and the debugging camera.
		/// </summary>
		[Command]
		void ToggleDebugCamera();
	}
}