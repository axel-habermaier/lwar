using System;

namespace Lwar.Client.Network
{
	using Pegasus.Framework;
	using Pegasus.Framework.Network;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Logging;
	using Pegasus.Framework.Platform.Memory;

	/// <summary>
	///   Represents the header of a packet.
	/// </summary>
	public struct PacketHeader
	{
		/// <summary>
		///   The application identifier that is used to determine whether a packet has been sent by another application.
		/// </summary>
		private const uint AppIdentifier = 0xf27087c5;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="acknowledgement">The acknowledged sequence number of the packet.</param>
		/// <param name="timestamp">The timestamp of the unreliable messages contained within the packet.</param>
		public PacketHeader(uint acknowledgement, uint timestamp)
			: this()
		{
			Acknowledgement = acknowledgement;
			Timestamp = timestamp;
		}

		/// <summary>
		///   Gets the acknowledged sequence number of the packet.
		/// </summary>
		public uint Acknowledgement { get; private set; }

		/// <summary>
		///   Gets the timestamp of the unreliable messages contained within the packet.
		/// </summary>
		public uint Timestamp { get; private set; }

		/// <summary>
		///   Initializes a new instance from a buffer.
		/// </summary>
		/// <param name="buffer">The buffer the header data should be read from.</param>
		public static PacketHeader? Create(BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer);

			if (!buffer.CanRead(Specification.HeaderSize))
			{
				Log.Warn(LogCategory.Client, "Received a packet with an incomplete header.");
				return null;
			}

			if (buffer.ReadUInt32() != AppIdentifier)
			{
				Log.Warn(LogCategory.Client, "Received a packet with an invalid application identifier from the server.");
				return null;
			}

			var header = new PacketHeader();
			header.Acknowledgement = buffer.ReadUInt32();
			header.Timestamp = buffer.ReadUInt32();
			return header;
		}

		/// <summary>
		///   Writes the header into the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the header should be written into.</param>
		public void Write(BufferWriter buffer)
		{
			Assert.ArgumentNotNull(buffer);

			buffer.WriteUInt32(AppIdentifier);
			buffer.WriteUInt32(Acknowledgement);
			buffer.WriteUInt32(Timestamp);
		}
	}
}