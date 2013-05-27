using System;

namespace Pegasus.Framework.Network
{
	using Platform;
	using Platform.Memory;

	/// <summary>
	///   Represents an outgoing packet.
	/// </summary>
	public class OutgoingPacket : PooledObject<OutgoingPacket>
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
		///   Gets the writer that can be used to write the data to the packet.
		/// </summary>
		public BufferWriter Writer { get; private set; }

		/// <summary>
		///   Gets the size of the packet data in bytes.
		/// </summary>
		internal int Size
		{
			get { return Writer.Count; }
		}

		/// <summary>
		///   Gets the data stored in the packet.
		/// </summary>
		internal byte[] Data
		{
			get { return _data; }
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="pool">The array pool that should be used to create the packet's data array instance.</param>
		public static OutgoingPacket Create(ArrayPool<byte> pool)
		{
			var packet = GetInstance();
			packet._pool = pool;
			packet._data = pool.Get();
			packet.Writer = BufferWriter.Create(packet._data, Endianess.Big);
			return packet;
		}

		/// <summary>
		///   Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override void OnReturning()
		{
			_pool.Return(_data);
			Writer.SafeDispose();
		}
	}
}