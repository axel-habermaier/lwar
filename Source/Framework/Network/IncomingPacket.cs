using System;

namespace Pegasus.Framework.Network
{
	using Platform;

	/// <summary>
	///   Represents an incoming packet.
	/// </summary>
	public class IncomingPacket : PooledObject<IncomingPacket>
	{
		/// <summary>
		///   The data stored in the packet.
		/// </summary>
		private readonly byte[] _data = new byte[Packet.MaxSize];

		/// <summary>
		///   Gets the data stored in the packet.
		/// </summary>
		internal byte[] Data
		{
			get { return _data; }
		}

		/// <summary>
		///   Gets the size of the packet data in bytes.
		/// </summary>
		internal int Size { get; private set; }

		/// <summary>
		///   Gets the reader that can be used to read the data stored in the packet.
		/// </summary>
		public BufferReader Reader { get; private set; }

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		internal static IncomingPacket Create()
		{
			return GetInstance();
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="size">The size in bytes of the packet's data.</param>
		internal static IncomingPacket Create(int size)
		{
			var packet = GetInstance();
			packet.Size = size;
			packet.Reader = BufferReader.Create(packet._data, 0, size, Endianess.Big);
			return packet;
		}

		/// <summary>
		///   Initializes the packet if its size is not known at creation time.
		/// </summary>
		/// <param name="size">The size of the packet's data in bytes.</param>
		internal void Initialize(int size)
		{
			Assert.InRange(size, 1, Packet.MaxSize);
			Reader = BufferReader.Create(_data, 0, size, Endianess.Big);
		}

		/// <summary>
		///   Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override void OnReturning()
		{
			Reader.SafeDispose();
		}
	}
}