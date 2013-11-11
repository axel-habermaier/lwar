namespace Lwar.Network
{
	using System;

	/// <summary>
	///   Describes the state of the virtual connection to the server.
	/// </summary>
	public enum ConnectionState
	{
		/// <summary>
		///   Indicates that a connection attempt has been started.
		/// </summary>
		Connecting,

		/// <summary>
		///   Indicates that a connection has been established and that the proxy is waiting for a Synced message.
		/// </summary>
		Syncing,

		/// <summary>
		///   Indicates that a connection is established and the game state is fully synced.
		/// </summary>
		Connected,

		/// <summary>
		///   Indicates that a connection is faulted due to an error and can no longer be used to send and receive any data.
		/// </summary>
		Faulted,

		/// <summary>
		///   Indicates that the server is full and cannot accept any further clients.
		/// </summary>
		Full,

		/// <summary>
		///   Indicates that a connection has been dropped after no packets have been received from the server for a specific
		///   amount of time.
		/// </summary>
		Dropped,

		/// <summary>
		///   Indicates that the server implements another revision of the network protocol.
		/// </summary>
		VersionMismatch
	}
}