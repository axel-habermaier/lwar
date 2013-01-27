using System;

namespace Pegasus.Framework.Network
{
	/// <summary>
	///   Represents the header of a message sent over the network.
	/// </summary>
	internal struct MessageHeader
	{
		/// <summary>
		///   The length of the message header in bytes.
		/// </summary>
		public const int Length = 17;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="serviceIdentifier">The identifier of the service.</param>
		/// <param name="messageType">The type of the message.</param>
		public MessageHeader(ServiceIdentifier serviceIdentifier, MessageType messageType)
			: this()
		{
			ServiceIdentifier = serviceIdentifier;
			MessageType = messageType;
		}

		/// <summary>
		///   Gets or sets the identifier of the service.
		/// </summary>
		public ServiceIdentifier ServiceIdentifier { get; private set; }

		/// <summary>
		///   Gets or sets the type of the message.
		/// </summary>
		public MessageType MessageType { get; private set; }

		/// <summary>
		///   Writes the message header into the outgoing packet.
		/// </summary>
		/// <param name="packet">The outgoing packet the header should be written into.</param>
		public void Write(OutgoingPacket packet)
		{
			Assert.ArgumentNotNull(packet, () => packet);

			packet.Write(ServiceIdentifier.FirstPart);
			packet.Write(ServiceIdentifier.SecondPart);
			packet.Write((byte)MessageType);
		}

		/// <summary>
		///   Reads the message header from the incoming packet.
		/// </summary>
		/// <param name="packet">The incoming packet the header should be read from.</param>
		public static MessageHeader Read(IncomingPacket packet)
		{
			Assert.ArgumentNotNull(packet, () => packet);

			return new MessageHeader
			{
				ServiceIdentifier = new ServiceIdentifier(packet.ReadUInt64(), packet.ReadUInt64()),
				MessageType = (MessageType)packet.ReadByte(),
			};
		}
	}
}