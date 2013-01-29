using System;

namespace Lwar.Client.Network.Messages
{
	using System.Collections.Generic;
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;

	public class UpdatePlayerStats : PooledObject<UpdatePlayerStats>, IReliableMessage
	{
		/// <summary>
		///   The new statistics of the players.
		/// </summary>
		private readonly List<Stats> _stats = new List<Stats>();

		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="session">The game session that should be updated.</param>
		public void Process(GameSession session)
		{
		}

		/// <summary>
		///   Serializes the message into the given buffer, returning false if the message did not fit.
		/// </summary>
		/// <param name="buffer">The buffer the message should be written to.</param>
		public bool Serialize(BufferWriter buffer)
		{
			Assert.That(false, "The client cannot send this type of message.");
			return true;
		}

		/// <summary>
		///   Gets or sets the sequence number of the message.
		/// </summary>
		public uint SequenceNumber { get; set; }

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="buffer">The buffer from which the instance should be deserialized.</param>
		public static UpdatePlayerStats Create(BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);

			if (!buffer.CanRead(sizeof(uint) + sizeof(byte)))
				return null;

			var message = GetInstance();
			message.SequenceNumber = buffer.ReadUInt32();
			message._stats.Clear();

			var count = buffer.ReadByte();
			if (!buffer.CanRead(count * (Identifier.Size + 3 * sizeof(ushort))))
				return null;

			for (var i = 0; i < count; ++i)
			{
				message._stats.Add(new Stats
				{
					Player = buffer.ReadIdentifier(),
					Kills = buffer.ReadUInt16(),
					Deaths = buffer.ReadUInt16(),
					Ping = buffer.ReadUInt16()
				});
			}

			return message;
		}

		private struct Stats
		{
			/// <summary>
			///   The number of deaths of the player.
			/// </summary>
			public ushort Deaths;

			/// <summary>
			///   The number of kills scored by the player.
			/// </summary>
			public ushort Kills;

			/// <summary>
			///   The player's network latency.
			/// </summary>
			public ushort Ping;

			/// <summary>
			///   The identifier of the player whose stats are updated.
			/// </summary>
			public Identifier Player;
		}
	}
}