namespace Lwar.Network.Messages
{
	using System;
	using System.Text;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Informs a client that a new player has joined the game.
	/// </summary>
	[ReliableTransmission(MessageType.PlayerJoin)]
	public sealed class PlayerJoinMessage : Message
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static PlayerJoinMessage()
		{
			ConstructorCache.Register(() => new PlayerJoinMessage());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private PlayerJoinMessage()
		{
		}

		/// <summary>
		///     Gets the name of the player that joined.
		/// </summary>
		public string PlayerName { get; private set; }

		/// <summary>
		///     Gets the identifier of the player that joined the game session.
		/// </summary>
		public NetworkIdentity Player { get; private set; }

		/// <summary>
		///     Serializes the message using the given writer.
		/// </summary>
		/// <param name="writer">The writer that should be used to serialize the message.</param>
		public override void Serialize(BufferWriter writer)
		{
			writer.WriteIdentifier(Player);
			writer.WriteString(PlayerName, NetworkProtocol.PlayerNameLength);
		}

		/// <summary>
		///     Deserializes the message using the given reader.
		/// </summary>
		/// <param name="reader">The reader that should be used to deserialize the message.</param>
		public override void Deserialize(BufferReader reader)
		{
			Player = reader.ReadIdentifier();
			PlayerName = reader.ReadString(NetworkProtocol.PlayerNameLength);
		}

		/// <summary>
		///     Dispatches the message to the given dispatcher.
		/// </summary>
		/// <param name="handler">The dispatcher that should be used to dispatch the message.</param>
		/// <param name="sequenceNumber">The sequence number of the message.</param>
		public override void Dispatch(IMessageHandler handler, uint sequenceNumber)
		{
			handler.OnPlayerJoin(this);
		}

		/// <summary>
		///     Creates a join message for the given player.
		/// </summary>
		/// <param name="poolAllocator">The pool allocator that should be used to allocate the message.</param>
		/// <param name="player">The player that has joined the game session.</param>
		/// <param name="playerName">The name of the player.</param>
		public static Message Create(PoolAllocator poolAllocator, NetworkIdentity player, string playerName)
		{
			Assert.ArgumentNotNull(poolAllocator);
			Assert.ArgumentNotNullOrWhitespace(playerName);
			Assert.That(Encoding.UTF8.GetByteCount(playerName) <= NetworkProtocol.PlayerNameLength, "Player name is too long.");

			var message = poolAllocator.Allocate<PlayerJoinMessage>();
			message.Player = player;
			message.PlayerName = playerName;
			return message;
		}

		/// <summary>
		///     Returns a string that represents the message.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0}, Player={1}, PlayerName='{2}'", MessageType, Player, PlayerName);
		}
	}
}