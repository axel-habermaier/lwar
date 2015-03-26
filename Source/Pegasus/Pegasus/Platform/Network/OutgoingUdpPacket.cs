namespace Pegasus.Platform.Network
{
	using System;
	using Memory;
	using Utilities;

	/// <summary>
	///     Represents an outgoing UDP data packet.
	/// </summary>
	public sealed class OutgoingUdpPacket : PooledObject
	{
		/// <summary>
		///     Gets the buffer storing the data of the packet.
		/// </summary>
		internal byte[] Buffer { get; private set; }

		/// <summary>
		///     Creates a buffer writer that can be used to write to the packet.
		/// </summary>
		public BufferWriter Writer
		{
			get { return new BufferWriter(Buffer, Endianess.Big); }
		}

		/// <summary>
		///     Allocates a new UDP packet with the given capacity.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate the packet.</param>
		/// <param name="capacity">The maximum number of bytes that can be stored in the UDP packet.</param>
		public static OutgoingUdpPacket Allocate(PoolAllocator allocator, int capacity)
		{
			Assert.ArgumentNotNull(allocator);
			Assert.InRange(capacity, 1, UInt16.MaxValue);

			var packet = allocator.Allocate<OutgoingUdpPacket>();

			if (packet.Buffer == null || packet.Buffer.Length < capacity)
				packet.Buffer = new byte[capacity];

			return packet;
		}
	}
}