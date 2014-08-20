namespace Lwar.Network
{
	using System;
	using Messages;
	using Pegasus;
	using Pegasus.Platform;
	using Pegasus.Platform.Memory;

	/// <summary>
	///     Manages the delivery guarantees of all incoming and outgoing messages.
	/// </summary>
	public class DeliveryManager
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
		///     The sequence number of the last reliable message that has been received and processed.
		/// </summary>
		private uint _lastReceivedReliableSequenceNumber;

		/// <summary>
		///     The sequence number of the last unreliable message that has been received and processed.
		/// </summary>
		private uint _lastReceivedUnreliableSequenceNumber;

		/// <summary>
		///     Writes the header of a packet.
		/// </summary>
		/// <param name="buffer">The buffer the header should be written into.</param>
		public void WriteHeader(BufferWriter buffer)
		{
			var header = new PacketHeader(_lastReceivedReliableSequenceNumber);
			header.Write(buffer);
		}

		/// <summary>
		///     Checks whether the reception of the given reliable message has been acknowledged by the remote peer.
		/// </summary>
		/// <param name="message">The message that should be checked.</param>
		public bool IsAcknowledged(ref Message message)
		{
			Assert.That(message.Type.IsReliable(), "The reception of unreliable messages cannot be acknowledged.");
			return message.SequenceNumber <= _lastAckedSequenceNumber;
		}

		/// <summary>
		///     Checks whether the reliable message is the immediate successor to the last processed reliable message. If true,
		///     the last processed sequence number is incremented by one.
		/// </summary>
		/// <param name="sequenceNumber">The sequence number that should be checked.</param>
		public bool AllowReliableDelivery(uint sequenceNumber)
		{
			if (sequenceNumber != _lastReceivedReliableSequenceNumber + 1)
				return false;

			++_lastReceivedReliableSequenceNumber;
			return true;
		}

		/// <summary>
		///     Checks the sequence number to determine whether the message has not been processed before. If true,
		///     updates the last timestamp and processed sequence number, causing future older unreliable messages to be discarded.
		/// </summary>
		/// <param name="sequenceNumber">The sequence number that should be checked.</param>
		public bool AllowUnreliableDelivery(uint sequenceNumber)
		{
			if (sequenceNumber <= _lastReceivedUnreliableSequenceNumber)
				return false;

			_lastReceivedUnreliableSequenceNumber = sequenceNumber;
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
		///     Assigns a sequence number to the message.
		/// </summary>
		/// <param name="message">The message the sequence number should be assigned to.</param>
		public void AssignSequenceNumber(ref Message message)
		{
			if (message.Type.IsReliable())
				message.SequenceNumber = ++_lastAssignedReliableSequenceNumber;
			else
				message.SequenceNumber = ++_lastAssignedUnreliableSequenceNumber;
		}
	}
}