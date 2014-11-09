namespace Lwar.Network.Messages
{
	using System;
	using System.Text;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Informs a server and its clients about a player name change.
	/// </summary>
	[ReliableTransmission(MessageType.PlayerName)]
	public sealed class PlayerNameMessage : Message
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static PlayerNameMessage()
		{
			ConstructorCache.Register(() => new PlayerNameMessage());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private PlayerNameMessage()
		{
		}

		/// <summary>
		///     Gets the new name of the player.
		/// </summary>
		public string PlayerName { get; private set; }

		/// <summary>
		///     Gets the player whose name is changed.
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
			handler.OnPlayerName(this);
		}

		/// <summary>
		///     Creates a message that instructs the server to change the name of the given player.
		/// </summary>
		/// <param name="poolAllocator">The pool allocator that should be used to allocate the message.</param>
		/// <param name="player">The player whose name should be changed.</param>
		/// <param name="playerName">The new player name.</param>
		public static Message Create(PoolAllocator poolAllocator, NetworkIdentity player, string playerName)
		{
			Assert.ArgumentNotNull(poolAllocator);
			Assert.ArgumentNotNullOrWhitespace(playerName);
			Assert.That(Encoding.UTF8.GetByteCount(playerName) <= NetworkProtocol.PlayerNameLength, "Player name is too long.");

			var message = poolAllocator.Allocate<PlayerNameMessage>();
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