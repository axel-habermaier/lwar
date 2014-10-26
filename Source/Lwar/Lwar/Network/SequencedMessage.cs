namespace Lwar.Network
{
	using System;
	using Messages;
	using Pegasus.Utilities;

	/// <summary>
	///     Associates a message with a sequence number.
	/// </summary>
	public struct SequencedMessage
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="message">The network message.</param>
		/// <param name="sequenceNumber">The sequence number of the message.</param>
		public SequencedMessage(Message message, uint sequenceNumber)
			: this()
		{
			Assert.ArgumentNotNull(message);

			Message = message;
			SequenceNumber = sequenceNumber;
		}

		/// <summary>
		///     Gets the network message.
		/// </summary>
		public Message Message { get; private set; }

		/// <summary>
		///     Gets the sequence number of the message.
		/// </summary>
		public uint SequenceNumber { get; private set; }
	}
}