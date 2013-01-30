using System;

namespace Lwar.Client.Network.Messages
{
	using System.Text;
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;

	public class ChangePlayerName : PooledObject<ChangePlayerName>, IReliableMessage
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
		public void Process(GameSession session)
		{
		}

		/// <summary>
		///   Serializes the message into the given buffer, returning false if the message did not fit.
		/// </summary>
		/// <param name="buffer">The buffer the message should be written to.</param>
		public void Write(BufferWriter buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);

			buffer.WriteByte((byte)MessageType.ChangePlayerName);
			buffer.WriteUInt32(SequenceNumber);
			buffer.WriteIdentifier(_playerId);
			buffer.WriteString(_name, Specification.MaxPlayerNameLength);
		}

		/// <summary>
		///   Gets or sets the sequence number of the message.
		/// </summary>
		public uint SequenceNumber { get; set; }

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="buffer">The buffer from which the instance should be deserialized.</param>
		public static ChangePlayerName Create(BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);

			var message = GetInstance();
			message.SequenceNumber = buffer.ReadUInt32();
			message._playerId = buffer.ReadIdentifier();
			message._name = buffer.ReadString(Specification.MaxPlayerNameLength);
			return message;
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="player">The player that changed his or her name.</param>
		/// <param name="name">The new name.</param>
		public static ChangePlayerName Create(Identifier player, string name)
		{
			Assert.ArgumentNotNullOrWhitespace(name, () => name);
			Assert.That(Encoding.UTF8.GetByteCount(name) < Specification.MaxPlayerNameLength, "Name is too long.");

			var nameChange = GetInstance();
			nameChange._playerId = player;
			nameChange._name = name;
			return nameChange;
		}
	}
}