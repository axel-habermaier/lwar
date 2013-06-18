using System;

namespace Lwar.Client.Network
{
	using Messages;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Memory;

	/// <summary>
	///   Manages the delivery guarantees of all incoming and outgoing messages.
	/// </summary>
	public class DeliveryManager : DisposableObject
	{
		/// <summary>
		///   Determines the current time for the creation of the unreliable message timestamps.
		/// </summary>
		private readonly Clock _clock = Clock.Create();

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
		///   Writes the header for a packet.
		/// </summary>
		/// <param name="buffer">The buffer the header should be written into.</param>
		public void WriteHeader(BufferWriter buffer)
		{
			var header = new PacketHeader(_lastReceivedSequenceNumber, (uint)_clock.Milliseconds);
			header.Write(buffer);
		}

		/// <summary>
		///   Checks whether the reception of the given reliable message has been acknowledged by the remote peer.
		/// </summary>
		/// <param name="message">The message that should be checked.</param>
		public bool IsAcknowledged(ref Message message)
		{
			Assert.That(message.Type.IsReliable(), "The reception of unreliable messages cannot be acknowledged.");
			return message.SequenceNumber <= _lastAckedSequenceNumber;
		}

		/// <summary>
		///   Checks whether the reliable message is the immediate successor to the last processed reliable message. If true,
		///   the last processed sequence number is incremented by one.
		/// </summary>
		/// <param name="sequenceNumber">The sequence number that should be checked.</param>
		public bool AllowReliableDelivery(uint sequenceNumber)
		{
			if (sequenceNumber == _lastReceivedSequenceNumber + 1)
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
		public bool AllowUnreliableDelivery(uint timestamp)
		{
			// We must allow the delivery of unreliable packets with the same timestamp, as they might contain
			// different data if the packet was split
			if (timestamp >= _lastReceivedTimestamp)
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
		public void AssignSequenceNumber(ref Message message)
		{
			Assert.That(message.Type.IsReliable(), "Cannot assign a sequence number to an unreliable message.");
			message.SequenceNumber = ++_lastAssignedSequenceNumber;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_clock.SafeDispose();
		}
	}
}