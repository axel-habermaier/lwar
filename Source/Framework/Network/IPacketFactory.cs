using System;

namespace Pegasus.Framework.Network
{
	/// <summary>
	///   Provides methods to create instances of incoming and outgoing packets.
	/// </summary>
	public interface IPacketFactory
	{
		/// <summary>
		///   Creates an outgoing packet.
		/// </summary>
		OutgoingPacket CreateOutgoingPacket();

		/// <summary>
		///   Creates an incoming packet.
		/// </summary>
		IncomingPacket CreateIncomingPacket();
	}
}