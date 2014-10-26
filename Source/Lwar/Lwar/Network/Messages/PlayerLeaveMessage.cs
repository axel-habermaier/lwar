namespace Lwar.Network.Messages
{
	using System;
	using Pegasus.Entities;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Informs a client that a player has left the game.
	/// </summary>
	[ReliableTransmission(MessageType.PlayerLeave)]
	public sealed class PlayerLeaveMessage : Message
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static PlayerLeaveMessage()
		{
			ConstructorCache.Set(() => new PlayerLeaveMessage());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private PlayerLeaveMessage()
		{
		}

		/// <summary>
		///     Gets the player that has left the game session.
		/// </summary>
		public Identity Player { get; private set; }

		/// <summary>
		///     Gets the reason explaining why the player has left the game session.
		/// </summary>
		public LeaveReason Reason { get; private set; }

		/// <summary>
		///     Serializes the message using the given writer.
		/// </summary>
		/// <param name="writer">The writer that should be used to serialize the message.</param>
		public override void Serialize(BufferWriter writer)
		{
			writer.WriteIdentifier(Player);
			writer.WriteByte((byte)Reason);
		}

		/// <summary>
		///     Deserializes the message using the given reader.
		/// </summary>
		/// <param name="reader">The reader that should be used to deserialize the message.</param>
		public override void Deserialize(BufferReader reader)
		{
			Player = reader.ReadIdentifier();
			Reason = (LeaveReason)reader.ReadByte();
		}

		/// <summary>
		///     Dispatches the message to the given dispatcher.
		/// </summary>
		/// <param name="handler">The dispatcher that should be used to dispatch the message.</param>
		/// <param name="sequenceNumber">The sequence number of the message.</param>
		public override void Dispatch(IMessageHandler handler, uint sequenceNumber)
		{
			handler.OnPlayerLeave(this);
		}

		/// <summary>
		///     Creates a leave message for the given player.
		/// </summary>
		/// <param name="poolAllocator">The pool allocator that should be used to allocate the message.</param>
		/// <param name="player">The player that has left the game session.</param>
		/// <param name="reason">The reason why the player left the game session.</param>
		public static Message Create(PoolAllocator poolAllocator, Identity player, LeaveReason reason)
		{
			Assert.ArgumentNotNull(poolAllocator);
			Assert.ArgumentInRange(reason);
			Assert.ArgumentSatisfies(reason != LeaveReason.Unknown, "The leave reason cannot be unknown.");

			var message = poolAllocator.Allocate<PlayerLeaveMessage>();
			message.Player = player;
			message.Reason = reason;
			return message;
		}

		/// <summary>
		///     Returns a string that represents the message.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0}, Player={1}, LeaveReason={2}", MessageType, Player, Reason);
		}
	}
}