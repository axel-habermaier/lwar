﻿using System;

namespace Lwar.Client.Network
{
	using Pegasus.Framework;
	using Pegasus.Framework.Network;

	/// <summary>
	///   Provides methods to create instances of incoming and outgoing packets.
	/// </summary>
	internal class LwarPacketFactory : IPacketFactory
	{
		/// <summary>
		///   The maximum packet size in bytes.
		/// </summary>
		private const int MaxPacketSize = 768;

		/// <summary>
		///   The pool that pools the byte arrays of the service packets.
		/// </summary>
		private static readonly ArrayPool<byte> Pool = new ArrayPool<byte>(MaxPacketSize);

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