namespace Lwar.Network
{
	using System;
	using Pegasus;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;

	/// <summary>
	///     Represents the header of a packet.
	/// </summary>
	public struct PacketHeader
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="acknowledgement">The acknowledged sequence number of the packet.</param>
		public PacketHeader(uint acknowledgement)
			: this()
		{
			Acknowledgement = acknowledgement;
		}

		/// <summary>
		///     Gets the acknowledged sequence number of the packet.
		/// </summary>
		public uint Acknowledgement { get; private set; }

		/// <summary>
		///     Initializes a new instance from a buffer.
		/// </summary>
		/// <param name="buffer">The buffer the header data should be read from.</param>
		public static PacketHeader? Create(BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer);

			if (!buffer.CanRead(Specification.HeaderSize))
			{
				Log.Warn("Received a packet with an incomplete header.");
				return null;
			}

			if (buffer.ReadUInt32() != Specification.AppIdentifier)
			{
				Log.Warn("Received a packet with an invalid application identifier from the server.");
				return null;
			}

			return new PacketHeader { Acknowledgement = buffer.ReadUInt32() };
		}

		/// <summary>
		///     Writes the header into the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the header should be written into.</param>
		public void Write(BufferWriter buffer)
		{
			Assert.ArgumentNotNull(buffer);

			buffer.WriteUInt32(Specification.AppIdentifier);
			buffer.WriteUInt32(Acknowledgement);
		}
	}
}