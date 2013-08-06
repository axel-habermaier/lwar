using System;

namespace Lwar.Client.Network
{
	/// <summary>
	///   Indicates why a connect attempt has been rejected by the server.
	/// </summary>
	public enum RejectReason
	{
		/// <summary>
		///   Indicates that the server was full.
		/// </summary>
		Full = 1,

		/// <summary>
		///   Indicates that the server uses another version of the network protocol than the client.
		/// </summary>
		VersionMismatch = 2,
	}
}