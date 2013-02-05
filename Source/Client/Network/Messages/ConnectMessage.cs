using System;

namespace Lwar.Client.Network.Messages
{
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;

	public class ConnectMessage : Message<ConnectMessage>, IReliableMessage
	{
		/// <summary>
		///   Writes the message into the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the message should be written to.</param>
		public override bool Write(BufferWriter buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			return buffer.TryWrite(this, (b, m) =>
				{
					b.WriteByte((byte)MessageType.Connect);
					b.WriteUInt32(m.SequenceNumber);
				});
		}

		/// <summary>
		///   Gets or sets the sequence number of the message.
		/// </summary>
		public uint SequenceNumber { get; set; }

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		public static ConnectMessage Create()
		{
			return GetInstance();
		}
	}
}