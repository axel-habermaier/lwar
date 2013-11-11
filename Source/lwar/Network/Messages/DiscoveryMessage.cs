namespace Lwar.Network.Messages
{
	using System;
	using Pegasus;
	using Pegasus.Platform.Memory;

	/// <summary>
	///   Represents a discovery message that is sent periodically by servers running the game.
	/// </summary>
	public struct DiscoveryMessage
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="reader">The reader that should be used to read the contents of the discovery message.</param>
		public DiscoveryMessage(BufferReader reader)
			: this()
		{
			Assert.ArgumentNotNull(reader);

			if (!reader.CanRead(sizeof(byte) + sizeof(uint) + sizeof(byte) + sizeof(ushort)))
				return;

			var type = (MessageType)reader.ReadByte();
			var applicationIdentifier = reader.ReadUInt32();
			var revision = reader.ReadByte();

			Port = reader.ReadUInt16();
			IsValid = type == MessageType.Discovery && applicationIdentifier == Specification.AppIdentifier && revision == Specification.Revision;
		}

		/// <summary>
		///   The port that the client should use to connect to the server.
		/// </summary>
		public ushort Port { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the discovery message is valid or should be ignored.
		/// </summary>
		public bool IsValid { get; private set; }
	}
}