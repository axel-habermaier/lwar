using System;

namespace Lwar.Client.Network.Messages
{
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;

	public class SyncedMessage : Message<SyncedMessage>, IReliableMessage
	{
		/// <summary>
		///   Processes the message, updating the given game session.
		/// </summary>
		/// <param name="session">The game session that should be updated.</param>
		public override void Process(GameSession session)
		{
			// Nothing to do here
		}

		/// <summary>
		///   Gets or sets the sequence number of the message.
		/// </summary>
		public uint SequenceNumber { get; set; }

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="buffer">The buffer from which the instance should be deserialized.</param>
		public static SyncedMessage Create(BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			return GetInstance();
		}
	}
}