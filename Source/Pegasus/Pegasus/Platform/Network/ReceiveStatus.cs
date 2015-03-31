namespace Pegasus.Platform.Network
{
	using System;

	/// <summary>
	///     Indicates whether a packet has been received.
	/// </summary>
	public enum ReceiveStatus
	{
		/// <summary>
		///     Indicates that an error occurred while trying to receive a packet.
		/// </summary>
		Error,

		/// <summary>
		///     Indicates that a packet has been received.
		/// </summary>
		PacketReceived,

		/// <summary>
		///     Indicates that no packet was available.
		/// </summary>
		NoPacketAvailable
	}
}