using System;

namespace Pegasus.Framework.Network
{
	using Platform;

	/// <summary>
	///   Provides configuration constants for packets.
	/// </summary>
	public static class Packet
	{
		/// <summary>
		///   The maximum length of a string that can be written into a packet.
		/// </summary>
		public const int MaxStringLength = 128;

		/// <summary>
		///   The maximum size of a packet. The value is chosen such that packet fragmentation is very unlikely.
		/// </summary>
		public const int MaxSize = 512;

		/// <summary>
		///   The number of bytes of the packet size that is appended to each packet.
		/// </summary>
		public const int SizeByteCount = sizeof(ushort);
	}

	/// <summary>
	///   Represents a packet that is sent across the network between peers.
	/// </summary>
	/// <typeparam name="TPacket">The concrete packet type.</typeparam>
	public abstract class Packet<TPacket> : PooledObject<TPacket>
		where TPacket : Packet<TPacket>, new()
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		protected Packet()
		{
			Buffer = new byte[Packet.MaxSize];
		}

		/// <summary>
		///   Gets the data buffer of the package.
		/// </summary>
		protected byte[] Buffer { get; private set; }
	}
}