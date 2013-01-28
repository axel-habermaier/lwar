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
		///   The sequence number of the last reliable message that has been sent and acknowledged.
		/// </summary>
		private uint _lastAckedSequenceNumber;

		/// <summary>
		///   The sequence number of the last reliable message that has been received and processed.
		/// </summary>
		private uint _lastReceivedSequenceNumber;

		/// <summary>
		///   The maximum of the timestamps of all received unreliable messages.
		/// </summary>
		private uint _lastReceivedTimestamp;

		/// <summary>
		///   The sequence number of the last reliable message that has been sent.
		/// </summary>
		private uint _lastSentSequenceNumber;

		/// <summary>
		///   Checks whether the reliable message is the immediate successor to the last processed reliable message. If true,
		///   the last processed sequence number is incremented by one.
		/// </summary>
		/// <param name="message">The reliable message that should be checked.</param>
		public bool AllowDelivery(IReliableMessage message)
		{
			Assert.ArgumentNotNull(message, () => message);

			if (message.SequenceNumber == _lastReceivedSequenceNumber + 1)
			{
				++_lastReceivedSequenceNumber;
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
		/// <param name="acknowledgedSequenceNumber">The sequence number that has been acknowledged.</param>
		public void UpdateLastAckedSequenceNumber(uint acknowledgedSequenceNumber)
		{
			if (acknowledgedSequenceNumber > _lastAckedSequenceNumber)
				_lastAckedSequenceNumber = acknowledgedSequenceNumber;
		}
	}
}