namespace Lwar.Network
{
	using System;
	using Messages;
	using Pegasus.Utilities;

	/// <summary>
	///     Manages the delivery guarantees of all incoming and outgoing messages.
	/// </summary>
	internal class DeliveryManager
	{
		/// <summary>
		///     The sequence number of the last reliable message that has been assigned and acknowledged.
		/// </summary>
		private uint _lastAckedSequenceNumber;

		/// <summary>
		///     The last sequence number that has been assigned to a reliable message.
		/// </summary>
		private uint _lastAssignedReliableSequenceNumber;

		/// <summary>
		///     The last sequence number that has been assigned to an unreliable message.
		/// </summary>
		private uint _lastAssignedUnreliableSequenceNumber;

		/// <summary>
		///     Gets the sequence number of the last reliable message that has been received and processed.
		/// </summary>
		public uint LastReceivedReliableSequenceNumber { get; private set; }

		/// <summary>
		///     Checks whether the reception of the given reliable message has been acknowledged by the remote peer.
		/// </summary>
		/// <param name="sequencedMessage">The message that should be checked.</param>
		public bool IsAcknowledged(SequencedMessage sequencedMessage)
		{
			Assert.That(sequencedMessage.Message.IsReliable, "The reception of unreliable messages cannot be acknowledged.");
			return sequencedMessage.SequenceNumber <= _lastAckedSequenceNumber;
		}

		/// <summary>
		///     Checks whether the reliable message is the immediate successor to the last processed reliable message. If true,
		///     the last processed sequence number is incremented by one.
		/// </summary>
		/// <param name="sequenceNumber">The sequence number that should be checked.</param>
		public bool AllowReliableDelivery(uint sequenceNumber)
		{
			if (sequenceNumber != LastReceivedReliableSequenceNumber + 1)
				return false;

			++LastReceivedReliableSequenceNumber;
			return true;
		}

		/// <summary>
		///     Updates the last acknowledged sequence number.
		/// </summary>
		/// <param name="ackedSequenceNumber">The sequence number that has been acknowledged.</param>
		public void UpdateLastAckedSequenceNumber(uint ackedSequenceNumber)
		{
			if (ackedSequenceNumber > _lastAckedSequenceNumber)
				_lastAckedSequenceNumber = ackedSequenceNumber;
		}

		/// <summary>
		///     Assigns a sequence number to the given reliable message.
		/// </summary>
		/// <param name="message">The message the sequence number should be assigned to.</param>
		public SequencedMessage AssignReliableSequenceNumber(Message message)
		{
			Assert.ArgumentNotNull(message);
			Assert.ArgumentSatisfies(message.IsReliable, "Expected a reliable message.");

			return new SequencedMessage(message, ++_lastAssignedReliableSequenceNumber);
		}

		/// <summary>
		///     Assigns a sequence number to the given unreliable message.
		/// </summary>
		/// <param name="message">The message the sequence number should be assigned to.</param>
		public SequencedMessage AssignUnreliableSequenceNumber(Message message)
		{
			Assert.ArgumentNotNull(message);
			Assert.ArgumentSatisfies(message.IsUnreliable, "Expected an unreliable message.");

			return new SequencedMessage(message, ++_lastAssignedUnreliableSequenceNumber);
		}

		/// <summary>
		///     Assigns an unreliable sequence number.
		/// </summary>
		public uint AssignUnreliableSequenceNumber()
		{
			return ++_lastAssignedUnreliableSequenceNumber;
		}

		/// <summary>
		///     Resets the delivery manager.
		/// </summary>
		public void Reset()
		{
			_lastAckedSequenceNumber = 0;
			_lastAssignedReliableSequenceNumber = 0;
			_lastAssignedUnreliableSequenceNumber = 0;
			LastReceivedReliableSequenceNumber = 0;
		}
	}
}