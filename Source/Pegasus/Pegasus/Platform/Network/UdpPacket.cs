namespace Pegasus.Platform.Network
{
	using System;
	using Memory;
	using Utilities;

	/// <summary>
	///     Represents a UDP data packet.
	/// </summary>
	public sealed class UdpPacket : UniquePooledObject
	{
		/// <summary>
		///     The application-wide object pool that is used to allocate UDP packets.
		/// </summary>
		private static readonly ObjectPool<UdpPacket> Pool =
			new ObjectPool<UdpPacket>(() => new UdpPacket(), hasGlobalLifetime: true);

		/// <summary>
		///     The buffer storing the data of the packet.
		/// </summary>
		private byte[] _buffer;

		/// <summary>
		///     The size of the stored data in bytes.
		/// </summary>
		private int _size;

		/// <summary>
		///     The buffer writer that is used to write to this packet.
		/// </summary>
		private BufferWriter _writer;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private UdpPacket()
		{
		}

		/// <summary>
		///     Gets the buffer storing the data of the packet.
		/// </summary>
		internal byte[] Buffer
		{
			get
			{
				CheckWriter();
				Assert.IsNull(_writer, "The packet cannot be used as it is still being written to.");

				return _buffer;
			}
		}

		/// <summary>
		///     Gets the size of the stored data in bytes.
		/// </summary>
		public int Size
		{
			get
			{
				CheckWriter();

				if (_writer != null)
					return _writer.Count;

				return _size;
			}
			internal set { _size = value; }
		}

		/// <summary>
		///     Checks whether a buffer writer is still being used to write to the packet.
		/// </summary>
		private void CheckWriter()
		{
			if (_writer == null || (_writer.InUse && _writer.Buffer == _buffer))
				return;

			Size = _writer.Count;
			_writer = null;
		}

		/// <summary>
		///     Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override void OnReturning()
		{
			CheckWriter();
			Assert.IsNull(_writer, "The packet is still being written to.");

			_size = 0;
		}

		/// <summary>
		///     Creates a buffer reader that can be used to read from the packet.
		/// </summary>
		public BufferReader CreateReader()
		{
			return BufferReader.Create(Buffer, 0, Size, Endianess.Big);
		}

		/// <summary>
		///     Creates a buffer writer that can be used to write to the packet.
		/// </summary>
		public BufferWriter CreateWriter()
		{
			CheckWriter();
			Assert.IsNull(_writer, "Another writer is currently writing to this packet.");

			_writer = BufferWriter.Create(_buffer, Endianess.Big);
			return _writer;
		}

		/// <summary>
		///     Allocates a new UDP packet with the given capacity.
		/// </summary>
		/// <param name="capacity">The maximum number of bytes that can be stored in the UDP packet.</param>
		public static UdpPacket Allocate(int capacity)
		{
			Assert.InRange(capacity, 1, UInt16.MaxValue);

			var packet = Pool.Allocate();

			if (packet._buffer == null || packet._buffer.Length < capacity)
				packet._buffer = new byte[capacity];

			return packet;
		}
	}
}