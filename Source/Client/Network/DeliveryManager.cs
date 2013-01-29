using System;

namespace Lwar.Client.Network
{
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;

	/// <summary>
	///   Manages the delivery guarantees of all incoming and outgoing messages.
	/// </summary>
	public class DeliveryManager
	{
		/// <summary>
		///   Determines the current time for the creation of the unreliable message timestamps.
		/// </summary>
		private readonly Time _time;

		/// <summary>
		///   The sequence number of the last reliable message that has been assigned and acknowledged.
		/// </summary>
		private uint _lastAckedSequenceNumber;

		/// <summary>
		///   The last sequence number that has been assigned to a reliable message.
		/// </summary>
		private uint _lastAssignedSequenceNumber;

		/// <summary>
		///   Gets the sequence number of the last reliable message that has been received and processed.
		/// </summary>
		private uint _lastReceivedSequenceNumber;

		/// <summary>
		///   The maximum of the timestamps of all received unreliable messages.
		/// </summary>
		private uint _lastReceivedTimestamp;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public DeliveryManager()
		{
			_time.Offset = _time.Seconds;
		}

		/// <summary>
		///   Writes the header for a packet.
		/// </summary>
		/// <param name="buffer">The buffer the header should be written into.</param>
		public void WriteHeader(BufferWriter buffer)
		{
			var header = new Header(_lastReceivedSequenceNumber, (uint)_time.Milliseconds);
			header.Write(buffer);
		}

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
		/// <param name="ackedSequenceNumber">The sequence number that has been acknowledged.</param>
		public void UpdateLastAckedSequenceNumber(uint ackedSequenceNumber)
		{
			if (ackedSequenceNumber > _lastAckedSequenceNumber)
				_lastAckedSequenceNumber = ackedSequenceNumber;
		}

		/// <summary>
		///   Assigns a sequence number to the reliable message.
		/// </summary>
		/// <param name="message">The reliable message the sequence number should be assigned to.</param>
		public void AssignSequenceNumber(IReliableMessage message)
		{
			message.SequenceNumber = _lastAssignedSequenceNumber++;
		}
	}
}