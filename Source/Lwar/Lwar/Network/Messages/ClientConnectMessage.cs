namespace Lwar.Network.Messages
{
	using System;
	using System.Text;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Informs a server about a connection attempt from a potential client.
	/// </summary>
	[ReliableTransmission(MessageType.ClientConnect)]
	internal sealed class ClientConnectMessage : Message
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static ClientConnectMessage()
		{
			ConstructorCache.Register(() => new ClientConnectMessage());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private ClientConnectMessage()
		{
		}

		/// <summary>
		///     Gets the name of the player that is connecting.
		/// </summary>
		public string PlayerName { get; private set; }

		/// <summary>
		///     Gets the revision number of the network protocol that the connecting client implements.
		/// </summary>
		public byte NetworkRevision { get; private set; }

		/// <summary>
		///     Serializes the message using the given writer.
		/// </summary>
		/// <param name="writer">The writer that should be used to serialize the message.</param>
		public override void Serialize(ref BufferWriter writer)
		{
			writer.WriteByte(NetworkRevision);
			writer.WriteString(PlayerName, NetworkProtocol.PlayerNameLength);
		}

		/// <summary>
		///     Deserializes the message using the given reader.
		/// </summary>
		/// <param name="reader">The reader that should be used to deserialize the message.</param>
		public override void Deserialize(ref BufferReader reader)
		{
			NetworkRevision = reader.ReadByte();
			PlayerName = reader.ReadString(NetworkProtocol.PlayerNameLength);
		}

		/// <summary>
		///     Dispatches the message to the given dispatcher.
		/// </summary>
		/// <param name="handler">The dispatcher that should be used to dispatch the message.</param>
		/// <param name="sequenceNumber">The sequence number of the message.</param>
		public override void Dispatch(IMessageHandler handler, uint sequenceNumber)
		{
			handler.OnConnect(this);
		}

		/// <summary>
		///     Creates a connect message.
		/// </summary>
		/// <param name="poolAllocator">The pool allocator that should be used to allocate the message.</param>
		/// <param name="playerName">The name of the player that is connecting.</param>
		public static ClientConnectMessage Create(PoolAllocator poolAllocator, string playerName)
		{
			Assert.ArgumentNotNull(poolAllocator);
			Assert.ArgumentNotNullOrWhitespace(playerName);
			Assert.That(Encoding.UTF8.GetByteCount(playerName) <= NetworkProtocol.PlayerNameLength, "Player name is too long.");

			var message = poolAllocator.Allocate<ClientConnectMessage>();
			message.PlayerName = playerName;
			message.NetworkRevision = NetworkProtocol.Revision;
			return message;
		}

		/// <summary>
		///     Returns a string that represents the message.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0}, NetworkRevision={1}, PlayerName='{2}'", MessageType, NetworkRevision, PlayerName);
		}
	}
}