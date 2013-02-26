using System;

namespace Lwar.Client.Network.Messages
{
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;

	public class NameMessage : Message<NameMessage>, IReliableMessage
	{
		/// <summary>
		///   The new player name.
		/// </summary>
		private string _name;

		/// <summary>
		///   The identifier of the player that changed his or her name.
		/// </summary>
		private Identifier _playerId;

		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="session">The game session that should be updated.</param>
		public override void Process(GameSessionOld session)
		{
			Assert.ArgumentNotNull(session, () => session);

			var player = session.GameSession.PlayerMap[_playerId];
			Assert.NotNull(player);

			if (player.Name != null && player.Name != _name)
				Log.Info("{0} was renamed to {1}.", player.Name, _name);

			player.Name = _name;
		}

		/// <summary>
		///   Writes the message into the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the message should be written to.</param>
		public override bool Write(BufferWriter buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			return buffer.TryWrite(this, (b, m) =>
				{
					b.WriteByte((byte)MessageType.Name);
					b.WriteUInt32(m.SequenceNumber);
					b.WriteIdentifier(m._playerId);
					b.WriteString(m._name, Specification.MaximumPlayerNameLength);
				});
		}

		/// <summary>
		///   Gets or sets the sequence number of the message.
		/// </summary>
		public uint SequenceNumber { get; set; }

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="buffer">The buffer from which the instance should be deserialized.</param>
		public static NameMessage Create(BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			return Deserialize(buffer, (b, m) =>
				{
					m._playerId = b.ReadIdentifier();
					m._name = b.ReadString(Specification.MaximumPlayerNameLength);
				});
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="player">The player that changed his or her name.</param>
		/// <param name="playerName">The new name.</param>
		public static NameMessage Create(Identifier player, string playerName)
		{
			Assert.ArgumentNotNullOrWhitespace(playerName, () => playerName);

			var name = playerName.TruncateUtf8(Specification.MaximumPlayerNameLength);
			if (name.Length != playerName.Length)
				Log.Warn("Your player name '{0}' is too long and has been truncated to '{1}'.", name, playerName);

			var nameChange = GetInstance();
			nameChange._playerId = player;
			nameChange._name = name;
			return nameChange;
		}
	}
}