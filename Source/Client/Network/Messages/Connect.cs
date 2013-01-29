using System;

namespace Lwar.Client.Network.Messages
{
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;

	public class Connect : PooledObject<Connect>, IReliableMessage
	{
		/// <summary>
		///   The size of the message in bytes.
		/// </summary>
		private const int Size = sizeof(byte) + sizeof(uint);

		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="session">The game session that should be updated.</param>
		public void Process(GameSession session)
		{
			Assert.That(false, "The client cannot process this type of message.");
		}

		/// <summary>
		///   Serializes the message into the given buffer, returning false if the message did not fit.
		/// </summary>
		/// <param name="buffer">The buffer the message should be written to.</param>
		public bool Serialize(BufferWriter buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);

			if (!buffer.CanWrite(Size))
				return false;

			buffer.WriteByte((byte)MessageType.Connect);
			buffer.WriteUInt32(SequenceNumber);
			return true;
		}

		/// <summary>
		///   Gets or sets the sequence number of the message.
		/// </summary>
		public uint SequenceNumber { get; set; }

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		public static Connect Create()
		{
			return GetInstance();
		}
	}
}