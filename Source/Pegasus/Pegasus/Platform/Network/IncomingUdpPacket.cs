namespace Pegasus.Platform.Network
{
	using System;
	using Memory;
	using Utilities;

	/// <summary>
	///     Represents an incoming UDP data packet.
	/// </summary>
	public sealed class IncomingUdpPacket : UniquePooledObject
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static IncomingUdpPacket()
		{
			ConstructorCache.Register(() => new IncomingUdpPacket());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private IncomingUdpPacket()
		{
		}

		/// <summary>
		///     Gets the buffer storing the data of the packet.
		/// </summary>
		internal byte[] Buffer { get; private set; }

		/// <summary>
		///     Gets the size of the stored data in bytes.
		/// </summary>
		public int Size { get; internal set; }

		/// <summary>
		///     Creates a buffer reader that can be used to read from the packet.
		/// </summary>
		public BufferReader Reader
		{
			get { return new BufferReader(Buffer, 0, Size, Endianess.Big); }
		}

		/// <summary>
		///     Allocates a new UDP packet with the given capacity.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate the packet.</param>
		/// <param name="capacity">The maximum number of bytes that can be stored in the UDP packet.</param>
		internal static IncomingUdpPacket Allocate(PoolAllocator allocator, int capacity)
		{
			Assert.ArgumentNotNull(allocator);
			Assert.InRange(capacity, 1, UInt16.MaxValue);

			var packet = allocator.Allocate<IncomingUdpPacket>();

			if (packet.Buffer == null || packet.Buffer.Length < capacity)
				packet.Buffer = new byte[capacity];

			return packet;
		}
	}
}