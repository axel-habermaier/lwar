namespace Lwar.Network
{
	using System;
	using System.Collections.Generic;
	using Messages;
	using Pegasus.Utilities;

	/// <summary>
	///     Batches a list of messages for optimized network transmission.
	/// </summary>
	public class BatchedMessage
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="messageType">The type of the batches messages.</param>
		public BatchedMessage(MessageType messageType)
		{
			Assert.ArgumentInRange(messageType);

			MessageType = messageType;
			Messages = new Queue<Message>();
		}

		/// <summary>
		///     Gets the type of the batched messages.
		/// </summary>
		public MessageType MessageType { get; private set; }

		/// <summary>
		///     Gets the messages that the batched message contains.
		/// </summary>
		public Queue<Message> Messages { get; private set; }
	}
}