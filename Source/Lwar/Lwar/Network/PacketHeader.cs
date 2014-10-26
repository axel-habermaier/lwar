namespace Lwar.Network
{
	using System;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents the header of a packet.
	/// </summary>
	public static class PacketHeader
	{
		/// <summary>
		///     Initializes a new instance from a buffer.
		/// </summary>
		/// <param name="buffer">The buffer the header data should be read from.</param>
		/// <param name="acknowledgement">Returns the acknowledged sequence number of the packet.</param>
		public static bool TryRead(BufferReader buffer, out uint acknowledgement)
		{
			Assert.ArgumentNotNull(buffer);

			acknowledgement = 0;

			if (!buffer.CanRead(NetworkProtocol.HeaderSize))
			{
				Log.Warn("Received a packet with an incomplete header.");
				return false;
			}

			if (buffer.ReadUInt32() != NetworkProtocol.AppIdentifier)
			{
				Log.Warn("Received a packet with an invalid application identifier from the server.");
				return false;
			}

			acknowledgement = buffer.ReadUInt32();
			return true;
		}

		/// <summary>
		///     Writes the header into the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the header should be written into.</param>
		/// <param name="acknowledgement">The acknowledged sequence number of the packet.</param>
		public static void Write(BufferWriter buffer, uint acknowledgement)
		{
			Assert.ArgumentNotNull(buffer);

			buffer.WriteUInt32(NetworkProtocol.AppIdentifier);
			buffer.WriteUInt32(acknowledgement);
		}
	}
}