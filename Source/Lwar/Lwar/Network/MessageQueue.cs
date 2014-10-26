namespace Lwar.Network
{
	using System;
	using System.Collections.Generic;
	using Messages;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     The message queue is responsible for packing all queued messages into a packet and sending it to the remote
	///     peer. Reliable messages will be resent until their reception has been acknowledged by the remote peer.
	/// </summary>
	public class MessageQueue : DisposableObject
	{
		/// <summary>
		///     The delivery manager that is used to enforce the message delivery constraints.
		/// </summary>
		private readonly DeliveryManager _deliveryManager;

		/// <summary>
		///     The list of batched messages that have not yet been sent.
		/// </summary>
		private readonly List<BatchedMessage> _batchedMessages = new List<BatchedMessage>();

		/// <summary>
		///     The queued reliable messages that have not yet been sent or that have not yet been acknowledged.
		/// </summary>
		private readonly Queue<SequencedMessage> _reliableMessages = new Queue<SequencedMessage>();

		/// <summary>
		///     The queued unreliable messages that have not yet been sent.
		/// </summary>
		private readonly List<SequencedMessage> _unreliableMessages = new List<SequencedMessage>();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="deliveryManager">The delivery manager that is used to enforce the message delivery constraints.</param>
		public MessageQueue(DeliveryManager deliveryManager)
		{
			Assert.ArgumentNotNull(deliveryManager);
			_deliveryManager = deliveryManager;
		}

		/// <summary>
		///     Enqueues the given message.
		/// </summary>
		/// <param name="message">The message that should be enqueued.</param>
		public void Enqueue(Message message)
		{
			Assert.ArgumentNotNull(message);
			Assert.ArgumentSatisfies(message.IsUnreliable || !message.UseBatchedTransmission,
				"Optimized network serialization is only supported for unreliable messages.");

			message.AcquireOwnership();

			if (message.IsReliable)
				_reliableMessages.Enqueue(_deliveryManager.AssignReliableSequenceNumber(message));
			else if (!message.UseBatchedTransmission)
				_unreliableMessages.Add(_deliveryManager.AssignUnreliableSequenceNumber(message));
			else
			{
				// Add the message to the batched message with the appropriate message type
				foreach (var batchedMessage in _batchedMessages)
				{
					if (batchedMessage.MessageType != message.MessageType)
						continue;

					batchedMessage.Messages.Enqueue(message);
					return;
				}

				// We haven't encountered this message type before, so add a new batched message
				var newBatchedMessage = new BatchedMessage(message.MessageType);
				newBatchedMessage.Messages.Enqueue(message);
				_batchedMessages.Add(newBatchedMessage);
			}
		}

		/// <summary>
		///     Sends all queued messages, resending all reliable messages that have previously been sent but not yet acknowledged.
		///     Returns the number of bytes that have been written.
		/// </summary>
		/// <param name="packetAssembler">The packet assembler that should be used to assemble the packets.</param>
		public void SendMessages(PacketAssembler packetAssembler)
		{
			// Do not send any reliable messages that have already been acknowledged.
			RemoveAckedMessages();

			// We send the reliable messages first.
			packetAssembler.SendReliableMessages(_reliableMessages);

			// Followed by the un-batched unreliable messages.
			packetAssembler.SendUnreliableMessages(_unreliableMessages);

			// Followed by the batched unreliable messages.
			packetAssembler.SendBatchedMessages(_batchedMessages, _deliveryManager);

			// We've sent all unreliable messages, so we no longer need them.
			ClearUnreliableMessages();
		}

		/// <summary>
		///     Removes all acknowledged reliable messages from the queue.
		/// </summary>
		private void RemoveAckedMessages()
		{
			while (_reliableMessages.Count != 0)
			{
				var sequencedMessage = _reliableMessages.Peek();
				if (_deliveryManager.IsAcknowledged(sequencedMessage))
				{
					sequencedMessage.Message.SafeDispose();
					_reliableMessages.Dequeue();
				}
				else
					break;
			}
		}

		/// <summary>
		///     Clears all unreliable messages.
		/// </summary>
		private void ClearUnreliableMessages()
		{
			foreach (var sequencedMessage in _unreliableMessages)
				sequencedMessage.Message.SafeDispose();

			_unreliableMessages.Clear();

			// Also clear all batched messages
			foreach (var batchedMessage in _batchedMessages)
				batchedMessage.Messages.SafeDisposeAll();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Clear();
		}

		/// <summary>
		///     Clears all queued messages.
		/// </summary>
		public void Clear()
		{
			foreach (var sequencedMessage in _reliableMessages)
				sequencedMessage.Message.SafeDispose();

			_reliableMessages.Clear();
			ClearUnreliableMessages();
		}
	}
}