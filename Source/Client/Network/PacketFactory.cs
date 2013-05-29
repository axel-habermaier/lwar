using System;

namespace Lwar.Client.Network
{
	using Pegasus.Framework.Network;
	using Pegasus.Framework.Platform.Memory;

	/// <summary>
	///   Provides methods to create instances of incoming and outgoing packets.
	/// </summary>
	internal class PacketFactory : IPacketFactory
	{
		/// <summary>
		///   The pool that pools the byte arrays of the service packets.
		/// </summary>
		private static readonly ArrayPool<byte> Pool = new ArrayPool<byte>(Specification.MaxPacketSize);

		/// <summary>
		///   Creates an outgoing packet.
		/// </summary>
		public OutgoingPacket CreateOutgoingPacket()
		{
			return OutgoingPacket.Create(Pool);
		}

		/// <summary>
		///   Creates an incoming packet.
		/// </summary>
		public IncomingPacket CreateIncomingPacket()
		{
			return IncomingPacket.Create(Pool);
		}
	}
}