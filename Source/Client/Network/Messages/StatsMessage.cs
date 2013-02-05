using System;

namespace Lwar.Client.Network.Messages
{
	using System.Collections.Generic;
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;

	public class StatsMessage : Message<StatsMessage>, IUnreliableMessage
	{
		/// <summary>
		///   The new statistics of the players.
		/// </summary>
		private readonly List<Stats> _stats = new List<Stats>();

		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="session">The game session that should be updated.</param>
		public override void Process(GameSession session)
		{
			// TODO
		}

		/// <summary>
		///   Gets or sets the timestamp of the message.
		/// </summary>
		public uint Timestamp { get; set; }

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="buffer">The buffer from which the instance should be deserialized.</param>
		public static StatsMessage Create(BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			return Deserialize(buffer, (b, m) =>
				{
					m._stats.Clear();

					var count = b.ReadByte();
					for (var i = 0; i < count; ++i)
					{
						m._stats.Add(new Stats
						{
							Player = b.ReadIdentifier(),
							Kills = b.ReadUInt16(),
							Deaths = b.ReadUInt16(),
							Ping = b.ReadUInt16()
						});
					}
				});
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