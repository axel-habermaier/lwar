namespace Lwar.Network.Messages
{
	using System;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Informs a client about the player stats of itself and other clients.
	/// </summary>
	[UnreliableTransmission(MessageType.PlayerStats, EnableBatching = true)]
	public sealed class PlayerStatsMessage : Message
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static PlayerStatsMessage()
		{
			ConstructorCache.Register(() => new PlayerStatsMessage());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private PlayerStatsMessage()
		{
		}

		/// <summary>
		///     Gets the number of deaths of the player.
		/// </summary>
		public ushort Deaths { get; private set; }

		/// <summary>
		///     Gets the number of kills scored by the player.
		/// </summary>
		public ushort Kills { get; private set; }

		/// <summary>
		///     Gets the player's network latency.
		/// </summary>
		public ushort Ping { get; private set; }

		/// <summary>
		///     Gets the player whose stats are updated.
		/// </summary>
		public NetworkIdentity Player { get; private set; }

		/// <summary>
		///     Serializes the message using the given writer.
		/// </summary>
		/// <param name="writer">The writer that should be used to serialize the message.</param>
		public override void Serialize(BufferWriter writer)
		{
			writer.WriteIdentifier(Player);
			writer.WriteUInt16(Kills);
			writer.WriteUInt16(Deaths);
			writer.WriteUInt16(Ping);
		}

		/// <summary>
		///     Deserializes the message using the given reader.
		/// </summary>
		/// <param name="reader">The reader that should be used to deserialize the message.</param>
		public override void Deserialize(BufferReader reader)
		{
			Player = reader.ReadIdentifier();
			Kills = reader.ReadUInt16();
			Deaths = reader.ReadUInt16();
			Ping = reader.ReadUInt16();
		}

		/// <summary>
		///     Dispatches the message to the given dispatcher.
		/// </summary>
		/// <param name="handler">The dispatcher that should be used to dispatch the message.</param>
		/// <param name="sequenceNumber">The sequence number of the message.</param>
		public override void Dispatch(IMessageHandler handler, uint sequenceNumber)
		{
			handler.OnPlayerStats(this, sequenceNumber);
		}

		/// <summary>
		///     Creates a stats message that the server broadcasts to all clients.
		/// </summary>
		/// <param name="poolAllocator">The pool allocator that should be used to allocate the message.</param>
		/// <param name="player">The player that has joined the game session.</param>
		/// <param name="kills">The kills scored by the player.</param>
		/// <param name="deaths">The number of times the player died.</param>
		/// <param name="ping">The latency between the server and the client in milliseconds.</param>
		public static Message Create(PoolAllocator poolAllocator, NetworkIdentity player, int kills, int deaths, int ping)
		{
			Assert.ArgumentNotNull(poolAllocator);
			Assert.ArgumentInRange(kills, 0, UInt16.MaxValue);
			Assert.ArgumentInRange(deaths, 0, UInt16.MaxValue);
			Assert.ArgumentInRange(ping, 0, UInt16.MaxValue);

			var message = poolAllocator.Allocate<PlayerStatsMessage>();
			message.Player = player;
			message.Kills = (ushort)kills;
			message.Deaths = (ushort)deaths;
			message.Ping = (ushort)ping;
			return message;
		}

		/// <summary>
		///     Returns a string that represents the message.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0}, Player={1},Kills={2},Deaths={3},Ping={4}", MessageType, Player, Kills, Deaths, Ping);
		}
	}
}