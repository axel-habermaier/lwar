using System;

namespace Pegasus.Framework.Network
{
	using Platform;

	/// <summary>
	///   Represents an outgoing packet.
	/// </summary>
	public class OutgoingPacket : PooledObject<OutgoingPacket>
	{
		/// <summary>
		///   The data stored in the packet.
		/// </summary>
		private readonly byte[] _data = new byte[Packet.MaxSize];

		/// <summary>
		///   Gets the writer that can be used to write the data to the packet.
		/// </summary>
		public BufferWriter Writer { get; private set; }

		/// <summary>
		///   Gets the size of the packet data in bytes.
		/// </summary>
		internal int Size
		{
			get { return Writer.Length; }
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
		public static OutgoingPacket Create()
		{
			var packet = GetInstance();
			packet.Writer = BufferWriter.Create(packet._data, Endianess.Big);
			return packet;
		}

		/// <summary>
		///   Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override void OnReturning()
		{
			Writer.SafeDispose();
		}
	}
}