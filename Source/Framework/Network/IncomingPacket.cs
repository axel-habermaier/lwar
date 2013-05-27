using System;

namespace Pegasus.Framework.Network
{
	using Platform;
	using Platform.Memory;

	/// <summary>
	///   Represents an incoming packet.
	/// </summary>
	public class IncomingPacket : PooledObject<IncomingPacket>
	{
		/// <summary>
		///   The data stored in the packet.
		/// </summary>
		private byte[] _data;

		/// <summary>
		///   The array pool that manages the data array instance.
		/// </summary>
		private ArrayPool<byte> _pool;

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
		/// <param name="pool">The array pool that should be used to create the packet's data array instance.</param>
		public static IncomingPacket Create(ArrayPool<byte> pool)
		{
			var packet = GetInstance();
			packet._pool = pool;
			packet._data = pool.Get();
			return packet;
		}

		/// <summary>
		///   Sets the valid data range of the packet's data buffer.
		/// </summary>
		/// <param name="size">The size of the packet's data in bytes.</param>
		internal void SetDataRange(int size)
		{
			Assert.InRange(size, 0, _data.Length);
			Reader = BufferReader.Create(_data, 0, size, Endianess.Big);
			Size = size;
		}

		/// <summary>
		///   Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override void OnReturning()
		{
			_pool.Return(_data);
			Reader.SafeDispose();
		}
	}
}