using System;

namespace Lwar.Client.Network.Messages
{
	using System.Runtime.InteropServices;
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;

	public class AddPlayer : PooledObject<AddPlayer>, IReliableMessage
	{
		/// <summary>
		///   The identifier of the player that is added.
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
		///   Gets or sets the sequence number of the message.
		/// </summary>
		public uint SequenceNumber { get; set; }

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="buffer">The buffer from which the instance should be deserialized.</param>
		public static AddPlayer Create(BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);

			if (!buffer.CanRead(Marshal.SizeOf(typeof(Identifier)) + sizeof(uint)))
				return null;

			var message = GetInstance();
			message.SequenceNumber = buffer.ReadUInt32();
			message._playerId = buffer.ReadIdentifier();
			return message;
		}
	}
}