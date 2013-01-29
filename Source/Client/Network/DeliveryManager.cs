using System;

namespace Lwar.Client.Network
{
	using Messages;
	using Pegasus.Framework;

	/// <summary>
	///   Manages the delivery guarantees of all incoming and outgoing messages.
	/// </summary>
	public class DeliveryManager
	{
		/// <summary>
		///   The sequence number of the last reliable message that has been assigned and acknowledged.
		/// </summary>
		private uint _lastAckedSequenceNumber;

		/// <summary>
		///   Gets the sequence number of the last reliable message that has been received and processed.
		/// </summary>
		public uint LastReceivedSequenceNumber { get; private set; }

		/// <summary>
		///   The maximum of the timestamps of all received unreliable messages.
		/// </summary>
		private uint _lastReceivedTimestamp;

		/// <summary>
		///   The last sequence number that has been assigned to a reliable message.
		/// </summary>
		private uint _lastAssignedSequenceNumber;

		/// <summary>
		///   Checks whether the reception of the given message has been acknowledged by the remote peer.
		/// </summary>
		/// <param name="message">The message that should be checked.</param>
		public bool IsAcknowledged(IReliableMessage message)
		{
			return message.SequenceNumber <= _lastAckedSequenceNumber;
		}

		/// <summary>
		///   Checks whether the reliable message is the immediate successor to the last processed reliable message. If true,
		///   the last processed sequence number is incremented by one.
		/// </summary>
		/// <param name="message">The reliable message that should be checked.</param>
		public bool AllowDelivery(IReliableMessage message)
		{
			Assert.ArgumentNotNull(message, () => message);

			if (message.SequenceNumber == LastReceivedSequenceNumber + 1)
			{
				++LastReceivedSequenceNumber;
				return true;
			}

			return false;
		}

		/// <summary>
		///   Checks whether the timestamp is newer than the ones of all previously received unreliable messages. If true,
		///   updates the last timestamp, causing future older unreliable messages to be discarded.
		/// </summary>
		/// <param name="timestamp">The timestamp of the received unreliable messages that should be delivered.</param>
		public bool AllowDelivery(uint timestamp)
		{
			if (timestamp > _lastReceivedTimestamp)
			{
				_lastReceivedTimestamp = timestamp;
				return true;
			}

			return false;
		}

		/// <summary>
		///   Updates the last acknowledged sequence number.
		/// </summary>
		/// <param name="ackedSequenceNumber">The sequence number that has been acknowledged.</param>
		public void UpdateLastAckedSequenceNumber(uint ackedSequenceNumber)
		{
			if (ackedSequenceNumber > _lastAckedSequenceNumber)
				_lastAckedSequenceNumber = ackedSequenceNumber;
		}

		/// <summary>
		/// Assigns a sequence number to the reliable message.
		/// </summary>
		/// <param name="message">The reliable message the sequence number should be assigned to.</param>
		public void AssignSequenceNumber(IReliableMessage message)
		{
			message.SequenceNumber = _lastAssignedSequenceNumber++;
		}
	}
}